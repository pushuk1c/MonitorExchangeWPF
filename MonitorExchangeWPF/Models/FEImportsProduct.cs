using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Models
{
    internal class FEImportsProduct
    {
        [JsonPropertyName("GUIDFile")]
        public string GUIDFile { get; set; }

        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonPropertyName("Artikul")]
        public string Artikul { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Size")]
        public string Size { get; set; }

        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("View")]
        public string View { get; set; }

        [JsonPropertyName("Manufacturer")]
        public string Manufacturer { get; set; }

        [JsonPropertyName("Country")]
        public string Country { get; set; }

        [JsonPropertyName("Material")]
        public string Material { get; set; }

        [JsonPropertyName("Season")]
        public string Season { get; set; }

        [JsonPropertyName("Color")]
        public string Color { get; set; }

        [JsonPropertyName("Categoria")]
        public string Categoria { get; set; }

        [JsonPropertyName("Marke")]
        public string Marke { get; set; }

        [JsonPropertyName("Brend")]
        public string Brend { get; set; }

        [JsonPropertyName("Sex")]
        public string Sex { get; set; }

    }
}
