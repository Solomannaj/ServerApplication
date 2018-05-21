using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Configuration;

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

        public void Dispose()
        {
            try
            {
              channel.Dispose();
            }
            catch { }

            try
            {
                connection.Dispose();
            }
            catch { }
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

                IEnumerable<string> csvTable = csvParser.ReadCSVLines(curves);

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

                throw new Exception(string.Format("Error in publishing data - {0}", ex.Message));
            }
        }

        private void PublishRowData(string message)
        {
            try
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception occured with RabbitMQ channel - {0}", ex.Message));
            }
        }
    }
}
