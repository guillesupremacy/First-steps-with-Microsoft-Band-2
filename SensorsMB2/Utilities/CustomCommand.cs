using System;
using System.Windows.Input;

namespace SensorsMB2.Utilities
{
    public class CustomCommand : ICommand
    {
        public CustomCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        private Action<object> execute { get; }
        private Predicate<object> canExecute { get; }

        public bool CanExecute(object parameter)
        {
            var b = canExecute?.Invoke(parameter) ?? true;
            return b;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}