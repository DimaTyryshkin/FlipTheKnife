using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messengers;
using ScsService.Client.Authentication;
using ScsService.Common;
using ScsService.Common.Authentication;
using System;
using System.Collections.Generic;


namespace ScsService.Client
{
    public class ScsClient
    {
        IScsClient _client;
        ConcurrentEventQueue _eventQueue;
        ConcurrentMsgQueue _msgQueue;
        MsgReadersCollection _msgReaders;
        UserAuthenticationState _authenticationState;
        string _userName;

        /// <summary>
        /// Сервер отключен. Коммуникация с ним уже не возможна.
        /// </summary>
        public event EventHandler OnUserDisconnected;

        /// <summary>
        /// Сервер полностью готов к работе. Аутентификайия уже прошла.
        /// </summary>
        public event EventHandler OnUserLogin;


        //--- prop
        public IMessenger Messenger
        {
            get { return _client; }
        }

        public MsgReadersCollection MsgReaders
        {
            get { return _msgReaders; }
        }

        public bool IsConnected { get; private set; }

        //---methods

        public ScsClient(string userName, string ip, int tcpPort)
        {
            _userName = userName;

            //Синхронная очередь клиентских событий 
            _eventQueue = new ConcurrentEventQueue();

            _msgReaders = new MsgReadersCollection();
            _msgReaders.RegisterMsgReader<AuthenticationSuccesMessage>(AuthenticationSuccessMsgReader);

            //Синхронная очередь msg с сервера
            _msgQueue = new ConcurrentMsgQueue(_msgReaders);

            _client = ScsClientFactory.CreateClient(new ScsTcpEndPoint(ip, tcpPort));
            _client.Connected += Client_Connected;
            _client.Disconnected += Client_Disconnected;

            _msgQueue.AddMessenger(_client);
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void MainLoopFrame()
        {
            //---В главном потоке спокойно читаем события подклюбчения\отключения

            int count = 0;
            while (_eventQueue.Count > 0 && count < 100)
            {
                count++;

                ClientEvent reseivedEvent = _eventQueue.Dequeue();
                // Disconnect
                if (reseivedEvent.EventType == ClientEvent.Event.Disconnected)
                {
                    if (IsConnected)
                    {
                        IsConnected = false;
                        if (OnUserDisconnected != null)
                            OnUserDisconnected(this, new EventArgs());
                    }
                }

                // Connect
                if (reseivedEvent.EventType == ClientEvent.Event.Connected)
                {
                    IsConnected = true;
                    _authenticationState = new UserAuthenticationState(_client, _userName);
                }
            }

            // аутентийикация
            if (_authenticationState != null)
            {
                if (_authenticationState.State == UserAuthenticationState.AuthenticationState.Fail)
                {
                    _authenticationState = null;
                }
            }

            _msgQueue.ReadStoredMsg();
        }

        public void Disconnect()
        {
            if (_client.CommunicationState == CommunicationStates.Connected)
                _client.Disconnect();
        }

        //---hendlers 
        private void Client_Disconnected(object sender, EventArgs e)
        {
            var clietnEvent = new ClientEvent(ClientEvent.Event.Disconnected);
            _eventQueue.EnqueueEvent(clietnEvent);
            _msgQueue.RemoveMessenger(_client);
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            var clietnEvent = new ClientEvent(ClientEvent.Event.Connected);
            _eventQueue.EnqueueEvent(clietnEvent);
        }

        //--- msg readers
        private void AuthenticationSuccessMsgReader(ReceivedMsg receivedMsg, AuthenticationSuccesMessage msg)
        {
            _authenticationState.Stem1_MsgReader(receivedMsg, msg);

            if (_authenticationState.State == UserAuthenticationState.AuthenticationState.Success)
            {
                _authenticationState = null;
                IsConnected = true;

                if (OnUserLogin != null)
                    OnUserLogin(this, new EventArgs());
            }
        }
    }
}