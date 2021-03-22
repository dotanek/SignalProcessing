using SignalProcessing.View.Model;
using SignalProcessing.View.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalProcessing.View.Commands
{
    class UpdateCurrentViewModelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private INavigator _navigator;

        public UpdateCurrentViewModelCommand(INavigator navigator)
        {
            _navigator = navigator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;
                switch (viewType)
                {
                    case ViewType.Home:
                        _navigator.CurrentViewModel = new HomeViewModel();
                        break;
                    case ViewType.Generation:
                        _navigator.CurrentViewModel = new GenerationViewModel();
                        break;
                    case ViewType.Operations:
                        _navigator.CurrentViewModel = new OperationsViewModel();
                        break;
                }
            }
        }
    }
}
