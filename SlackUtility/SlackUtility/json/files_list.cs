using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackUtility.json
{
    public class Paging
    {
        public int count { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
    }

    public class files_list
    {
        public bool ok { get; set; }
        public List<file> files { get; set; }
        public Paging paging { get; set; }
    }
}
