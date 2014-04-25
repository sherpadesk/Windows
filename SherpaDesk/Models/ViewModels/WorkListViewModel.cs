using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;

namespace SherpaDesk.Models
{
    public class WorkListViewModel : INotifyPropertyChanged
    {
        private int _pageIndex;
        private ObservableCollection<WorkListItem> _data;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DataLoading;

        public WorkListViewModel()
        {
            this._pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;
            this._data = null;
        }

        public ObservableCollection<WorkListItem> Data
        {
            get
            {
                //return new ObservableCollection<WorkListItem>(
                //    (new WorkListItem[1] { new WorkListItem { Subject = "text", TicketId = 1, Status = "Open" } }).AsEnumerable());
                return _data;
            }
            set
            {
                _data = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("Data"));
                    this.PropertyChanged(this,
                        new PropertyChangedEventArgs("PagePrevEnabled"));
                    this.PropertyChanged(this,
                        new PropertyChangedEventArgs("PageNextEnabled"));
                }
            }
        }

        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
        }

        public bool PagePrevEnabled
        {
            get
            {
                return _pageIndex > SearchRequest.DEFAULT_PAGE_INDEX;
            }
        }

        public bool PageNextEnabled
        {
            get
            {
                return _data != null ? _data.Count >= SearchRequest.DEFAULT_PAGE_COUNT : false;
            }
        }

        public void PageNext()
        {
            _pageIndex++;
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this,
                      new PropertyChangedEventArgs("PageIndex"));
            }
            if (this.DataLoading != null)
            {
                this.DataLoading(this, EventArgs.Empty);
            }
        }

        public void PagePrev()
        {
            if (_pageIndex > SearchRequest.DEFAULT_PAGE_INDEX)
            {
                _pageIndex--;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,
                          new PropertyChangedEventArgs("PageIndex"));
                }
                if (this.DataLoading != null)
                {
                    this.DataLoading(this, EventArgs.Empty);
                }
            }
        }

        public void SelectAll(bool @checked)
        {
            for (var i = 0; i < _data.Count; i++)
            {
                _data[i].IsChecked = @checked;
            }
        }

        public void SelectOne(bool @checked, int ticketId)
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (_data[i].TicketId == ticketId)
                    _data[i].IsChecked = @checked;
            }
        }

        public void AddList(IList<TicketSearchResponse> list)
        {
            var result = new ObservableCollection<WorkListItem>();
            foreach (var x in list)
            {
                result.Add(new WorkListItem
                {
                    TicketId = x.TicketId,
                    TicketKey = x.TicketKey,
                    Subject = x.Subject.Length > 80 ? x.Subject.Substring(0, x.Subject.Substring(0, 80).LastIndexOf(' ')) + "..." : x.Subject,
                    AccountName = x.AccountName,
                    ClassName = x.ClassName,
                    Status = x.Status,
                    TechnicianFullName = Helper.FullName(x.TechnicianFirstName, x.TechnicianLastName, x.TechnicianEmail),
                    TicketNumber = x.TicketNumber,
                    UserFullName = x.UserFullName,
                    DaysOld = (x.СreatedTime != DateTime.MinValue) ?
                        (DateTime.Now - x.СreatedTime).CalculateDate() :
                        string.Empty
                });
            }
            this.Data = result;
        }
    }

    public class WorkListItem : INotifyPropertyChanged
    {
        public string Index { get; set; }

        public int TicketId { get; set; }

        public string TicketKey { get; set; }

        public int TicketNumber { get; set; }

        public string TechnicianFullName { get; set; }

        public string Subject { get; set; }

        public string Status { get; set; }

        public string UserFullName { get; set; }

        public string AccountName { get; set; }

        public string ClassName { get; set; }

        public string DaysOld { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _checked = false;

        public bool IsChecked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this,
                              new PropertyChangedEventArgs("IsChecked"));
                    }
                }
            }
        }
    }
}
