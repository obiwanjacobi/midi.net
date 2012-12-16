using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class FileNewCommandHandler : CommandHandler
    {
        private AppData appData;

        public FileNewCommandHandler(AppData appData)
        {
            this.appData = appData;
            Command = AppCommands.FileNew;
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            this.appData.SelectedContentItems = null;
            this.appData.SysExBuffers.Clear();

            base.Execute(sender, e);
        }
    }
}
