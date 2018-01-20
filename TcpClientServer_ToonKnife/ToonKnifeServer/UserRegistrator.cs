using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Hik.Communication.Scs.Server;
using Hik.Communication.Scs.Communication.Messages;
using ScsServiceInfrastructure;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;

using ToonKnife.Common.ScsMessages;

namespace ToonKnife.Server
{
    internal class UserRegistrator
    {
        struct ClientInfo
        {
            public IScsServerClient client;
            public AuthenticationMessage authenticationMessage;
            public bool waitRemove;
            public bool waitAuthentication;
        }

        int clientMaxCount = 8192; //16384;

        IScsServer server;
        MsgReadersCollection readersCollection;
        Dictionary<IScsServerClient, ToonKnifeUser> users;

        /// <summary>
        /// Клиент УЖЕ удален и отключен. Коммуникация с ним уже не возможна.
        /// </summary>
        public event EventHandler<ToonKnifeUserEventArgs> OnClientDisconnected;

        /// <summary>
        /// Клиент полностью готов к работе.
        /// </summary>
        public event EventHandler<ToonKnifeUserEventArgs> OnClientLogin;

        ConcurrentDictionary<IScsServerClient, ClientInfo> clientsWaitMainLoop;

        public UserRegistrator(int tcpPort)
        {
            clientsWaitMainLoop = new ConcurrentDictionary<IScsServerClient, ClientInfo>();
            users = new Dictionary<IScsServerClient, ToonKnifeUser>(clientMaxCount);

            readersCollection = new MsgReadersCollection();
            readersCollection.RegisterMsgReader(typeof(AuthenticationMessage), AuthenticationMsgReader);

            server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(tcpPort));

            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;

            server.Start();
        }

        public void MainLoopFrame()
        {
            // В главном потеке синхронно добавляем\удаялем
            // клинетов, которые подклоючились\отквалились

            foreach (var scsClient in clientsWaitMainLoop.Keys)
            {
                ClientInfo tempClient;
                if (clientsWaitMainLoop.TryGetValue(scsClient, out tempClient))
                {
                    //Обязательно сначала удаляем(хотя он все равно может асинхронно отрубиться)
                    if (tempClient.waitRemove)
                    {
                        if (users.ContainsKey(tempClient.client))
                        {
                            var removedClietn = users[tempClient.client];
                            users.Remove(tempClient.client);

                            //Бросаем событие уже после удаления из спаска. Все равно ему уже ничего не послать.
                            OnClientDisconnected(this, new ToonKnifeUserEventArgs(removedClietn));
                        }
                    }
                    else if (tempClient.waitAuthentication)
                    {
                        var m = tempClient.authenticationMessage;

                        //TODO authentication
                        //Елси она будлет много шаговой, то надо переносить в новый список.
                        // И там тоже следить за его состоянием. В честнотси удалять его от туда
                        //если он досрочно отвалится, вроде это легоко сделать просто вместе с другим удалением синхронным
                        //протсо второй список надо будет проверить

                        //Больше он не висит на регистраторе.
                        tempClient.client.MessageReceived -= readersCollection.MessageReceived_Handler;

                        ToonKnifeUser newUser = new ToonKnifeUser();
                        users.Add(tempClient.client, newUser);

                        OnClientLogin(this, new ToonKnifeUserEventArgs(newUser));
                    }

                    //Обработанного клиунт аудаляем из списка не обработанных
                    ClientInfo clientDeleted;
                    clientsWaitMainLoop.TryRemove(scsClient, out clientDeleted);
                }
            }
        }

        public void Stop()
        {
            server.Stop();
        }

        void Server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A new client is connected. Client Id = " + e.Client.ClientId);

            if (users.Count < clientMaxCount)
            {
                e.Client.MessageReceived += readersCollection.MessageReceived_Handler;
            }
            else
            {
                e.Client.SendMessage(
                    new ErrorMessage(ErrorMessage.Error.ToManyClientOnServer, null, null));

                e.Client.Disconnect();
            }
        }

        void AuthenticationMsgReader(IScsServerClient client, IScsMessage msg)
        {
            var m = (AuthenticationMessage)msg;

            var newClietnInfo = new ClientInfo()
            {
                authenticationMessage = m,
                client = client,
                waitAuthentication = true
            };

            //запихиваем в список на прохождение аунеттификации в слудующем кадре
            clientsWaitMainLoop.TryAdd(client, newClietnInfo);
        }

        void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            var newClietnInfo = new ClientInfo()
            {
                client = e.Client,
                waitAuthentication = false
            };

            //запихиваем в список на удаление в слудующем кадре
            clientsWaitMainLoop.AddOrUpdate(e.Client, newClietnInfo,
               (scsClietn, imfo) =>
               {
                   imfo.waitRemove = true;
                   imfo.waitAuthentication = false;
                   return imfo;
               });

        }
    }
}