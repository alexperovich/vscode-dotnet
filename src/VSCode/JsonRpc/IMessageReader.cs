using System;
using System.Threading.Tasks;

namespace VSCode.JsonRpc
{
    internal interface IMessageReader : IDisposable
    {
        IMessage Read();
    }
}
