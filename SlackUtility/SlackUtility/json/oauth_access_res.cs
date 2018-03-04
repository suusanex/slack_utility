using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackUtility.json
{
   
    public class oauth_access_res
    {
        public bool ok { get; set; }
        public string access_token { get; set; }
        public string scope { get; set; }
        public string user_id { get; set; }
        public string team_name { get; set; }
        public string team_id { get; set; }
    }
}
