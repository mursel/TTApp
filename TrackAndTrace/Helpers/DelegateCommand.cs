using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace TrackAndTrace.Helpers
{
    public class DelegateCommand : ICommand
    {
        Action<object> cmdDelegate;
        Func<object, bool> canExecute;

        public DelegateCommand(Action<object> action)
        {
            this.cmdDelegate = action;
            this.canExecute = (x) => { return true; };
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            cmdDelegate(parameter);
        }

    }
}