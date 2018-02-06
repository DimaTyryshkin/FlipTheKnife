using System;

namespace ToonKnife.Server
{
    public class ToonKnifeUserEventArgs : EventArgs
    {
        public ToonKnifeUser Client { get; private set; }

        public ToonKnifeUserEventArgs(ToonKnifeUser client)
        {
            Client = client;
        }
    }
}