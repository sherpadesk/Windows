using System;

namespace SherpaDesk.Models
{
    [Flags]
    public enum eTicketStatus : int
    {
        [Details("open")]
        Open = 1,
        [Details("closed")]
        Closed = 2,
        [Details("on_hold")]
        OnHold = 4,
        [Details("waiting")]
        Waiting = 8,
        [Details("new_messages")]
        NewMessages = 16
    }
}
