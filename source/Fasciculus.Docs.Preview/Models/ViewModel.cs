using System;

namespace Fasciculus.Docs.Preview.Models
{
    public class ViewModel
    {
        public required string Title { get; init; }

        public required DateTime Version { get; init; }
    }
}
