using SherpaDesk.Common;
using SherpaDesk.Models.Request;
using SherpaDesk.Models.Response;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace SherpaDesk.Models
{
    public class WorkListViewModel : INotifyPropertyChanged
    {
        private int _pageIndex;
        private IList<TicketSearchResponse> _data;

        public event PropertyChangedEventHandler PropertyChanged;

        public WorkListViewModel()
        {
            this._pageIndex = SearchRequest.DEFAULT_PAGE_INDEX;
            this._data = null;
        }

        public IList<TicketSearchResponse> Data
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
                return _data.Count >= SearchRequest.DEFAULT_PAGE_COUNT;
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
            }
        }

    }
}
