using ServerApplication.BusinessLogic;
using ServerApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ServerApplication.BussinessLogic
{

    public interface ICSVParser
    {
        void PopulateCurvesData();

        IEnumerable<string> GetDataRows(string curves);

        List<string> GetDataRows(List<CurveInfo> curveInfos);

        string GetCurveHeaders();
    }

    public class CSVParser : ICSVParser
    {
       // private string[] csvRows;

        public IEnumerable<string> GetDataRows(string curves)
        {
            string[] arrCurves= curves.Split(',');
            List<Curve> lstRowCurves;
            string[] result;
            long index;

            for (int i=0;i<CurvesData.RowCount;i++)
            {
                result = new string[arrCurves.Length + 1];

                lstRowCurves = CurvesData.LSTCurvesData.Where(x => x.RowIndex == i).ToList();
                if (lstRowCurves.Count == 0)
                    continue;
                index = lstRowCurves.First().Index;
                result[0] =Convert.ToString(index);
                for (int j= 0; j < arrCurves.Count();j++)
                {
                    for (int k = 0; k < lstRowCurves.Count; k++)
                    {
                        if(arrCurves[j]== lstRowCurves[k].Name)
                        {
                            result[j+1] =lstRowCurves[k].Value;
                            break;
                        }
                    }
                }

                yield return string.Join(",", result);
            }
        }

        public List<string> GetDataRows(List<CurveInfo> curveInfos)
        {
            List<string> lstResult = new List<string>();

            List<Curve> lstRowCurves;
            string[] result;
            // string index;
            long index;

            for (int i = 0; i < CurvesData.RowCount; i++)
            {
                result = new string[curveInfos.Count + 1];

                lstRowCurves = CurvesData.LSTCurvesData.Where(x => x.RowIndex == i).ToList();
                if (lstRowCurves.Count == 0)
                    continue;
                int resultIndex = 1;

                for (int j = 0; j < curveInfos.Count(); j++)
                {
                    for (int k = 0; k < lstRowCurves.Count; k++)
                    {
                        if (curveInfos[j].Name == lstRowCurves[k].Name)
                        {
                            if(curveInfos[j].MinIndex <= lstRowCurves[k].Index && curveInfos[j].MaxIndex >= lstRowCurves[k].Index)
                            {
                                if(result[0] == null)
                                {
                                    index = lstRowCurves[k].Index;
                                    result[0] =Convert.ToString(index);
                                }
                                result[resultIndex] = lstRowCurves[k].Value;
                                resultIndex ++ ;
                                break;
                            }
                           
                        }
                    }
                }
                if(result[1] != null)
                  lstResult.Add( string.Join(",", result));
            }

            return lstResult;
        }

        public string GetCurveHeaders()
        { 
            try
            {
                string[] csvRows=new string[CurvesData.LSTCurvesHeaders.Count()];
                
                for(int i=0; i< CurvesData.LSTCurvesHeaders.Count();i++)
                {
                    csvRows[i] = CurvesData.LSTCurvesHeaders[i].Name;
                }

                return string.Join(",", csvRows);
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Unable to fetch CSV headers - {0}", ex.Message));
                throw new Exception(string.Format("Unable to fetch CSV headers - {0}", ex.Message));
            }
        }

        public void PopulateCurvesData()
        {
            try
            {
                CurvesData.LSTCurvesData.Clear();
                CurvesData.LSTCurvesHeaders.Clear();
                CurvesData.RowCount = 0;

                string[] csvRows;
                string[] csvColumns;
                Curve curve;
                CurveHeader header;
                CurveHeader correspondingHeader;
                long index;
                csvRows = System.IO.File.ReadAllLines(ConfigurationManager.AppSettings["CSVPath"]);
                if (csvRows.Count() > 0 && csvRows[0] != string.Empty)
                {

                    //adding header details
                    csvColumns = csvRows[0].Split(',');
                    for (int j = 1; j < csvColumns.Count(); j++)
                    {
                        header = new CurveHeader() { Name = csvColumns[j], ColumnIndex = j - 1 };
                        CurvesData.LSTCurvesHeaders.Add(header);
                    }

                    //adding data
                    for (int i = 1; i < csvRows.Count(); i++)
                    {
                       
                        csvColumns =csvRows[i].Split(',');
                        if (csvColumns[0] == string.Empty)
                            continue;
                        index =Convert.ToInt64(csvColumns[0]);
                        for (int j = 1; j < csvColumns.Count(); j++)
                        {
                            correspondingHeader = CurvesData.LSTCurvesHeaders.Where(x => x.ColumnIndex == j-1).First();
                            curve = new Curve() { RowIndex = i-1, ColumnIndex = j-1, Name = correspondingHeader.Name, Value = csvColumns[j], Index = index };
                            CurvesData.LSTCurvesData.Add(curve);
                        }
                        
                    }

                    CurvesData.RowCount = csvRows.Count() - 1;
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
