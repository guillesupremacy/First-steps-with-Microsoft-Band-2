using System;
using System.Windows.Input;

namespace SensorsMB2.Utilities
{
    public class CustomCommand :ICommand
    {
        private Action<object> execute { get; set; }
        private Predicate<object> canExecute { get; set; }

        public CustomCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            bool b = canExecute?.Invoke(parameter) ?? true;
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
