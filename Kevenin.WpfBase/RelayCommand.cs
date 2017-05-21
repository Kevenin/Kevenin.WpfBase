using System;
using System.Windows.Input;

namespace Kevenin.WpfBase
{
    public class RelayCommand<T> : ICommand
    {
        #region Private Fields

        private Func<bool> canExecuteEvaluator;

        private Action methodToExecute;

        private Action<T> methodWithParameterToExecute;

        #endregion Private Fields

        #region Public Constructors

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute) : this(methodToExecute, null)
        {
        }

        public RelayCommand(Action<T> methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodWithParameterToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action<T> methodToExecute) : this(methodToExecute, null)
        {
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion Public Events

        #region Public Methods

        public bool CanExecute(object parameter)
        {
            if (this.canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this.canExecuteEvaluator.Invoke();
                return result;
            }
        }

        public void Execute(object parameter)
        {
            if (methodToExecute != null)
                this.methodToExecute.Invoke();
            else
                this.methodWithParameterToExecute((T)parameter);
        }

        #endregion Public Methods

    }
}
