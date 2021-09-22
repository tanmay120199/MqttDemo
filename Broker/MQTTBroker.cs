using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;

namespace Broker
{
    public class MQTTBroker
    {
        public static void Main(string[] args)
        {
            MQTTBroker mb = new MQTTBroker();
            mb.ConnectMQTTBroker();
        }

        public string ConnectMQTTBroker(string message = null)
        {
            Console.WriteLine("----Connecting Broker----");
            //configure options
            var optionsBuilder = BuildServerOptions();

            // creates a new mqtt server     
            var mqttServer = CreateServer();
            mqttServer.StartAsync(optionsBuilder.Build()).Wait();

            Console.WriteLine("Broker is running.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            mqttServer.StopAsync().Wait();
            //Console.ReadLine();
            return "success";
        }
        public IMqttServer CreateServer()
        {
            return new MqttFactory().CreateMqttServer();
        }

        public MqttServerOptionsBuilder BuildServerOptions()
        {
            return new MqttServerOptionsBuilder()
                                        //.WithConnectionValidator(c =>
                                        //{
                                        //    Console.WriteLine($"{c.ClientId} connection validator for c.Endpoint:{c.Endpoint}");
                                        //    c.ReasonCode = MqttConnectReasonCode.Success;
                                        //})
                                        // set endpoint to localhost
                                        .WithDefaultEndpoint()
                                        // port used will be 1884
                                        .WithDefaultEndpointPort(1884);
        }
    }
}
