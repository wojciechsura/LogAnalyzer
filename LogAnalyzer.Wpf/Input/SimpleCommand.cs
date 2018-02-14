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
        // Private fields -----------------------------------------------------

        private Action<object> action;
        private BaseCondition condition;            

        // Private methods ----------------------------------------------------

        private void ConditionValueChanged(object sender, ValueChangedEventArgs args)
        {
            OnCanExecuteChanged();
        }

        // Protected methods --------------------------------------------------

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // Public methods -----------------------------------------------------

        public SimpleCommand(Action<object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;
        }

        public SimpleCommand(Action<object> action, BaseCondition condition)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            this.action = action;
            this.condition = condition;
            this.condition.ValueChanged += ConditionValueChanged;
        }

        public bool CanExecute(object parameter)
        {
            return condition?.GetValue() ?? true;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
