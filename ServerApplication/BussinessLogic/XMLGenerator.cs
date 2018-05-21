using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace ServerApplication.BussinessLogic
{
    public interface IXMLGenerator
    {
        MemoryStream GetMemStream(string curveNames, List<long> minIndexes, List<long> maxIndexes);
    }

    public class XMLGenerator : IXMLGenerator
    {
        public  MemoryStream GetMemStream(string curveNames, List<long> minIndexes, List<long> maxIndexes)
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
                foreach (var item in curveNames.Split(','))
                {
                    CreateCurves(item, minIndexes[i], maxIndexes[i], writer);
                    i++;
                }

                //Generate Values
                writer.WriteStartElement("logData");
                writer.WriteStartElement("header");
                writer.WriteString("index," + curveNames);
                writer.WriteEndElement();
                foreach (var value in CurvesData.LSTCurvesData)
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

                throw new Exception(string.Format("Error in writing values to xml - {0}", ex.Message));
            }
            
        }

        private void CreateCurves(string curveName, long minIndex, long maxIndex, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("logCurveInfo");
                writer.WriteAttributeString("id", curveName);

                writer.WriteElementString("minIndex", minIndex.ToString());
                writer.WriteElementString("maxIndex", maxIndex.ToString());

                writer.WriteElementString("typeLogData", "double");

                writer.WriteEndElement();
            }
            catch (Exception ex)
            {

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