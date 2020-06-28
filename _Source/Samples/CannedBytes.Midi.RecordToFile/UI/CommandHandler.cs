using System;
using System.Windows.Input;

namespace CannedBytes.Midi.RecordToFile.UI
{
    abstract class CommandHandler
    {
        public RoutedCommand Command { get; protected set; }

        protected virtual void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Command != null);
        }

        protected virtual void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        public CommandBinding ToCommandBinding()
        {
            if (Command == null) throw new InvalidOperationException("The Command property must be assigned in the derived class' ctor.");

            var cmdBinding = new CommandBinding(Command, Execute, CanExecute);

            return cmdBinding;
        }
    }
}