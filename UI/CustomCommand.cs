using System;
using System.Windows.Input;

namespace UI
{
    internal class CustomCommand : ICommand
    {
        private readonly Action action;

        public event EventHandler? CanExecuteChanged;

        public CustomCommand(Action action)
        {
            this.action = action;
        }
        public bool CanExecute(object? parameter)
        {
            return action != null;
        }

        public void Execute(object? parameter)
        {
            action();
        }
    }
}