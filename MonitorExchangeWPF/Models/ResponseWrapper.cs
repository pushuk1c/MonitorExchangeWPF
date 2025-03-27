using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Models
{
    internal class ResponseWrapper<T>
    {
        [JsonPropertyName("data")]
        public List<T> data { get; set; }

        [JsonPropertyName("saccess")]
        public bool saccess { get; set; }

        [JsonPropertyName("message")]
        public string message { get; set; }

        [JsonPropertyName("meta")]
        public MetaResponse meta { get; set; }
    }

    public class MetaResponse
    {
        [JsonPropertyName("page")]
        public int page { get; set; }

        [JsonPropertyName("pageSize")]
        public int pageSize { get; set; }

        [JsonPropertyName("totalItems")]
        public int totalItems { get; set; }
    }
}
