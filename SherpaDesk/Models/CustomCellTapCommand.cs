using SherpaDesk.Models.Response;
using SherpaDesk.Models.ViewModels;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace SherpaDesk.Models
{
    public class CustomCellTapCommand : DataGridCommand
    {
        public CustomCellTapCommand()
        {
            Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var ticketKey = ((TicketBaseResponse)(((DataGridCellInfo)(parameter)).Item)).TicketKey;
            var workListPageViewModel = Owner.DataContext as WorkListPageViewModel;
            if (workListPageViewModel != null)
                workListPageViewModel.OnCommandExecuted(ticketKey);
        }
    }

}
