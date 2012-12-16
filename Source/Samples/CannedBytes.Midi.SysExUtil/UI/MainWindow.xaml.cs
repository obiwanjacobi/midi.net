﻿using System;
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
using CannedBytes.Midi.SysExUtil.Midi;

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
    }
}
