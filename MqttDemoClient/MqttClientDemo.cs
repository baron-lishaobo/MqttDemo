using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttDemoClient
{
    internal class MqttClientDemo
    {
        public static IMqttClient? mqttClient { get; set; }
        #region Mqtt Client
        public static void StartClient()
        {
            ConnectServer();
            SendMessage();
        }
        public static void ConnectServer()
        {
            try
            {
                mqttClient = new MqttFactory().CreateMqttClient();
                MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder();
                optionsBuilder.WithTcpServer("127.0.0.1", 890);
                optionsBuilder.WithCredentials("baron", "123456");
                optionsBuilder.WithClientId(Guid.NewGuid().ToString());
                IMqttClientOptions options = optionsBuilder.Build();
                mqttClient.UseApplicationMessageReceivedHandler(ClientMeaageReceived);
                mqttClient.UseDisconnectedHandler(async arg =>
                {
                    Console.WriteLine("The connection to the server was lost, Trying to reconnec");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try
                    {
                        await mqttClient.ConnectAsync(options);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Reconnection to the server failed: {ex}");
                    }
                });
                mqttClient.ConnectAsync(options);
                Console.WriteLine("Successfully connected to the sever! Please enter any content and press enter the menu interface");
            }
            catch (Exception ex)
            {

                Console.WriteLine($" Faild to connect to server:{ex}");
            }
        }


        private static void ClientMeaageReceived(MqttApplicationMessageReceivedEventArgs arg)
        {
            Console.WriteLine("### Recevied a message from the server ###");
            string topic = arg.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            var qos = arg.ApplicationMessage.QualityOfServiceLevel;
            bool retain = arg.ApplicationMessage.Retain;
            Console.WriteLine($"The theme: [{topic}] >> Content: [{payload}] >> Qos: [{qos}] >> Retain: [{retain}]");

        }

        public static void SendMessage()
        {
            Console.ReadLine();
            bool isExit = false;
            while (!isExit)
            {
                Console.WriteLine(@"Please enter
                                    1. Subscribe to topics
                                    2. Unsubscribe
                                    3. Send a message
                                    4. Sign out
                                    ");
                string? input = Console.ReadLine();
                string? topic = "";
                switch (input)
                {
                    case "1":
                        Console.WriteLine(@"Please enter a topic name");
                        topic = Console.ReadLine();
                        ClientSubscribeTopic(topic);
                        break;
                    case "2":
                        Console.WriteLine(@"Please enter the subject name to unsubscribe from:");
                        topic = Console.ReadLine();
                        ClientUnsubscribeTopic(topic);
                        break;
                    case "3":
                        Console.WriteLine("@Please enter the subject name to be sent:");
                        topic = Console.ReadLine();
                        Console.WriteLine(@"Please enter the content of the message to be sent:");
                        var message = Console.ReadLine();
                        ClientPublish(topic, message);
                        break;
                    case "4":
                        isExit = true;
                        break;
                    default:
                        Console.WriteLine("Please enter correct command");
                        break;
                }
            }
        }
        private static async void ClientSubscribeTopic(string topic)
        {
            topic = topic.Trim();
            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("The subscription topic cannot be empty!");
                return;
            }
            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("MQTT the client is not connected yet!");
                return;
            }
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build();
            await mqttClient.SubscribeAsync(subscribeOptions, System.Threading.CancellationToken.None);
        }
        private static async void ClientUnsubscribeTopic(string topic)
        {
            topic = topic.Trim();
            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("Unsubscribe subject canne be empty!"); ;
                return;
            }
            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("MQTT The client is not connected yet!");
                return;
            }
            var subscribeOptions = new MqttClientUnsubscribeOptionsBuilder().WithTopicFilter(topic).Build();
            await mqttClient.UnsubscribeAsync(subscribeOptions, CancellationToken.None);
        }
        private async static void ClientPublish(string topic, string message)
        {
            topic = topic.Trim();
            message = message.Trim();
            if (string.IsNullOrEmpty(topic))
            {
                Console.WriteLine("Unsubscribe subject cannot be emoty!");
                return;
            }
            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("MQTT The client is not connected yet!");
                return;
            }
            var applicationMessage = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(message).WithExactlyOnceQoS().WithRetainFlag().Build();
            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }
        #endregion
    }
}
