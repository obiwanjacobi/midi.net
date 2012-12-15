using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            this.CommandBindings.Add(new StartStopCommandHandler(appData).ToCommandBinding());

            DataContext = appData;
        }
    }
}
