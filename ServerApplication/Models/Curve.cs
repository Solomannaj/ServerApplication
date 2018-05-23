using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApplication.Models
{
    public class Curve
    {
        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public string Name { get; set; }

        public long Index { get; set; }

        public string Value { get; set; }
    }
}