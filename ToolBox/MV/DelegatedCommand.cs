using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToolBox.MV
{
    public class DelegatedCommand : ICommand
    {
        private readonly Action<object> _action;
        private bool _canExecute = true;
        public DelegatedCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            var notifier = parameter as ICanExecuteNotifier;
            if (notifier != null)
            {
                var canExecute = notifier.CanExecute();
                if (canExecute != _canExecute)
                {
                    _canExecute = !_canExecute;
                    OnCanExecuteChanged();
                    return _canExecute;
                }
            }
            return _canExecute;
        }

        //notifier needs to be stored in order to not be garbage collected
        private ICanExecuteNotifier _notifier;
        public ICommand WithNotifier(ICanExecuteNotifier notifier)
        {
            notifier.Command = this;
            _notifier = notifier;
            return this;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteImpl(parameter);
            }
        }

        public virtual void ExecuteImpl(object parameter)
        {
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    public class DelegatedCommand<T> : DelegatedCommand
    {
        private readonly Action<T> _action;
        public DelegatedCommand(Action<T> action):base(null)
        {
            _action = action;
        }

        public override void ExecuteImpl(object parameter)
        {
            _action((T)parameter);
        }
    }
}
