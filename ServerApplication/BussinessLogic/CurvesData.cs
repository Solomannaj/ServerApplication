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
            LSTCurvesData = new List<string>();
        }
        public static List<string> LSTCurvesData { get; set; }

        public static string[] ArrCurvesHeaders { get; set; }
    }
}