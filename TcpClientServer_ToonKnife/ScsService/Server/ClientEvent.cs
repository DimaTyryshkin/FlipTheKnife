using Hik.Communication.Scs.Server;


namespace ScsService.Server
{
    public class ClientEvent
    {
        public enum Event
        {
            Connected = 0,
            Disconnected = 1,
        }

        /// <summary>
        /// Кто прислал
        /// </summary>
        public IScsServerClient Client { get; private set; }
         
         
        public Event EventType { get; private set; }

        public ClientEvent(IScsServerClient sender, Event eventType)
        {
            Client = sender;
            EventType = eventType;
        }
    }
}