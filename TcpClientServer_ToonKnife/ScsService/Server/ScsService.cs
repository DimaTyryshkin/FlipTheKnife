using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Hik.Communication.Scs.Server;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;

using ScsService.Server.Authentication;
using ScsService.Common.ScsMessages;

namespace ScsService.Server
{
    public class ScsService
    {
        int _scsClientMaxCount = 8192; //16384;
        int _scsClientCount;
         
        IScsServer _server;
        ConcurrentQueue<ReceivedClientEvent> _eventQueue;
        LinkedList<UserAuthenticationState> _authenticationStates;
        Dictionary<IScsServerClient, User> _authenticatedUsers;

        //---events

        /// <summary>
        /// Юзер УЖЕ удален и отключен. Коммуникация с ним уже не возможна.
        /// </summary>
        public event EventHandler<UserEventArgs> OnUserDisconnected;

        /// <summary>
        /// Юзер полностью готов к работе.
        /// </summary>
        public event EventHandler<UserEventArgs> OnUserLogin;



        //---methods

        public ScsService(int tcpPort, int scsClientMaxCount= 8192)
        {
            this._scsClientMaxCount = scsClientMaxCount;

            // Список по сути особен ну нужен, но используется 
            // чтобы отделять клиентов, котоыре еще не прошли регистраци от тех кто уже прошел,
            // чтобы не пропускать не нужные событи вверх к основному приложению.
            _authenticatedUsers = new Dictionary<IScsServerClient, User>();

            //Синхронная очередь клиентских событий 
            _eventQueue = new ConcurrentQueue<ReceivedClientEvent>();

            // Проходящите подключение клиенты
            _authenticationStates = new LinkedList<UserAuthenticationState>();

            // Scs server(tcp слой)
            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(tcpPort));
            _server.ClientConnected += Server_ClientConnected;
            _server.ClientDisconnected += Server_ClientDisconnected;
            _server.Start();
        }

        public void MainLoopFrame()
        {
            //---В главном потоке спокойно читаем события подклюбчения\отключения

            foreach (var reseivedEvent in _eventQueue.ToArray())
            {
                // Disconnect
                if (reseivedEvent.EventType == ReceivedClientEvent.Event.Disconnected)
                {
                    var client = reseivedEvent.Client;
                    if (_authenticatedUsers.ContainsKey(client))
                    {
                        var disconectedUser = _authenticatedUsers[client];
                        _authenticatedUsers.Remove(client);

                        //Бросаем событие уже после удаления из спаска. Все равно ему уже ничего не послать.
                        OnUserDisconnected(this, new UserEventArgs(disconectedUser));
                    }
                }

                // Connect
                if (reseivedEvent.EventType == ReceivedClientEvent.Event.Connected)
                {
                    _scsClientCount++;
                    _authenticationStates.AddLast(new UserAuthenticationState(reseivedEvent.Client));
                }
            }

            LinkedListNode<UserAuthenticationState> stateNode = _authenticationStates.First;
            while (stateNode != null)
            {
                UserAuthenticationState userAuth = stateNode.Value;
                if (userAuth.State == UserAuthenticationState.AuthenticationState.Success)
                {
                    ////TODO тут типа из бызы надо грузить 
                    //ToonKnifeUser newUser = new ToonKnifeUser();
                    //users.Add(userAuth.ScsClient, newUser);

                    _authenticatedUsers.Add(userAuth.ScsClient, userAuth.User);
                    OnUserLogin(this, new UserEventArgs(userAuth.User));

                    stateNode = stateNode.Next;
                }
                else if (userAuth.State == UserAuthenticationState.AuthenticationState.Fail)
                {
                    // Удаляем этого клиента и его аутентификатор
                    var nodeToRemove = stateNode;
                    stateNode = nodeToRemove.Next;

                    nodeToRemove.Value.Stop();
                    _authenticationStates.Remove(nodeToRemove);
                }
            }
        }

        public void Stop()
        {
            _server.Stop();
        }

        //---hendlers

        void Server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A new client is connected. Client Id = " + e.Client.ClientId);

            if (_scsClientCount < _scsClientMaxCount)
            {
                _eventQueue.Enqueue(new ReceivedClientEvent(e.Client, ReceivedClientEvent.Event.Connected));
            }
            else
            {
                e.Client.SendMessage(
                    new ErrorMessage(ErrorMessage.Error.ToManyClientOnServer, null, null));

                e.Client.Disconnect();
            }
        }

        void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            _eventQueue.Enqueue(new ReceivedClientEvent(e.Client, ReceivedClientEvent.Event.Disconnected));
        }
    }
}