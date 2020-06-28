using System.Windows.Input;

namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    internal static class AppCommands
    {
        public static readonly RoutedCommand FileNew = new UpdatingRoutedUICommand("New", "FileNewCommand", typeof(AppCommands));
        public static readonly RoutedCommand FileOpen = new UpdatingRoutedUICommand("Open", "FileOpenCommand", typeof(AppCommands));
        public static readonly RoutedCommand FileSave = new UpdatingRoutedUICommand("Save", "FileSaveCommand", typeof(AppCommands));

        public static readonly RoutedCommand StartStop = new UpdatingRoutedUICommand("Start", "StartStopCommand", typeof(AppCommands));
        public static readonly RoutedCommand Play = new UpdatingRoutedUICommand("Play", "PlayCommand", typeof(AppCommands));
    }
}
