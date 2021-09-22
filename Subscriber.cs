using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Protocol;
using System;
using System.Text;

namespace Client2_Subscriber
{
    public class Subscriber
    {
        private static IMqttClient _client;
        private static MqttClientOptionsBuilder _options;

        static void Main(string[] args)
        {
            Subscriber sub = new Subscriber();
            sub.ConnectSubscriber();
        }

        public void ConnectSubscriber()
        {
            string msg = string.Empty;
            try
            {
                Console.WriteLine("----Starting Subscriber----");
                //Create a new MQTT Client
                _client = CreateClient();
                //configure options
                _options = BuildClientOptions();

                //handlers
                _client = ConnectHandler();

                //connect
                _client.ConnectAsync(_options.Build()).Wait();
                Console.WriteLine("----Subsriber is connected----");
                Console.ReadLine();
                Console.WriteLine("Press key to exit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
           
        }

        public IMqttClient CreateClient()
        {
            var factory = new MqttFactory();
            return factory.CreateMqttClient();
        }

        public MqttClientOptionsBuilder BuildClientOptions()
        {
            return new MqttClientOptionsBuilder()
                                .WithClientId("Subscriber")
                                .WithTcpServer("localhost", port: 1884)
                                .WithCredentials(username: "Tanmay", password: "Wipro123")
                                .WithCleanSession();
        }

        public IMqttClient ConnectHandler(IMqttClient mqttClient = null)
        {
            string message = string.Empty;
            _client = _client == null ? mqttClient : _client;
            _client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected successfully with MQTT Brokers.");

                //Subscribe to topic
                _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("test").Build()).Wait();
            });
            _client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });
            _client.UseApplicationMessageReceivedHandler(e =>
            {
                message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });
            return _client;
        }
        public MqttClientSubscribeOptions BuildSubscribeOptions(string topic)
        {
            return new MqttClientSubscribeOptionsBuilder()
                            .WithTopicFilter(topic, MqttQualityOfServiceLevel.ExactlyOnce)
                            .Build();
        }
    }
}


