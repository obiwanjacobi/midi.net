using System.Windows.Input;

namespace CannedBytes.Midi.Samples.RecordToFile.UI
{
    internal static class AppCommands
    {
        public static readonly RoutedCommand StartStop = new UpdatingRoutedUICommand("Start", "StartStopCommand", typeof(AppCommands));
    }
}