using Hik.Communication.Scs.Client;


namespace ScsService.Client
{
    public struct ClientEvent
    {
        public enum Event
        {
            Connected = 0,
            Disconnected = 1,
        }

        public Event EventType { get; private set; }

        public ClientEvent( Event eventType)
        { 
            EventType = eventType;
        }
    }
}