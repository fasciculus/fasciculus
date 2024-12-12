using Fasciculus.Maui.ComponentModel;
using System.ComponentModel;

namespace Fasciculus.Eve.Services
{
    public interface ITrades : INotifyPropertyChanged
    {

    }

    public partial class Trades : MainThreadObservable, ITrades
    {

    }
}
