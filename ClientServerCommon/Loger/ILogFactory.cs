namespace CodeWriter.Logging
{
    public interface ILogFactory
    {
        ILog GetLogger(string name);
    }

    sealed class EmptyLogFactory : ILogFactory
    {
        private static readonly EmptyLog m_emptyLog = new EmptyLog();

        public ILog GetLogger(string name)
        {
            return m_emptyLog;
        }
    }
}