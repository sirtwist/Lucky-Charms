using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LZA.Models
{

    public class VisionResponse
    {
        public string id { get; set; }
        public string project { get; set; }
        public string iteration { get; set; }
        public DateTime created { get; set; }
        public Prediction[] predictions { get; set; }
    }

    public class Prediction
    {
        public float probability { get; set; }
        public string tagId { get; set; }
        public string tagName { get; set; }
        public Boundingbox boundingBox { get; set; }
    }

    public class Boundingbox
    {
        public float left { get; set; }
        public float top { get; set; }
        public float width { get; set; }
        public float height { get; set; }
    }

}