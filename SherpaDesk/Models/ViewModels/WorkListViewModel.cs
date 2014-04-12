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

namespace SherpaDesk.Models
{
    public class WorkListViewModel : INotifyPropertyChanged
    {
        private int _pageIndex;
        private ObservableCollection<TicketSearchResponse> _data;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DataLoading;

        public WorkListViewModel()
        {
            this._pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;
            this._data = null;
        }

        public ObservableCollection<TicketSearchResponse> Data
        {
            get
            {
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
    }
}
