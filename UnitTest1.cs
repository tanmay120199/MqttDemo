using Client1_Publisher;
using Client2_Subscriber;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Broker;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xTests
{
    public class UnitTest1
    {

        private readonly MQTTBroker broker;
        private readonly Publisher publisher;
        private readonly Subscriber subscriber;

        public UnitTest1()
        {
            broker = new MQTTBroker();
            publisher = new Publisher();
            subscriber = new Subscriber();
        }

        [Fact]
        public async Task VerifyMessage_Same_PubSub()
        {
            //Arrange
            
            MqttApplicationMessage receivedMessage = null;
            var brokerOptions = broker.BuildServerOptions();
            var brokerserver = broker.CreateServer();
            var pubOptions = publisher.BuildClientOptions();
            var pubClient = publisher.CreateClient();
            var subOptions = subscriber.BuildClientOptions();
            var subClient = subscriber.CreateClient();
            subClient = subscriber.ConnectHandler(subClient);

            //Act
            await brokerserver.StartAsync(brokerOptions.Build());
            await subClient.ConnectAsync(subOptions.Build());
            await pubClient.ConnectAsync(pubOptions.Build());
            
            var message = publisher.SimulatePublish(pubClient, "Wipro Tanmay");
            
            await subClient.SubscribeAsync(subscriber.BuildSubscribeOptions("test"));
            var msg1 = subClient.UseApplicationMessageReceivedHandler(e => { receivedMessage = e.ApplicationMessage; });
            
            await brokerserver.StopAsync();

            //Assert
            Assert.NotNull(message);
            Assert.Equal("test", message.Topic);
            Assert.Equal(MqttQualityOfServiceLevel.ExactlyOnce, message.QualityOfServiceLevel);
            Assert.Equal("Wipro Tanmay", Encoding.UTF8.GetString(message.Payload));

        }

    }
}




//await Task.Delay(10);