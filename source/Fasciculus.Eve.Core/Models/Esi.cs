using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
    public class EsiMarketPrice
    {
        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }

        [JsonPropertyName("average_price")]
        public double AveragePrice { get; set; }

        [JsonPropertyName("adjusted_price")]
        public double AdjustedPrice { get; set; }
    }

    public class EsiMarketOrder
    {
        [JsonPropertyName("type_id")]
        public int Type { get; set; }

        [JsonPropertyName("location_id")]
        public long LocationId { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("volume_remain")]
        public int Quantity { get; set; }
    }
}
