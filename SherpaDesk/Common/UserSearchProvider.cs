using System;
using System.Collections;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;

namespace SherpaDesk.Common
{
    public class UserSearchProvider : TextSearchProvider
    {
        public override IEnumerable FilteredItems
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HasItems
        {
            get { throw new NotImplementedException(); }
        }
    }
}
