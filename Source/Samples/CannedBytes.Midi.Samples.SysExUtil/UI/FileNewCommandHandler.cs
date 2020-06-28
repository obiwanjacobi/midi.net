namespace CannedBytes.Midi.Samples.SysExUtil.UI
{
    internal sealed class FileNewCommandHandler : CommandHandler
    {
        private readonly AppData _appData;

        public FileNewCommandHandler(AppData appData)
        {
            _appData = appData;
            Command = AppCommands.FileNew;
        }

        protected override void Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            _appData.SelectedContentItems = null;
            _appData.SysExBuffers.Clear();

            base.Execute(sender, e);
        }
    }
}
