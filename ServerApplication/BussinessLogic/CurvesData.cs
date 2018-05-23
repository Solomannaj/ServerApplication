using ServerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerApplication.BussinessLogic
{
    public static class CurvesData
    {
        static CurvesData()
        {
            LSTCurvesData = new List<Curve>();
            LSTCurvesHeaders = new List<CurveHeader>();
        }
        public static List<Curve> LSTCurvesData { get; set; }

        public static List<CurveHeader> LSTCurvesHeaders { get; set; }

        public static int RowCount { get; set; }
    }
}