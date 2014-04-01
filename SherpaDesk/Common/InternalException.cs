using SherpaDesk.Models;
using System;

namespace SherpaDesk.Common
{
    public class InternalException : Exception
    {
        public string Title { get; private set; }

        public InternalException(string message, eErrorType title)
            : base(message)
        {
            this.Title = title.Details();
        }
    }
}
