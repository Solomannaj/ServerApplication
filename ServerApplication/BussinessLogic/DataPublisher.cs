using RabbitMQ.Client;
using ServerApplication.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerApplication.BussinessLogic
{
    public interface IDataPublisher
    {
        void PublishData(string curves);
        void StopPublish();
    }

    public class DataPublisher : IDataPublisher
    {
        private  IConnection connection;
        private  IModel channel;
        private ConnectionFactory factory;
        private ICSVParser csvParser;

        public bool TrabsferInvoked { get; set; }

        public DataPublisher(ICSVParser objParser)
        {
            try
            {
                csvParser = objParser;
                factory = new ConnectionFactory()
                {
                    HostName = ConfigurationManager.AppSettings["RabbitMQServer"],
                    UserName = ConfigurationManager.AppSettings["RabbitMQUser"],
                    Password = ConfigurationManager.AppSettings["RabbitMQPassword"],
                    VirtualHost = ConfigurationManager.AppSettings["RabbitMQVirtualHost"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["RabbitMQPort"])
                };

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Failed to connect RabbitMQ channel - {0}", ex.Message));
                throw new Exception(string.Format("Failed to connect RabbitMQ channel - {0}", ex.Message));
            }
           
        }
       

        public void StopPublish()
        {
            TrabsferInvoked = false;
        }

        public void PublishData(string curves)
        {
            try
            {
                TrabsferInvoked = true;

                IEnumerable<string> csvTable = csvParser.GetDataRows(curves);

                //To avoid differed execution of enumerable
                List<string> dataToPublish = csvTable.ToList();

                foreach (string item in dataToPublish)
                {
                    if (!TrabsferInvoked)
                        break;
                    PublishRowData(item);
                    Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["DataTranferRate"]));
                }
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Error in publishing data - {0}", ex.Message));
                throw new Exception(string.Format("Error in publishing data - {0}", ex.Message));
            }
        }

        private void PublishRowData(string message)
        {
            try
            {
                channel.QueueDeclare(queue: ConfigurationManager.AppSettings["QueueName"], durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: ConfigurationManager.AppSettings["QueueName"], basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                Logger.WrieException(string.Format("Exception occured with RabbitMQ channel - {0}", ex.Message));
                throw new Exception(string.Format("Exception occured with RabbitMQ channel - {0}", ex.Message));
            }
        }
    }
}
