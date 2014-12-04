using SherpaDesk.Models.Response;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SherpaDesk.Models.ViewModels
{
    public class TimeLogModel : INotifyPropertyChanged
    {
        private ObservableCollection<TimeResponse> _list;
        private TimeResponse _time;
        private bool _edit;

        public ObservableCollection<TimeResponse> List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("List"));
                }
            }
        }

        public TimeResponse Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("Time"));
                }
            }
        }

        public bool IsEdit
        {
            get { return _edit; }
            set
            {
                _edit = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("IsEdit"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnDataLoading;

        public TimeLogModel(ObservableCollection<TimeResponse> list)
        {
            _list = list;
        }

        public void RefreshData()
        {
            if (this.OnDataLoading != null)
            {
                this.OnDataLoading(this, new EventArgs());
            }
        }


    }
}
