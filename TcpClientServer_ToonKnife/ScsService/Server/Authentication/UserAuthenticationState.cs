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
        AuthenticationState _state = AuthenticationState.Processing;

        //---prop

        public AuthenticationState State
        {
            get
            {
                if (_state == AuthenticationState.Processing)
                {
                    if (DateTime.Now > _authenticationMaxTime)
                        _state = AuthenticationState.Fail;
                }

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

            client.Disconnected += Client_Disconnected;
        }

        public void Step1_MsgReader(ReceivedMsg receivedMsg, AuthenticationMessage authenticationMessage)
        {
            if (receivedMsg.Sender == ScsClient)
            {
                Console.WriteLine(GetType().Name + " :Get AuthenticationMessage " + authenticationMessage.login);
                if (State == AuthenticationState.Processing)
                {
                    User = new User(authenticationMessage.login, ScsClient);
                    State = AuthenticationState.Success;

                    //Console.WriteLine("Authentication Success login=" + authenticationMessage.login);
                }

                Stop();
            }
        }

        public void Stop()
        {
            ScsClient.Disconnected -= Client_Disconnected;
        }

        void Client_Disconnected(object sender, EventArgs e)
        {
            Stop();
            State = AuthenticationState.Fail;
        }
    }
}
