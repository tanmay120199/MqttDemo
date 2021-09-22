//using System;
//using Xunit;

//namespace MqttTest
//{
//    public class UnitTest1
//    {
//        [Fact]
//        public void Test1()
//        {

//        }
//    }
//}


using Xunit;
using Client1_Publisher;
using Client2_Subscriber;
using Broker;
using System.Threading.Tasks;
using MQTTnet.Server;
using MQTTnet.Client;
using System.Text;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;

namespace DemoMQTTConsole.Tests
{
    public class MQTTTests
    {
        private readonly MQTTBroker broker;
        private readonly Publisher publisher;
        private readonly Subscriber subscriber;

        public MQTTTests()
        {
            broker = new MQTTBroker();
            publisher = new Publisher();
            subscriber = new Subscriber();
        }

        [Fact]
        public async Task Subscriber_WhenSubscribedToTopic_GetsCorrectMessageWhenPublished()
        {
            //Arrange
            var msgCount = 0;
            MqttApplicationMessage receivedMessage = null;
            var bOptions = broker.BuildServerOptions();
            var server = broker.CreateServer();
            var pClient = publisher.CreateClient();
            var pcOptions = publisher.BuildClientOptions();
            var sClient = subscriber.CreateClient();
            var scOptions = subscriber.BuildClientOptions();
            sClient.UseApplicationMessageReceivedHandler(e => { receivedMessage = e.ApplicationMessage; msgCount++; });

            //Act
            await server.StartAsync(bOptions.Build());
            await sClient.ConnectAsync(scOptions.Build());
            await sClient.SubscribeAsync(subscriber.BuildSubscribeOptions("Test"));
            await pClient.ConnectAsync(pcOptions.Build());
            await pClient.PublishAsync(publisher.BuildMessage("Test", "demo"));
            await Task.Delay(10);
            await server.StopAsync();

            //Assert
            Assert.NotNull(receivedMessage);
            Assert.Equal("Test", receivedMessage.Topic);
            Assert.Equal(MqttQualityOfServiceLevel.ExactlyOnce, receivedMessage.QualityOfServiceLevel);
            Assert.Equal("demo", Encoding.UTF8.GetString(receivedMessage.Payload));
            Assert.Equal(1, msgCount);
        }

        [Fact]
        public async Task Subscriber_WhenSubscribedToTopic_GetsAllMessagesWhenPublished()
        {
            //Arrange
            var msgCount = 0;
            MqttApplicationMessage receivedMessage = null;
            var bOptions = broker.BuildServerOptions();
            var server = broker.CreateServer();
            var pClient = publisher.CreateClient();
            var pcOptions = publisher.BuildClientOptions();
            var sClient = subscriber.CreateClient();
            var scOptions = subscriber.BuildClientOptions();
            sClient.UseApplicationMessageReceivedHandler(e => { receivedMessage = e.ApplicationMessage; msgCount++; });

            //Act
            await server.StartAsync(bOptions.Build());
            await sClient.ConnectAsync(scOptions.Build());
            await sClient.SubscribeAsync(subscriber.BuildSubscribeOptions("Test"));
            await pClient.ConnectAsync(pcOptions.Build());
            await pClient.PublishAsync(publisher.BuildMessage("Test", "demo"));
            await pClient.PublishAsync(publisher.BuildMessage("Test", "demo1"));
            await pClient.PublishAsync(publisher.BuildMessage("Test", "demo2"));
            await Task.Delay(10);
            await server.StopAsync();

            //Assert

            Assert.Equal(3, msgCount);
        }


    }
}




