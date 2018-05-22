using ServerApplication.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ServerApplication.BussinessLogic
{

    public interface ICSVParser
    {
        IEnumerable<string> ReadCSVLines(string curves);

        string GetCurveHeaders();
    }

    public class CSVParser : ICSVParser
    {
       // private string[] csvRows;

        public IEnumerable<string> ReadCSVLines(string curves)
        {

            string[] headerColumns = CurvesData.ArrCurvesHeaders;
            string[] curvesList = curves.Split(',');
            string[] csvColumns;
            string[] result;

            int[] requiredColumnIndexes = headerColumns.Where(x => curvesList.Contains(x)|| x== ConfigurationManager.AppSettings["IndexColumn"].TrimEnd().TrimStart())
                                                       .Select(x => headerColumns.ToList().IndexOf(x)).ToArray();

            foreach (string line in CurvesData.LSTCurvesData)
            {
                csvColumns = line.Split(',');
                if(csvColumns[0]!= ConfigurationManager.AppSettings["IndexColumn"].TrimEnd().TrimStart())
                {
                    result = csvColumns.Where(x => requiredColumnIndexes.Contains(csvColumns.ToList().IndexOf(x))).ToArray();

                    yield return string.Join(",", result);
                }
            }

        }

        public string GetCurveHeaders()
        { 
            try
            {
                CurvesData.LSTCurvesData.Clear();
                CurvesData.ArrCurvesHeaders = null;

                string[] csvRows;
                csvRows = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["CSVPath"]);
                if (csvRows.Count() > 0 && csvRows[0] != string.Empty)
                {
                    CurvesData.LSTCurvesData = csvRows.ToList();
                    string result = csvRows[0].Replace(ConfigurationManager.AppSettings["IndexColumn"].TrimEnd().TrimStart() + ",", string.Empty);
                    CurvesData.ArrCurvesHeaders = csvRows[0].Split(',');
                    CurvesData.LSTCurvesData.RemoveAt(0);
                    return result.TrimEnd().TrimStart();
                }
                else
                {
                    throw new Exception("Invalid CSV file");
                }
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Unable to read CSV file - {0}", ex.Message));
                throw new Exception(string.Format("Unable to read CSV file - {0}", ex.Message));
            }
        }
    }
}
