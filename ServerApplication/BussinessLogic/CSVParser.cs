using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

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

            string[] headerColumns = CurvesData.LSTCurvesData[0].Split(',');
            string[] curvesList = curves.Split(',');
            string[] csvColumns;
            string[] result;

            int[] requiredColumnIndexes = headerColumns.Where(x => curvesList.Contains(x)|| x=="index")
                                                       .Select(x => headerColumns.ToList().IndexOf(x)).ToArray();

            foreach (string line in CurvesData.LSTCurvesData)
            {
                csvColumns = line.Split(',');
                if(csvColumns[0]!="index")
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
                string[] csvRows;
                csvRows = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["CSVPath"]);
                if (csvRows.Count() > 0 && csvRows[0] != string.Empty)
                {
                    CurvesData.LSTCurvesData = csvRows.ToList();
                    string result = csvRows[0].Replace("index,", string.Empty);
                    return result.TrimEnd().TrimStart();
                }
                else
                {
                    throw new Exception("Invalid CSV file");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to read CSV file - {0}", ex.Message));
            }
        }
    }
}
