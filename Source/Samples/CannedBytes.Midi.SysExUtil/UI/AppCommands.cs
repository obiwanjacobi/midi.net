using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CannedBytes.Midi.SysExUtil.UI
{
    internal static class AppCommands
    {
        public static readonly RoutedCommand StartStop = new UpdatingRoutedUICommand("Start", "StartStopCommand", typeof(AppCommands));
    }
}
