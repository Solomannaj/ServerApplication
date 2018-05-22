using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApplication.Models
{
    public class CurveInfo
    {
        public string Name { get; set; }

        public long MaxIndex { get; set; }

        public long MinIndex { get; set; }
    }
}