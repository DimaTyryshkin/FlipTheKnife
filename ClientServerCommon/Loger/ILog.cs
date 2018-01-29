namespace CodeWriter.Logging
{
    public interface ILog
    {
        bool isDebugEnabled { get; }
        bool isWarnEnabled { get; }
        bool isErrorEnabled { get; }

        void Debug(object obj);
        void DebugFormat(string format, params object[] args);

        void Warn(object obj);
        void WarnFormat(string format, params object[] args);

        void Error(object obj);
        void ErrorFormat(string format, params object[] args);
    }

    sealed class EmptyLog : ILog
    {
        public bool isDebugEnabled
        {
            get { return false; }
        }

        public bool isErrorEnabled
        {
            get { return false; }
        }

        public bool isWarnEnabled
        {
            get { return false; }
        }

        public void Debug(object obj)
        {

        }

        public void DebugFormat(string format, params object[] args)
        {

        }

        public void Error(object obj)
        {

        }

        public void ErrorFormat(string format, params object[] args)
        {

        }

        public void Warn(object obj)
        {

        }

        public void WarnFormat(string format, params object[] args)
        {

        }
    }
}