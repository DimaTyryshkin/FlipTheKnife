using System;

namespace ToonKnife.Server
{
    internal class ToonKnifeUserEventArgs : EventArgs
    {
        public ToonKnifeUser Client { get; private set; }

        public ToonKnifeUserEventArgs(ToonKnifeUser client)
        {
            Client = client;
        }
    }
}