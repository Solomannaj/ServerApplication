using ServerApplication.BusinessLogic;
using ServerApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace ServerApplication.BussinessLogic
{
    public interface IXMLGenerator
    {
        MemoryStream GetMemStream(List<CurveInfo> curveInfo);
    }

    public class XMLGenerator : IXMLGenerator
    {
        ICSVParser csvParser;
        public XMLGenerator(ICSVParser objCSVParser)
        {
            csvParser = objCSVParser;
        }

        public  MemoryStream GetMemStream(List<CurveInfo> lstCurveInfo)
        {
            try
            {
                var memoryStream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(memoryStream, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("log");
                CreateIndex(writer);
                int i = 0;

                //GenerateCurves
                string curveNames=string.Empty;
                foreach (CurveInfo item in lstCurveInfo)
                {
                    curveNames = curveNames + item.Name + ",";
                    CreateCurves(item, writer);
                    i++;
                }
                curveNames = curveNames.TrimEnd(',');

                //Generate Values
                writer.WriteStartElement("logData");
                writer.WriteStartElement("header");
                writer.WriteString("index," + curveNames);
                writer.WriteEndElement();

                List<string> lstData = csvParser.GetDataRows(lstCurveInfo);
                foreach (var value in lstData)
                {
                    CreateValues(value, writer);

                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();

                return memoryStream;
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Failed to generate xml file - {0}", ex.Message));
                throw new Exception(string.Format("Failed to generate xml file - {0}", ex.Message));
            }
          
        }

        private void CreateValues(string value, XmlWriter writer)
        {
            try
            {
                writer.WriteElementString("data", value);
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Error in writing values to xml - {0}", ex.Message));
                throw new Exception(string.Format("Error in writing values to xml - {0}", ex.Message));
            }
            
        }

        private void CreateCurves(CurveInfo curveInfo, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("logCurveInfo");
                writer.WriteAttributeString("id", curveInfo.Name);

                writer.WriteElementString("minIndex", curveInfo.MinIndex.ToString());
                writer.WriteElementString("maxIndex", curveInfo.MaxIndex.ToString());

                writer.WriteElementString("typeLogData", "double");

                writer.WriteEndElement();
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Error in writing curves to xml - {0}", ex.Message));
                throw new Exception(string.Format("Error in writing curves to xml - {0}", ex.Message));
            }
         
        }

        private void CreateIndex(XmlTextWriter writer)
        {
            writer.WriteStartElement("logCurveInfo");
            writer.WriteAttributeString("id", "index");
            writer.WriteElementString("typeLogData", "long");
            writer.WriteEndElement();
        }
    }
}