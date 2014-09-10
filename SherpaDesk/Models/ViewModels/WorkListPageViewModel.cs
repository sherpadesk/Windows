using System;

namespace SherpaDesk.Models.ViewModels
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
