using System;
using System.Windows.Input;

namespace ToolBox.MV
{
    public class DelegatedCommand : ICommand
    {
        protected readonly Action<object> _action;
        protected Predicate<object> _canExecute;
        public DelegatedCommand(Action<object> action, Predicate<object> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }


    public class DelegatedCommand<T> : DelegatedCommand
    {
        public DelegatedCommand(Action<T> action, Predicate<T> canExecute = null) 
            :base(p => action((T)p), p => canExecute?.Invoke((T)p) ?? true)
        {
        }
    }
}
