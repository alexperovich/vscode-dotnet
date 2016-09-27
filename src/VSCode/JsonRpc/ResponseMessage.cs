﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSCode.JsonRpc
{
    /// <summary>
    /// An <see cref="IMessage" /> implementation representing a JSON-RPC response.
    /// </summary>
    public class ResponseMessage : IMessage
    {
        /// <summary>
        /// Creates a new <see cref="ResponseMessage" /> instance.
        /// </summary>
        public ResponseMessage()
        {
            Jsonrpc = Constants.JsonRpc.SupportedVersion;
        }

        /// <summary>
        /// A <see cref="JObject" /> representing an error that occurred while processing the request.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JToken Error { get; set; }

        /// <summary>
        /// A unique ID assigned to the request/response session. The request creator is responsible for this value.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// See <see cref="IMessage.Jsonrpc" />.
        /// </summary>
        public string Jsonrpc { get; set; }

        /// <summary>
        /// A <see cref="JObject" /> representing the result of processing the request.
        /// </summary>
        public JToken Result { get; set; }
    }
}
