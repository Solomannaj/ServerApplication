using ServerAPI.BussinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication29.Controllers
{
    public class CurvesController : ApiController
    {
       public  IDataPublisher dataPublisher;
       private ICSVParser csvParser;

        public   CurvesController(IDataPublisher objPublisher, ICSVParser objCSVParser)
        {
            dataPublisher = objPublisher;
            csvParser = objCSVParser;
        }

        // GET api/values
        [HttpGet]
        public void InvokeTransfer(string curves)
        {
            dataPublisher.PublishData(curves);
            //Action actPublish = () => { dataPublisher.PublishData(curves); };
            //Task.Run(actPublish);
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
    }
}
