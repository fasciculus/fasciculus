using System;

namespace Fasciculus.Blog.Preview.Models
{
    public class ViewModel
    {
        public required string Title { get; init; }

        public required DateTime Version { get; init; }
    }
}
