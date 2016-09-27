using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace VSCode.JsonRpc
{
    internal class MessageBuffer
    {
        private const string _ContentLength = "Content-Length";
        private const string _HeaderContentSeperator = "\r\n\r\n";
        private const string _HeaderDelimiter = "\r\n";
        private const char _HeaderKeyValueSeperator = ':';

        public MessageBuffer()
        {
            TextBuffer = new StringBuilder();
        }

        private StringBuilder TextBuffer { get; }

        private string Text => TextBuffer.ToString();

        public bool TryReadMessage(out IMessage message)
        {
            IDictionary<string, string> headers;
            int contentStartIndex;
            if (TryReadHeaders(out contentStartIndex, out headers))
            {
                int length = int.Parse(headers["Content-Length"]);
                string content;
                if (TryReadContent(contentStartIndex, length, out content))
                {
                    TextBuffer.Remove(0, contentStartIndex + length);
                    message = MessageSerializer.Deserialize(content);
                    return true;
                }
            }
            message = null;
            return false;
        }

        private bool TryReadHeaders(out int contentStartIndex, out IDictionary<string, string> headers)
        {
            Dictionary<string, string> _headers = new Dictionary<string, string>();
            headers = null;
            contentStartIndex = -1;

            var headerEndIndex = Text.IndexOf(_HeaderContentSeperator);
            if (headerEndIndex == -1)
            {
                return false;
            }

            string[] headerLines = Text.Substring(0, headerEndIndex).Split(new[] { _HeaderDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            headers = headerLines.Select(l =>
            {
                var index = l.IndexOf(_HeaderKeyValueSeperator);
                return new KeyValuePair<string, string>(l.Substring(0, index).Trim(), l.Substring(index + 1));
            }).ToDictionary(p => p.Key, p => p.Value);
            contentStartIndex = headerEndIndex + _HeaderContentSeperator.Length;
            return true;
        }

        private bool TryReadContent(int startIndex, int length, out string content)
        {
            if (Text.Length >= startIndex + length)
            {
                content = Text.Substring(startIndex, length);
                return true;
            }
            content = null;
            return false;
        }

        internal void Append(string chunk)
        {
            if (string.IsNullOrEmpty(chunk))
            {
                return;
            }
            TextBuffer.Append(chunk);
        }
    }
}
