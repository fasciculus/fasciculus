﻿namespace Fasciculus.Eve.Models
{
    public class EveTradeOptions
    {
        public const int DefaultTargetStationId = 60003760;
        public const int DefaultMaxDistance = 5;
        public const int DefaultMaxVolumePerType = 5000;
        public const int DefaultMaxIskPerType = 10_000_000;

        public int TargetStationId { get; set; } = DefaultTargetStationId;
        public int MaxDistance { get; set; } = DefaultMaxDistance;
        public int MaxVolumePerType { get; set; } = DefaultMaxVolumePerType;
        public int MaxIskPerType { get; set; } = DefaultMaxIskPerType;
    }
}