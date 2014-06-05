using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace SherpaDesk
{
    public class CustomCellTapCommand : DataGridCommand
    {
        public CustomCellTapCommand()
        {
            this.Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var ticketKey = ((TicketBaseResponse)(((DataGridCellInfo)(parameter)).Item)).TicketKey;
            (this.Owner.DataContext as WorkListPageViewModel).OnCommandExecuted(ticketKey);
        }
    }

}
