using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;
using ScsService.Common;
using System;

namespace ScsService.Server
{
    public class User
    {
        MsgReadersCollection _msgReaders;
        IScsServerClient _scsServerClient;

        public string Login { get; private set; }


        //---events
        public event EventHandler<ClientEvent> Disconnected;

        //---prop

        public MsgReadersCollection MsgReaders { get => _msgReaders; }

        public IScsServerClient Client => _scsServerClient;

        //---methods
        public User(string login, IScsServerClient scsServerClient)
        {
            _scsServerClient = scsServerClient;
            Login = login;

            _msgReaders = new MsgReadersCollection();
        }

        public void SendMsg(IScsMessage message)
        {
            _scsServerClient.SendMessage(message);
        }

        internal void OnDisconnected()
        {
            Disconnected?.Invoke(this, new ClientEvent(_scsServerClient, ClientEvent.Event.Disconnected));
        }
    }
}