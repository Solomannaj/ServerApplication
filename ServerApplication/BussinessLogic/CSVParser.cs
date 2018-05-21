using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerAPI.BussinessLogic
{

    public interface ICSVParser
    {
        IEnumerable<string> ReadCSVLines(string curves);

        string GetCurveHeaders();
    }

    public class CSVParser : ICSVParser
    {
        private string[] csvRows;

        public IEnumerable<string> ReadCSVLines(string curves)
        {

            string[] headerColumns = csvRows[0].Split(',');
            string[] curvesList = curves.Split(',');
            string[] csvColumns;
            string[] result;

            int[] requiredColumnIndexes = headerColumns.Where(x => curvesList.Contains(x)|| x=="index")
                                                       .Select(x => headerColumns.ToList().IndexOf(x)).ToArray();

            foreach (string line in csvRows)
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
            csvRows = null;
            csvRows = System.IO.File.ReadAllLines(@"C:\MyDrive\Petro\PPP.csv");
            if(csvRows.Count() > 0 && csvRows[0] !=string.Empty)
            {
                string result = csvRows[0].Replace("index,", string.Empty);
                return result.TrimEnd().TrimStart();
            }
            return string.Empty;
        }
    }
}
