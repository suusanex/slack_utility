using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackUtility
{
    [Serializable]
    public class SlackWebAPI
    {
        public SlackWebAPI(string ClientId, string ClientSecret, string TeamName)
        {

            m_ClientId = ClientId;
            m_ClientSecret = ClientSecret;
            m_TeamName = TeamName;
        }
        internal string m_ClientId;
        internal string m_ClientSecret;
        internal string m_TeamName;
        public string GenerateOAuthStartUrl(string RedirectUrl, string Scope)
        {
            return string.Join("&",
                $"https://slack.com/oauth/authorize?",
                $"client_id={m_ClientId}",
                $"scope={Scope}",
                $"redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}",
                $"team={m_TeamName}"
                );
        }


        [Conditional("DEBUG")]
        void TraceDebug(string msg)
        {
            TraceLog.Trace(TraceEventType.Verbose, msg);


        }

        public bool IsLogonSucceeded => m_TokenData != null;

        json.oauth_access_res m_TokenData;

        public async Task UpdateTokenData(string AuthCode, string RedirectUrl)
        {
            var GetTokenUrl = $"https://slack.com/api/oauth.access?" + string.Join("&",
                $"code={AuthCode}",
                $"client_id={m_ClientId}",
                $"client_secret={HttpUtility.UrlEncode(m_ClientSecret)}",
                $"redirect_uri={HttpUtility.UrlEncode(RedirectUrl)}"
                );


            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(GetTokenUrl);


                if ((int)res.StatusCode < 200 || 300 <= (int)res.StatusCode)
                {
                    var resData = await res.Content.ReadAsStringAsync();
                    TraceDebug(resData);
                    throw new Exception(resData + Environment.NewLine + res.ToString());
                }

                var resStr = await res.Content.ReadAsStringAsync();
                TraceDebug(resStr);
                m_TokenData = JsonConvert.DeserializeObject<json.oauth_access_res>(resStr);
                TraceLog.Trace(TraceEventType.Information, $"UpdateToken Sucess User={m_TokenData.user_id}");
            }

        }

        public async Task<IEnumerable<json.file>> GetAllFiles()
        {
            var onePageCount = 100;
            var getUrl = $"https://slack.com/api/files.list?" + string.Join("&",
                                      $"token={m_TokenData.access_token}",
                                      $"count={onePageCount}"
                                  );

            var files = new List<json.file>();

            using (var client = new HttpClient())
            {
                int maxPages = 1;
                for (int i = 1; i <= maxPages; i++)
                {
                    var res = await client.GetAsync(string.Join("&", getUrl, $"page={i}"));

                    if ((int) res.StatusCode < 200 || 300 <= (int) res.StatusCode)
                    {
                        var resData = await res.Content.ReadAsStringAsync();
                        TraceDebug(resData);
                        throw new Exception(resData + Environment.NewLine + res.ToString());
                    }

                    var resStr = await res.Content.ReadAsStringAsync();
                    TraceDebug($"files.list, [{i}], {resStr}");
                    var ret1 = JsonConvert.DeserializeObject<json.files_list>(resStr);

                    files.AddRange(ret1.files);

                    if (maxPages == 1)
                    {
                        maxPages = ret1.paging.pages;
                    }
                }


            }

            return files;

        }

        public async Task DeleteFile(string deleteFileId)
        {

            var url = $"https://slack.com/api/files.delete";
            var param = new JObject()
            {
                { "file", deleteFileId }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", m_TokenData.access_token);
                var PostRet = await client.PostAsync(url,
                    new StringContent(param.ToString(), Encoding.UTF8, "application/json"));

                if ((int)PostRet.StatusCode < 200 || 300 <= (int)PostRet.StatusCode)
                {
                    var PostRetData = await PostRet.Content.ReadAsStringAsync();
                    TraceDebug(PostRetData);
                    throw new Exception(PostRetData + Environment.NewLine + PostRet.ToString());
                }
                
                var resStr = await PostRet.Content.ReadAsStringAsync();
                TraceDebug($"resStr, {resStr}");
                var res = JObject.Parse(resStr);
                if (!res["ok"].Value<bool>())
                {
                    throw new Exception($"files.delete, fail, {res["error"]}");
                }

            }
        }

    }
}
