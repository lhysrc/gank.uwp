using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GankIO.Models
{

    public class ContentResultRoot
    {
        public bool error { get; set; }
        public ContentResult[] results { get; set; }
    }

    public class ContentResult
    {
        public string _id { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public DateTime publishedAt { get; set; }
        public string rand_id { get; set; }
        public string title { get; set; }
        public DateTime updated_at { get; set; }
    }

}
