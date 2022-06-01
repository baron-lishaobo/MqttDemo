using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttDemo
{
    internal class MqttDemoServer
    {
        public static IMqttServer? mqttServer { get; set; }
        #region Server
        public static async void StartServer()
        {
            try
            {
                MqttServerConnectionValidatorDelegate connectionValidatorDelegate = new MqttServerConnectionValidatorDelegate(p =>
                  {
                      if (p.ClientId=="twle_client")
                      {
                          if (p.Username!="baron"&&p.Password!="123456")
                          {
                              p.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                          }
                      }
                  });
                MqttServerOptions options = new MqttServerOptions();
                options.ConnectionValidator = connectionValidatorDelegate;
                options.DefaultEndpointOptions.Port = 890;
                 mqttServer = new MqttFactory().CreateMqttServer();
                mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(SubScribedTopic);
                mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(UnSubscribedTopic);
                mqttServer.UseApplicationMessageReceivedHandler(MessageReceived);
                await mqttServer.StartAsync(options);
                mqttServer.UseClientConnectedHandler(ClientConnected);
                mqttServer.UseClientDisconnectedHandler(ClientDisConnected);
                Console.WriteLine($"Mqtt Server started successfully! Press enter directly stop the server!");
                Console.ReadLine();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void ClientDisConnected(MqttServerClientDisconnectedEventArgs arg)
        {
            string clientId=arg.ClientId;
            Console.WriteLine($"Client [{clientId}] has left!");
        }

        private static void ClientConnected(MqttServerClientConnectedEventArgs arg)
        {
            string clinetId = arg.ClientId;
            Console.WriteLine($"New client [{clinetId}] join in!");
        }

        private static void MessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            string clientId = args.ClientId;
            string topic = args.ApplicationMessage.Topic;
            string payload=Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            var qos = args.ApplicationMessage.QualityOfServiceLevel;
            bool retain = args.ApplicationMessage.Retain;
            Console.WriteLine($" client [{clientId}] >> The theme: [{topic}] >> Content: [{payload}] >> Qos:[{qos}] >> Retain:[{retain}]");
        }

        private static void UnSubscribedTopic(MqttServerClientUnsubscribedTopicEventArgs args)
        {
            string clientId = args.ClientId;
            string topic = args.TopicFilter;
            Console.WriteLine($"client [{clientId}] Unsubscribed from topic: [{topic}]");
        }

        private static void SubScribedTopic(MqttServerClientSubscribedTopicEventArgs args)
        {
            string clientId = args.ClientId;
            string topic = args.TopicFilter.Topic;
            Console.WriteLine($"client [{clientId}] Subscribed topics : {topic}");
        }

        #endregion


    
    }
}
