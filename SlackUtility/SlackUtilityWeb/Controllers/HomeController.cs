using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SlackUtility;

namespace SlackUtilityWeb.Controllers
{
    public class HomeController : Controller
    {
        private SlackWebAPI m_Api
        {
            get
            {
                var api = (SlackWebAPI)Session[nameof(m_Api)];
                if (api == null)
                {
                    api = new SlackWebAPI(
                        ConfigurationManager.AppSettings["ClientId"],
                        ConfigurationManager.AppSettings["ClientSecret"],
                        ConfigurationManager.AppSettings["TeamName"]
                    );
                    Session[nameof(m_Api)] = api;
                }

                return api;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var RedirectUrl = $"http://{new Uri(Request.Url.AbsoluteUri).Authority}/Home/AuthResult";
            ViewBag.Url_Login = m_Api.GenerateOAuthStartUrl(RedirectUrl, GetUseScopes());
            ViewBag.DeleteBorderDate = DeleteBorderDate;
            ViewBag.DeleteBorderFileSize = DeleteBorderFileSize;

            base.OnActionExecuting(filterContext);
        }
        string GetUseScopes()
        {
            return string.Join(" ",
                "files:read",
                "files:write:user"
                );
        }

        public ActionResult Index()
        {
            if (!m_Api.IsLogonSucceeded)
            {
                TempData["RedirectAfterAction"] = RouteData.Values["action"].ToString();
                return Redirect(ViewBag.Url_Login);
            }

            return View();
        }

        public ActionResult DeleteOldAndLargeFilesView()
        {
            if (!m_Api.IsLogonSucceeded)
            {
                TempData["RedirectAfterView"] = RouteData.Values["action"].ToString();
                return Redirect(ViewBag.Url_Login);
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Conditional("DEBUG")]
        void TraceDebug(string msg)
        {
            TraceLog.Trace(TraceEventType.Verbose, msg);

        }

        [HttpGet]
        public async Task<ActionResult> AuthResult(string code, string state)
        {
            try
            {

                TraceDebug($"AuthResult Req URL = {HttpContext.Request.Url.ToString()}");

                var RedirectUrl = new Uri(new Uri(Request.Url.AbsoluteUri), "AuthResult").AbsoluteUri;

                await m_Api.UpdateTokenData(code, RedirectUrl);

                Session[nameof(m_Api)] = m_Api;

                ViewBag.LoginMessage = "ログイン成功しました";

                var viewName = (string)TempData["RedirectAfterView"];
                if (viewName != null)
                {
                    return View(viewName);
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                TraceLog.Trace(TraceEventType.Warning, ex.ToString());
                ViewBag.LoginMessage = "ログイン失敗しました";
                throw;
            }
        }

        private DateTimeOffset UnixTimeBase = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        [HttpGet]
        public async Task<ActionResult> GetFileInfos()
        {
            try
            {

                var files = await m_Api.GetAllFiles();


                var CSVBuf = new StringBuilder();
                CSVBuf.AppendLine(string.Join(",", "id", "filename", "filesize", "created", "user"));
                foreach (var file in files)
                {
                    var createdTime = UnixTimeBase.AddSeconds(file.created);

                    CSVBuf.AppendLine(string.Join(",", file.id, file.name, file.size, createdTime.ToLocalTime(), file.user));
                }

                ViewBag.GetFileInfosResult = CSVBuf.ToString();

                return View("Index");
            }
            catch (Exception ex)
            {
                TraceLog.Trace(TraceEventType.Warning, ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFile(string fileId)
        {

            try
            {

                await m_Api.DeleteFile(fileId);

                ViewBag.DeleteFileResult = "削除成功";

                return View("Index");
            }
            catch (Exception ex)
            {
                TraceLog.Trace(TraceEventType.Warning, ex.ToString());
                throw;
            }
        }

        private readonly int DeleteBorderFileSize = 10 * 1024 * 1024;
        private readonly DateTimeOffset DeleteBorderDate = new DateTimeOffset(2018, 1, 1, 0, 0, 0, new TimeSpan(9, 0, 0));

        DateTimeOffset GetFromUnixTime(int unixTime)
        {
            return UnixTimeBase.AddSeconds(unixTime);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteOldAndLargeFiles(string resultView)
        {
            try
            {

                var files = await m_Api.GetAllFiles();

                var deleteFiles = files.Where(item =>
                    (DeleteBorderFileSize <= item.size) ||
                    (GetFromUnixTime(item.created).ToLocalTime() <= DeleteBorderDate));

                foreach (var file in deleteFiles)
                {
                    SlackUtility.TraceLog.Trace(TraceEventType.Information, $"Delete Start, {string.Join(",", file.id, file.name, file.size, GetFromUnixTime(file.created).ToLocalTime(), file.user)}");
                    await m_Api.DeleteFile(file.id);
                }

                var retStr = new StringBuilder("以下のファイルの削除に成功しました。" + Environment.NewLine);
                retStr.AppendLine(string.Join(Environment.NewLine, deleteFiles.Select(item => item.name)));
                TraceLog.Trace(TraceEventType.Information, retStr.ToString());
                ViewBag.DeleteOldAndLargeFilesResult = retStr.ToString();

                if (resultView != null)
                {
                    return View(resultView);
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                TraceLog.Trace(TraceEventType.Warning, ex.ToString());
                throw;
            }
        }
    }
}