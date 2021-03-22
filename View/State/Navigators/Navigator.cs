using SignalProcessing.View.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalProcessing.View.State.Navigators
{
    public enum ViewType
    {
        Home,
        Generation,
        Operations
    }

    class Navigator : INavigator
    {
        public BaseViewModel CurrentViewModel { get; set; }

        public ICommand UpdateCurrentViewModelCommand => new UpdateCurrentViewModelCommand(this);
    }
}
