
using System;
using System.Collections.Generic;
using System.Threading;
using VSCode.JsonRpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSCode.Completion
{
    public class Feature : IFeature
    {
        public LanguageServer Server { get; private set; }

        public virtual void Initialize(LanguageServer languageServer)
        {
            Server = languageServer;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Feature() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class Request
    {
        public Request(CancellationToken token, RequestContext context)
        {
            Token = token;
            Context = context;
        }

        protected RequestContext Context { get; }

        public CancellationToken Token { get; }
    }

    public enum CompletionItemKind
    {
        Text = 1,
        Method = 2,
        Function = 3,
        Constructor = 4,
        Field = 5,
        Variable = 6,
        Class = 7,
        Interface = 8,
        Module = 9,
        Property = 10,
        Unit = 11,
        Value = 12,
        Enum = 13,
        Keyword = 14,
        Snippet = 15,
        Color = 16,
        File = 17,
        Reference = 18
    }

    public class CompletionItem
    {
        public string Label { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CompletionItemKind Kind { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Documentation { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SortText { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FilterText { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InsertText { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TextEdit TextEdit { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public class CompletionList
    {
        /// <summary>
        ///   This list is not complete. Further typing should result in recomputing this list.
        /// </summary>
        public bool IsIncomplete { get; set; } = true;
        /// <summary>
        ///   The completion items.
        /// </summary>
        public IList<CompletionItem> Items { get; set; } = new List<CompletionItem>();
    }

    public class CompletionRequest : Request
    {
        public TextDocumentPositionParams Parameters { get; }

        public CompletionRequest(CancellationToken token, RequestContext context, TextDocumentPositionParams parameters) : base(token, context)
        {
            Parameters = parameters;
        }

        public void SendResult(IEnumerable<CompletionItem> items)
        {
            Context.SendResult(items);
        }

        public void SendResult(CompletionList list)
        {
            Context.SendResult(list);
        }
    }

    public class CompletionResolveRequest : Request
    {
        public CompletionItem CompletionItem { get; }
        public CompletionResolveRequest(CancellationToken token, RequestContext context, CompletionItem completionItem) : base(token, context)
        {
            CompletionItem = completionItem;
        }

        public void SendResult(CompletionItem item)
        {
            Context.SendResult(item);
        }
    }

    public class CompletionFeature : Feature
    {
        public event EventHandler<CompletionRequest> Completion;
        public event EventHandler<CompletionResolveRequest> CompletionResolve;

        public override void Initialize(LanguageServer languageServer)
        {
            base.Initialize(languageServer);
            Server.RequestReceived += RequestReceived;
        }

        private void RequestReceived(object sender, RequestContext context)
        {
            var cts = new CancellationTokenSource();
            context.RequestCancelled += (s, e) => cts.Cancel();
            switch (context.Request.Method)
            {
                case "textDocument/completion":
                    Completion?.Invoke(this, new CompletionRequest(cts.Token, context, context.Request.Params.ToObject<TextDocumentPositionParams>()));
                    break;
                case "completionItem/resolve":
                    CompletionResolve?.Invoke(this, new CompletionResolveRequest(cts.Token, context, context.Request.Params.ToObject<CompletionItem>()));
                    break;
            }
        }
    }
}