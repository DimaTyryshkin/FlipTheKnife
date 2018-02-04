using System;

using Hik.Communication.Scs.Client;
using ScsService.Common;
using ScsService.Common.Authentication;

namespace ScsService.Client.Authentication
{
    public class UserAuthenticationState
    {
        public enum AuthenticationState
        {
            Processing = 0,
            Success = 1,
            Fail = 2
        }
         
        AuthenticationState _state = AuthenticationState.Processing;
        IScsClient scsClient;

        //---prop

        public AuthenticationState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
            }
        }


        //--methods

        public UserAuthenticationState(IScsClient client, string login)
        {
            scsClient = client;
             
            client.Disconnected += Client_Disconnected;

            Console.WriteLine("Send AuthenticationMessage "+ login);
            client.SendMessage(new AuthenticationMessage(login));
        }

        public void Stem1_MsgReader(ReceivedMsg msg, AuthenticationSuccesMessage authenticationMessage)
        {
            if (State == AuthenticationState.Processing)
            {
                State = AuthenticationState.Success;
            }

            Stop();
        }

        public void Stop()
        { 
            scsClient.Disconnected -= Client_Disconnected;
        }

        void Client_Disconnected(object sender, EventArgs e)
        {
            Stop();
            State = AuthenticationState.Fail;
        }

    }
}