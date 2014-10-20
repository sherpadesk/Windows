using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace SherpaDesk
{
    public class CustomBeginEditCommand : DataGridCommand
    {
        public CustomBeginEditCommand()
        {
            // Set Id so that this command can be properly associated with the desired action/event
            Id = CommandId.BeginEdit;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);
        }
    }
}
