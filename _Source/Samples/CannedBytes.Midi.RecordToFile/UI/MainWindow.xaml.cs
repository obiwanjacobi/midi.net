using System.Windows;
using CannedBytes.Midi.RecordToFile.UI;

namespace CannedBytes.Midi.RecordToFile
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