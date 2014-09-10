using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;

namespace SherpaDesk.Models.ViewModels
{
    public class WorkListViewData : ObservableCollection<TicketSearchResponse>, ISupportIncrementalLoading
    {
        public event EventHandler LoadNext;

        public WorkListViewData()
        {
        }

        public WorkListViewData(IEnumerable<TicketSearchResponse> data)
            : base(data)
        {
        }

        public bool HasMoreItems
        {
            get { return this.Count >= SearchRequest.DEFAULT_PAGE_COUNT; }
        }

        private async Task<LoadMoreItemsResult> LoadNextPage()
        {
            return await Task.Run(() =>
            {
                if (this.LoadNext != null)
                    this.LoadNext(this, EventArgs.Empty);
                return new LoadMoreItemsResult { Count = (uint)this.Count };
            });
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(c => LoadNextPage());
        }
    }

    public class WorkListViewModel : INotifyPropertyChanged
    {
        private int _pageIndex;
        private WorkListViewData _data;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DataLoading;

        public WorkListViewModel()
        {
            this._pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;
            this._data = null;
        }

        public WorkListViewData Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                
                //_data.LoadNext += (s, e) => PageNext();

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
                return _data != null && _data.Count >= SearchRequest.DEFAULT_PAGE_COUNT;
            }
        }

        public void PageNext()
        {
            _pageIndex++;
            //if (this.PropertyChanged != null)
            //{
            //    this.PropertyChanged(this,
            //          new PropertyChangedEventArgs("PageIndex"));
            //}
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
            foreach (TicketSearchResponse item in _data)
            {
                item.IsChecked = @checked;
            }
        }

        public void SelectOne(bool @checked, int ticketId)
        {
            foreach (TicketSearchResponse item in _data)
            {
                if (item.TicketId == ticketId)
                    item.IsChecked = @checked;
            }
        }
    }
}
