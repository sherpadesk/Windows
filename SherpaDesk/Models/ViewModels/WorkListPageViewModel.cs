using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SherpaDesk
{
    public class WorkListPageViewModel
    {
        public event EventHandler CommandExecuted;

        public void OnCommandExecuted(string ticketKey)
        {
            var commandExecuted = this.CommandExecuted;
            if (commandExecuted != null)
            {
                commandExecuted(ticketKey, new EventArgs());
            }
        }
    }
}
