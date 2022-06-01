# MqttDemo
- After running Mqtt server.
- Running serval Mqtt client.
- subscribe topic.
- send message to who already subscribed the topic.

## Mqtt Server
- MqttServerConnectionValidatorDelegate
- - Connectting sever according clientId, username and password.
- MqttServerOptions
- - Initial Mqtt server oiptions
- -- ConnectionValidator
- -- DefaultEndpointOptions
- - CreateMqttServer
- -- ClientSubscribedTopicHandler
- -- ClientUnsubscribedTopicHandler
- -- UseApplicationMessageReceivedHandler
- -- StartAsync
- -- UseClientConnectedHandler
- -- UseClientDisconnectedHandler

## Mqtt Client
- CreateMqttClient
- MqttClientOptionsBuilder
- WithTcpServer
- WithCredentials
- WithClientId
- IMqttClientOptions
- UseApplicationMessageReceivedHandler
- UseDisconnectedHandler
- ConnectAsync
- SendMessage
- SubscribeAsync
- UnsubscribeAsync
- PublishAsync
