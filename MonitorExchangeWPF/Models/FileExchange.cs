using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Models
{
    internal class FileExchange
    {

        [JsonPropertyName("dataCreate")]
        public DateTime Date { get; set; }

        [JsonPropertyName("id")]
        public string GUID { get; set; }

        [JsonPropertyName("strId")]
        public string GUIDFile { get; set; }

        [JsonPropertyName("item")]
        public int Item { get; set; }

        [JsonPropertyName("inAll")]
        public int AllIn { get; set; }

        [JsonPropertyName("IsUpload")]
        public bool IsUpload { get; set; }
    }
}
