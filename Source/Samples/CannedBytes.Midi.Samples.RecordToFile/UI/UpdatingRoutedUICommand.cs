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
            this._text = text;
        }

        private string _text;

        public string Text
        {
            get { return this._text; }
            set
            {
                if (this._text != value)
                {
                    this._text = value;
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