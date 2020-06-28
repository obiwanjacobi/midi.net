using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CannedBytes.Midi.SysExUtil.Midi;
using CannedBytes.Midi.SysExUtil.UI;

namespace CannedBytes.Midi.SysExUtil
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var appData = new AppData(this.Dispatcher);

            this.CommandBindings.Add(new FileNewCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new FileOpenCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new FileSaveCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new StartStopCommandHandler(appData).ToCommandBinding());
            this.CommandBindings.Add(new PlayCommandHandler(appData).ToCommandBinding());

            DataContext = appData;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppData appData = (AppData)DataContext;

            appData.SelectedContentItems = ContentList.SelectedItems.Cast<MidiSysExBuffer>();
        }

        protected override void OnClosed(EventArgs e)
        {
            var appData = (AppData)this.DataContext;

            // play nice and dispose all (unmanaged) resources
            appData.Dispose();

            base.OnClosed(e);
        }
    }
}