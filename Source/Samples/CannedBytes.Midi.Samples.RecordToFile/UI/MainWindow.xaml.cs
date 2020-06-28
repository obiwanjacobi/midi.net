using CannedBytes.Midi.Samples.RecordToFile.UI;
using System.Windows;

namespace CannedBytes.Midi.Samples.RecordToFile
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

            this.CommandBindings.Add(new StartStopCommandHandler(appData).ToCommandBinding());

            this.DataContext = appData;
        }
    }
}