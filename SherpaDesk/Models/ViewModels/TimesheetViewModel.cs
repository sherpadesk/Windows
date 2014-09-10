using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using SherpaDesk.Models.Response;

namespace SherpaDesk.Models.ViewModels
{
    public class TimesheetViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TimesheetEventArgs> OnDataLoading;
        //public event EventHandler<TimesheetEventArgs> OnSelectedDate;

        public TimesheetViewModel()
        {
            DateTime date = DateTime.Now;
            _currentDate = date;
            _startDate = new DateTime(date.Year, date.Month, 1);
            _endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            _timesheet = null;
            _timeLogList = null;
            //_selectedDateRange = null;
        }

        public void RefreshData()
        {
            if (this.OnDataLoading != null)
            {
                this.OnDataLoading(this, new TimesheetEventArgs(this));
            }
        }

        private ObservableCollection<CalendarCell> _timesheet;

        public ObservableCollection<CalendarCell> Timesheet
        {
            get
            {
                return _timesheet;
            }
            set
            {
                _timesheet = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("Timesheet"));
                }
            }
        }

        private ObservableCollection<TimeResponse> _timeLogList;

        public ObservableCollection<TimeResponse> TimeLogList
        {
            get { return _timeLogList; }
            set
            {
                _timeLogList = value;
                this.Timesheet = new ObservableCollection<CalendarCell>(_timeLogList
                    .GroupBy(x => x.Date.Date)
                    .Select(time => new CalendarCell
                    {
                        Date = time.Key,
                        Text = time.Sum(x => x.Hours).ToString("F"), 
                        Value = time.Sum(x => x.Hours)
                    }));
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TimeLogList"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("FullList"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("NonTicketsList"));
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TicketTimeList"));
                }
            }
        }

        public ObservableCollection<TimeResponse> FullList
        {
            get
            {
                return new ObservableCollection<TimeResponse>(_timeLogList
                    .Where(x => x.Date.Date == _currentDate.Date));
            }
        }

        public ObservableCollection<TimeResponse> NonTicketsList
        {
            get
            {
                return new ObservableCollection<TimeResponse>(_timeLogList
                    .Where(x => x.Date.Date == _currentDate.Date && x.TicketId == 0));
            }
        }

        public Visibility VisibleNonTickets
        {
            get
            {
                return _timeLogList.Any(x => x.Date.Date == _currentDate.Date && x.TicketId == 0) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ObservableCollection<TimeResponse> TicketTimeList
        {
            get
            {
                return new ObservableCollection<TimeResponse>(_timeLogList
                    .Where(x => x.Date.Date == _currentDate.Date && x.TicketId > 0));
            }
        }

        public Visibility VisibleTicketTime
        {
            get
            {
                return _timeLogList.Any(x => x.Date.Date == _currentDate.Date && x.TicketId > 0) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private DateTime _currentDate;

        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (_currentDate != value)
                {
                    if (value.Month != _currentDate.Month || value.Year != _currentDate.Year)
                    {
                        _startDate = new DateTime(value.Year, value.Month, 1);
                        _endDate = new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
                        if (this.OnDataLoading != null)
                        {
                            this.OnDataLoading(this, new TimesheetEventArgs(this));
                        }
                    }
                    _currentDate = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("CurrentDate"));
                    }
                }
            }
        }
        //public string SelectedDateText
        //{
        //    get
        //    {
        //        return _currentDate.ToString("MMMM dd, yyyy - dddd");
        //    }
        //}

        //public DateTime SelectedDate
        //{
        //    get
        //    {
        //        return _currentDate;
        //    }
        //    set
        //    {
        //        if (_currentDate != value)
        //        {
        //            _currentDate = value;
        //            if (this.PropertyChanged != null)
        //            {
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("CurrentDate"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedDate"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedDateText"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("NonTicketsList"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("VisibleNonTickets"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("TicketTimeList"));
        //                this.PropertyChanged(this, new PropertyChangedEventArgs("VisibleTicketTime"));
        //            }
        //            if (this.OnSelectedDate != null)
        //            {
        //                this.OnSelectedDate(this, new TimesheetEventArgs(_currentDate, _currentDate));
        //            }
        //        }
        //    }
        //}

        //private CalendarDateRange? _selectedDateRange;
        //public CalendarDateRange? SelectedDateRange
        //{
        //    get
        //    {
        //        return _selectedDateRange;
        //    }
        //    set
        //    {
        //        if (value != _selectedDateRange)
        //        {
        //            _selectedDateRange = value;
        //            if (value.HasValue)
        //            {
        //                _currentDate = value.Value.StartDate;
        //                if (this.PropertyChanged != null)
        //                {
        //                    this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedDateRange"));
        //                }
        //                if (this.OnSelectedDate != null)
        //                {
        //                    this.OnSelectedDate(this, new TimesheetEventArgs(value.Value.StartDate, value.Value.EndDate));
        //                }
        //            }
        //        }
        //    }
        //}

        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StartDate"));
                    }
                }
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("EndDate"));
                    }
                }
            }
        }
    }

    public class TimesheetEventArgs : EventArgs
    {
        public TimesheetEventArgs(TimesheetViewModel model)
        {
            this.StartDate = model.StartDate;
            this.EndDate = model.EndDate;
        }
        public TimesheetEventArgs(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

    }
}
