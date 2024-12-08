using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

[assembly: XmlnsDefinition("http://fasciculus.github.io/2024/maui", "Fasciculus.Maui.Controls")]
[assembly: XmlnsPrefix("http://fasciculus.github.io/2024/maui", "fasciculus")]

namespace Fasciculus.Maui
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder UseMauiFasciculus(this MauiAppBuilder builder)
        {
            return builder;
        }
    }
}
