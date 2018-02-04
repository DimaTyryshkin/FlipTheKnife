using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Hik.Communication.Scs.Server; 
using Hik.Communication.Scs.Communication.EndPoints.Tcp;

using ScsService.Server.Authentication;
using ScsService.Common.ScsMessages;
using ScsService.Common.Authentication;
using ScsService.Common;

namespace ScsService.Server
{
    public class ScsService
    {
        int _scsClientMaxCount; // 8192; //16384;
        int _scsClientCount;

        IScsServer _server;
        ConcurrentMsgQueue _msgQueueForAutentification;
        ConcurrentMsgQueue _msgQueue;
        Dictionary<IScsServerClient, UserAuthenticationState> _authenticationStates;
        Dictionary<IScsServerClient, User> _authenticatedUsers;
        MsgReadersCollection _msgReaders;
        MsgReadersCollection _msgReadersForAutentification;

        //---events

        /// <summary>
        /// Юзер УЖЕ удален и отключен. Коммуникация с ним уже не возможна.
        /// </summary>
        public event EventHandler<UserEventArgs> OnUserDisconnected;

        /// <summary>
        /// Юзер полностью готов к работе.
        /// </summary>
        public event EventHandler<UserEventArgs> OnUserLogin;

        //--- prop
        public MsgReadersCollection MsgReaders
        {
            get { return _msgReadersForAutentification; }
        }


        //---methods

        public ScsService(int tcpPort, int scsClientMaxCount = 8192)
        {
            _scsClientMaxCount = scsClientMaxCount;

            // Список по сути особен ну нужен, но используется 
            // чтобы отделять клиентов, котоыре еще не прошли регистраци от тех кто уже прошел,
            // чтобы не пропускать не нужные событи вверх к основному приложению.
            _authenticatedUsers = new Dictionary<IScsServerClient, User>();

            //Синхронная очередь клиентских msg для тех кто еще не прошел аутентификацию
            _msgReadersForAutentification = new MsgReadersCollection();
            _msgReadersForAutentification.RegisterMsgReader<AuthenticationMessage>(AuthenticationMsgReader);
            _msgQueueForAutentification = new ConcurrentMsgQueue(_msgReadersForAutentification);
            _msgQueueForAutentification.ClientEventReaded += MsgQueue_ClientEventReaded;

            //Синхронная очередь клиентских msg
            _msgReaders = new MsgReadersCollection();
            _msgQueue = new ConcurrentMsgQueue(_msgReaders);
            _msgQueue.ClientEventReaded += MsgQueue_ClientEventReaded;

            // Проходящите подключение клиенты
            _authenticationStates = new Dictionary<IScsServerClient, UserAuthenticationState>();

            // Scs server(tcp слой)
            _server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(tcpPort));
            _server.ClientConnected += Server_ClientConnected;
            _server.ClientDisconnected += Server_ClientDisconnected;
        }


        public void Start()
        {
            _server.Start();
        }

        public void MainLoopFrame()
        {
            //---В главном потоке спокойно читаем события подклюбчения\отключения

            _msgQueueForAutentification.ReadStoredMsg();
            //_msgQueue.ReadStoredMsg(_msgQueue.Count);


            //TODO оптимизировать без копирования в каждом кадре
            // Тут отключаем тех кто затянул с аутентийикацией
            var authenticationStates = _authenticationStates.Values.ToArray();
            foreach (UserAuthenticationState userAuth in authenticationStates)
            {
                if (userAuth.State == UserAuthenticationState.AuthenticationState.Fail)
                {
                    // Удаляем этого клиента и его аутентификатор
                    userAuth.Stop();
                    _authenticationStates.Remove(userAuth.ScsClient);
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
            Console.WriteLine(GetType().Name + " :New tcp client Id = " + e.Client.ClientId);

            if (_scsClientCount < _scsClientMaxCount)
            {
                _msgQueueForAutentification.EnqueueEvent(new ClientEvent(e.Client, ClientEvent.Event.Connected));

                //Прямо сраз уначинаем слушать! иначе можно терять сообщения
                _msgQueueForAutentification.AddMessenger(e.Client);
            }
            else
            {
                //TODO клиент не сможет ничего принять, он не успеет создать очередь сообщений
                e.Client.SendMessage(
                    new ErrorMessage(ErrorMessage.Error.ToManyClientOnServer, null, null));

                e.Client.Disconnect();
            }
        }

        void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine(GetType().Name + " :A client is Disconnected. Client Id = " + e.Client.ClientId);

            _msgQueueForAutentification.EnqueueEvent(new ClientEvent(e.Client, ClientEvent.Event.Disconnected));
        }

        private void MsgQueue_ClientEventReaded(object sender, ClientEvent e)
        {
            if (e.EventType == ClientEvent.Event.Connected)
            {
                Console.WriteLine(GetType().Name + " :Start authentication Id = " + e.Client.ClientId);
                _scsClientCount++;
                _authenticationStates.Add(e.Client, new UserAuthenticationState(e.Client));
            }


            // Disconnect
            if (e.EventType == ClientEvent.Event.Disconnected)
            {
                var client = e.Client;
                if (_authenticatedUsers.ContainsKey(client))
                {
                    var disconectedUser = _authenticatedUsers[client];


                    //_msgQueue.RemoveMessenger(e.Client);

                    //Бросаем событие уже после удаления из спаска. Все равно ему уже ничего не послать.
                    OnUserDisconnected?.Invoke(this, new UserEventArgs(disconectedUser));
                }

                _authenticatedUsers.Remove(client);
                _msgQueueForAutentification.RemoveMessenger(e.Client);
            }
        }

        //---msg readers

        private void AuthenticationMsgReader(ReceivedMsg receivedMsg, AuthenticationMessage msg)
        {
            UserAuthenticationState autState;
            if (_authenticationStates.TryGetValue((IScsServerClient)receivedMsg.Sender, out autState))
            {
                autState.Step1_MsgReader(receivedMsg, msg);

                if (autState.State == UserAuthenticationState.AuthenticationState.Success)
                {
                    // Удаляем аутентификатор 
                    _authenticationStates.Remove(autState.ScsClient);

                    //Сначала добавляем потом удаляем, чтобы не бьыло ни наносекунды, когда никто не слушает клиента
                    //_msgQueue.AddMessenger(autState.ScsClient);
                    //_msgQueueForAutentification.RemoveMessenger(autState.ScsClient);

                    _authenticatedUsers.Add(autState.ScsClient, autState.User);
                    autState.ScsClient.SendMessage(new AuthenticationSuccesMessage());


                    OnUserLogin?.Invoke(this, new UserEventArgs(autState.User));
                }
            }
            else
            {
                Console.WriteLine(GetType().Name +" :AuthenticationMsgReader error login="+ msg.login);   
            }
        }
    }
}