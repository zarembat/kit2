using System;
using System.Windows.Input;

namespace Insurance_company.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;
        private object parameter { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public void Execute(object parameter = null)
        {
            _command(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

    }
}