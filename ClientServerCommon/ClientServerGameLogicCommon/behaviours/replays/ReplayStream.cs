using Assets.game.model.knife;
using System;
using System.Runtime.InteropServices;

namespace Assets.game.logic.playground.common.behaviours.replays
{
    public interface IReplayReader
    {
        bool PeekNext(out float time, out ReplayCommandCode code);
        bool ReadNext(out float time, out ReplayCommandCode code);

        bool ReadThrow(out float force);
        bool ReadThrowDebug(out float force, out float startRotation);
        bool ReadRestart();
        bool ReadChangeMode(out KnifeMode knifeMode);
    }

    public interface IReplayWriter
    {
        void WriteThrow(float time, float force);
        void WriteThrowDebug(float time, float force, float startRotation);
        void WriteRestart(float time);
        void WriteChangeMode(float time, KnifeMode knifeMode);
    }

    public class ReplayStreamReader : IReplayReader
    {
        private byte[] m_data;
        private int m_position;

        public byte[] data
        {
            get { return m_data; }
        }

        public ReplayStreamReader(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            m_data = data;
            m_position = 0;
        }

        public bool PeekNext(out float time, out ReplayCommandCode code)
        {
            if (m_position + 4 + 1 > m_data.Length)
            {
                time = 0f;
                code = ReplayCommandCode.None;
                return false;
            }

            int tempPosition = m_position;
            ReadSingle(m_data, ref tempPosition, out time);
            code = (ReplayCommandCode)m_data[tempPosition++];
            return true;
        }

        public bool ReadNext(out float time, out ReplayCommandCode code)
        {
            if (m_position + 4 + 1 > m_data.Length)
            {
                time = 0f;
                code = ReplayCommandCode.None;
                return false;
            }

            ReadSingle(m_data, ref m_position, out time);
            code = (ReplayCommandCode)m_data[m_position++];
            return true;
        }

        public bool ReadThrow(out float force)
        {
            if (m_position + 4 > m_data.Length)
            {
                force = 0f;
                return false;
            }

            ReadSingle(m_data, ref m_position, out force);
            return true;
        }

        public bool ReadThrowDebug(out float force, out float startRotation)
        {
            if (m_position + 4  + 4> m_data.Length)
            {
                force = 0f;
                startRotation = 0f;
                return false;
            }

            ReadSingle(m_data, ref m_position, out force);
            ReadSingle(m_data, ref m_position, out startRotation);
            return true;
        }

        public bool ReadRestart()
        {
            if (m_position > m_data.Length)
            {
                return false;
            }

            return true;
        }

        public bool ReadChangeMode(out KnifeMode knifeMode)
        {
            if (m_position + 1 > m_data.Length)
            {
                knifeMode = KnifeMode.Medium;
                return false;
            }

            knifeMode = (KnifeMode)m_data[m_position++];
            return true;
        }

        private static void ReadSingle(byte[] data, ref int position, out float value)
        {
            ConvertStruct convert;
            convert.f = 0;
            convert.b1 = data[position++];
            convert.b2 = data[position++];
            convert.b3 = data[position++];
            convert.b4 = data[position++];
            value = convert.f;
        }
    }

    public class ReplayStreamWriter : IReplayWriter
    {
        private byte[] m_data;
        private int m_position;

        public byte[] data
        {
            get { return m_data; }
        }

        public ReplayStreamWriter() : this(null)
        { }

        private ReplayStreamWriter(byte[] data)
        {
            m_data = data;
            m_position = 0;
        }

        public void WriteThrow(float time, float force)
        {
            EnsureBufferSize(m_position + 4 + 1 + 4);

            WriteSingle(m_data, ref m_position, time);
            m_data[m_position++] = (byte)ReplayCommandCode.Throw;
            WriteSingle(m_data, ref m_position, force);
        }

        public void WriteThrowDebug(float time, float force, float startRotation)
        {
            EnsureBufferSize(m_position + 4 + 1 + 4 + 4);

            WriteSingle(m_data, ref m_position, time);
            m_data[m_position++] = (byte)ReplayCommandCode.Throw;
            WriteSingle(m_data, ref m_position, force);
            WriteSingle(m_data, ref m_position, startRotation);
        }

        public void WriteRestart(float time)
        {
            EnsureBufferSize(m_position + 4 + 1);

            WriteSingle(m_data, ref m_position, time);
            m_data[m_position++] = (byte)ReplayCommandCode.Restart;
        }

        public void WriteChangeMode(float time, KnifeMode knifeMode)
        {
            EnsureBufferSize(m_position + 4 + 1 + 1);

            WriteSingle(m_data, ref m_position, time);
            m_data[m_position++] = (byte)ReplayCommandCode.ChangeMode;
            m_data[m_position++] = (byte)knifeMode;
        }

        private void EnsureBufferSize(int numberOfBytes)
        {
            if (m_data == null)
            {
                m_data = new byte[numberOfBytes];
                return;
            }
            if (m_data.Length < numberOfBytes)
                Array.Resize<byte>(ref m_data, numberOfBytes);
        }

        private static void WriteSingle(byte[] data, ref int position, float value)
        {
            ConvertStruct convert;
            convert.b1 = 0;
            convert.b2 = 0;
            convert.b3 = 0;
            convert.b4 = 0;
            convert.f = value;

            data[position++] = convert.b1;
            data[position++] = convert.b2;
            data[position++] = convert.b3;
            data[position++] = convert.b4;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ConvertStruct
    {
        [FieldOffset(0)]
        public byte b1;

        [FieldOffset(1)]
        public byte b2;

        [FieldOffset(2)]
        public byte b3;

        [FieldOffset(3)]
        public byte b4;

        [FieldOffset(0)]
        public float f;
    }
}