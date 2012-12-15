using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace CannedBytes.Midi.SysExUtil.UI
{
    class UpdatingRoutedUICommand : RoutedCommand, INotifyPropertyChanged
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
                    OnNotifyPropertyChanged("Text");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
