using SherpaDesk.Models.Response;
using SherpaDesk.Models.ViewModels;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.UI.Xaml.Navigation;

namespace SherpaDesk
{
    public sealed partial class TimeLogs : SherpaDesk.Common.LayoutAwarePage
    {
        public TimeLogs()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            UpdateGrid((ObservableCollection<TimeResponse>)e.Parameter);
        }

        public void UpdateGrid(ObservableCollection<TimeResponse> list)
        {
            this.DataContext = new TimeLogsModel(list);
        }
    }

    public class CustomCommitEditCommand : DataGridCommand
    {
        public CustomCommitEditCommand()
        {
            this.Id = CommandId.CommitEdit;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as EditContext;

            this.Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context);

            TimeResponse time = context.CellInfo.Item as TimeResponse;
            if (time != null)
            {
                //TODO: update time in Sherpadesk API
            }
        }
    }
}
