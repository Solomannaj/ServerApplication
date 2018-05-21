using ServerApplication.BussinessLogic;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ServerApplication.Controllers
{
    public class CurvesController : ApiController
    {
       public  IDataPublisher dataPublisher;
       private ICSVParser csvParser;
       private IXMLGenerator xmlGenerator;

        public   CurvesController(IDataPublisher objPublisher, ICSVParser objCSVParser, IXMLGenerator objXMLGenerator)
        {
            dataPublisher = objPublisher;
            csvParser = objCSVParser;
            xmlGenerator = objXMLGenerator;
        }

        [HttpGet]
        public void InvokeTransfer(string curves)
        {
            dataPublisher.PublishData(curves);
        }

        [HttpGet]
        public void StopTransfer()
        {
            dataPublisher.StopPublish();
        }

        [HttpGet]
        public string GetCurveHeaders()
        {
            return csvParser.GetCurveHeaders();
        }

        [HttpGet]
        public HttpResponseMessage ExportData(string curveNames, List<long> minIndexes, List<long> maxIndexes)
        {
            var dataStream = xmlGenerator.GetMemStream(curveNames, minIndexes, maxIndexes);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(dataStream.GetBuffer())
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("xmlfile")
            {
                FileName = string.Format(ConfigurationManager.AppSettings["XMLFileName"])
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
