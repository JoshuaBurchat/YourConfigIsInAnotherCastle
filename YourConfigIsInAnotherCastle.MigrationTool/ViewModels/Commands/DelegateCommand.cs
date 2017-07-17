using System;
using System.Windows.Input;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels
{
    /// <summary>
    /// Generic ICommand for view models to use
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }


    }
}
