using System;

namespace CodeWriter.Logging
{
    public static class LogManager
    {
        private static ILogFactory m_factory = new EmptyLogFactory();

        public static ILogFactory factory
        {
            get { return m_factory; }
            set { m_factory = value ?? new EmptyLogFactory(); }
        }

        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.Name);
        }

        public static ILog GetLogger(string name)
        {
            return factory.GetLogger(name);
        }
    }
}