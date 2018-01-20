using System;
using Hik.Communication.Scs.Server;
using ScsService.Common;
using ScsService.Common.Authentication;

namespace ScsService.Server.Authentication
{
    public class UserAuthenticationState
    {
        public enum AuthenticationState
        {
            Processing = 0,
            Success = 1,
            Fail = 2
        }

        TimeSpan _authenticationTimeOut = TimeSpan.FromSeconds(30);
        DateTime _authenticationMaxTime;
        User _user;
        MsgReadersCollection _rpcList;
        AuthenticationState _state = AuthenticationState.Processing;

        //---prop

        public AuthenticationState State
        {
            get
            {
                if (DateTime.Now > _authenticationMaxTime)
                    _state = AuthenticationState.Fail;

                return _state;
            }
            private set
            {
                // Нельзя имзенить состояние если уже закончился процесс аутентификации
                if (_state == AuthenticationState.Processing)
                {
                    if (DateTime.Now < _authenticationMaxTime)
                    {
                        _state = value;
                    }
                    else
                    {
                        _state = AuthenticationState.Fail;
                    }
                }
            }
        }

        public IScsServerClient ScsClient { get; private set; }

        public User User
        {
            get
            {
                if (State != AuthenticationState.Success)
                    throw new InvalidOperationException("Клиент еще не прошел аутентификацию");

                return _user;
            }
            private set { _user = value; }
        }

        //--methods
        
        public UserAuthenticationState(IScsServerClient client)
        {
            _authenticationMaxTime = DateTime.Now + _authenticationTimeOut;

            ScsClient = client;

            _rpcList = new MsgReadersCollection();
            _rpcList.RegisterMsgReader(typeof(AuthenticationMessage), Stem1_MsgReader);

            client.MessageReceived += _rpcList.AsyncClient_MessageReceivedHendler;
            client.Disconnected += Client_Disconnected;
        }

        public void Stop()
        {
            ScsClient.MessageReceived -= _rpcList.AsyncClient_MessageReceivedHendler;
            ScsClient.Disconnected -= Client_Disconnected;
        }
         
        private void Client_Disconnected(object sender, EventArgs e)
        {
            Stop();
            State = AuthenticationState.Fail;
        }

        void Stem1_MsgReader(ReceivedMsg msg)
        {
            if (State == AuthenticationState.Processing)
            {
                var m = (AuthenticationMessage)msg.Msg;
                IScsServerClient clietn = (IScsServerClient)msg.Sender;

                User = new User(m.login, clietn);
                State = AuthenticationState.Success;

                if (State == AuthenticationState.Success)
                {
                    //TODO send to clietn
                }
            }

            Stop();
        }
    }
}