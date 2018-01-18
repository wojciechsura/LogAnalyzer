using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.Wpf.Input
{
    public class SimpleCommand : ICommand
    {
        private Action<object> action;
        private Func<object, bool> canExecute;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public SimpleCommand(Action<object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;
            this.canExecute = (o) => true;
        }

        public SimpleCommand(Action<object> action, Func<object, bool> canExecute)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));

            this.action = action;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged;
    }
}
