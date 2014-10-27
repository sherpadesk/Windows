using SherpaDesk.Models.Response;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Core;

namespace SherpaDesk.Models.ViewModels
{
    public class TimeLogsModel : ViewModelBase
    {
        private ObservableCollection<TimeResponse> _data = null;

        public ObservableCollection<TimeResponse> Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public TimeLogsModel(ObservableCollection<TimeResponse> data)
        {
            this.Data = data;
        }
    }
}
