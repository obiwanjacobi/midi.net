using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CannedBytes.Midi.Samples.RecordToFile.UI
{
    internal sealed class UpdatingRoutedUICommand : RoutedCommand, INotifyPropertyChanged
    {
        public UpdatingRoutedUICommand()
        { }

        public UpdatingRoutedUICommand(string text, string name, Type ownerType)
            : base(name, ownerType)
        {
            this.text = text;
        }

        private string text;

        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    OnNotifyPropertyChanged(nameof(Text));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}