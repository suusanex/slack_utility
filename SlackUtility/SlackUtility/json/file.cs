﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackUtility.json
{
    public class Reaction
    {
        public string name { get; set; }
        public int count { get; set; }
        public List<string> users { get; set; }
    }

    public class file
    {
        public string id { get; set; }
        public int created { get; set; }
        public int timestamp { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string mimetype { get; set; }
        public string filetype { get; set; }
        public string pretty_type { get; set; }
        public string user { get; set; }
        public string mode { get; set; }
        public bool editable { get; set; }
        public bool is_external { get; set; }
        public string external_type { get; set; }
        public string username { get; set; }
        public int size { get; set; }
        public string url_private { get; set; }
        public string url_private_download { get; set; }
        public string thumb_64 { get; set; }
        public string thumb_80 { get; set; }
        public string thumb_360 { get; set; }
        public string thumb_360_gif { get; set; }
        public int thumb_360_w { get; set; }
        public int thumb_360_h { get; set; }
        public string thumb_480 { get; set; }
        public int thumb_480_w { get; set; }
        public int thumb_480_h { get; set; }
        public string thumb_160 { get; set; }
        public string permalink { get; set; }
        public string permalink_public { get; set; }
        public string edit_link { get; set; }
        public string preview { get; set; }
        public string preview_highlight { get; set; }
        public int lines { get; set; }
        public int lines_more { get; set; }
        public bool is_public { get; set; }
        public bool public_url_shared { get; set; }
        public bool display_as_bot { get; set; }
        public int num_stars { get; set; }
        public bool is_starred { get; set; }
        public List<Reaction> reactions { get; set; }
        public int comments_count { get; set; }
    }
}
