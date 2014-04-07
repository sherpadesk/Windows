using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;

namespace SherpaDesk.Common
{
    public class UserSearchProvider : TextSearchProvider
    {
        public UserSearchProvider()
            : base()
        {

        }
        public override System.Collections.IEnumerable FilteredItems
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HasItems
        {
            get { throw new NotImplementedException(); }
        }
    }
}
