using System;
using System.Windows.Input;

namespace MouseChef
{
    internal class Command : ICommand
    {
        private readonly Action _action;

        public Command(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();

        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
