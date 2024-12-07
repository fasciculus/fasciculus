﻿using System.IO;
using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
    public class EsiMarketPrice
    {
        [JsonPropertyName("average_price")]
        public double AveragePrice { get; set; }

        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }
    }

    public class EsiMarketOrder
    {
        [JsonPropertyName("is_buy_order")]
        public bool IsBuyOrder { get; set; }

        [JsonPropertyName("location_id")]
        public long LocationId { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("type_id")]
        public int Type { get; set; }

        [JsonPropertyName("volume_remain")]
        public int Quantity { get; set; }

        public void Write(Stream stream)
        {
            stream.WriteBool(IsBuyOrder);
            stream.WriteLong(LocationId);
            stream.WriteDouble(Price);
            stream.WriteInt(Quantity);
        }

        public static EsiMarketOrder Read(Stream stream)
        {
            bool isBuyOrder = stream.ReadBool();
            long locationId = stream.ReadLong();
            double price = stream.ReadDouble();
            int quantity = stream.ReadInt();

            return new()
            {
                IsBuyOrder = isBuyOrder,
                LocationId = locationId,
                Price = price,
                Quantity = quantity
            };
        }
    }
}
