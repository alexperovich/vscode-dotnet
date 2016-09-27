using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VSCode.JsonRpc
{
    internal class StreamMessageReader : IMessageReader
    {
        private const int _BufferSize = 1024;
        private const string _HeaderContentLength = "Content-Length";

        internal StreamMessageReader(Stream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        internal StreamMessageReader(Stream stream, Encoding encoding)
        {
            Enumerator = ReadMessages(stream, encoding);
        }

        internal IEnumerator<IMessage> Enumerator { get; }

        public void Dispose()
        {
            Enumerator?.Dispose();
        }

        private static IEnumerator<IMessage> ReadMessages(Stream stream, Encoding encoding)
        {
            var messageBuffer = new MessageBuffer();
            var charBuffer = new char[_BufferSize];
            var reader = new StreamReader(stream, encoding);
            while (true)
            {
                int read = reader.Read(charBuffer, 0, charBuffer.Length);
                if (read == 0)
                {
                    yield break;
                }
                messageBuffer.Append(new string(charBuffer, 0, read));
                IMessage message;
                while (messageBuffer.TryReadMessage(out message))
                {
                    yield return message;
                }
            }
        }

        public IMessage Read()
        {
            if (!Enumerator.MoveNext())
            {
                throw new InvalidOperationException();
            }
            return Enumerator.Current;
        }
    }
}
