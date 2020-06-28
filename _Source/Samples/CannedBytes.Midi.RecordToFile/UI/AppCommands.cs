using System.Windows.Input;

namespace CannedBytes.Midi.RecordToFile.UI
{
    internal static class AppCommands
    {
        public static readonly RoutedCommand StartStop = new UpdatingRoutedUICommand("Start", "StartStopCommand", typeof(AppCommands));
    }
}