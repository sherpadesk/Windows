
namespace SherpaDesk.Models
{
    public enum eTimeType
    {
        [Details("")]
        None,
        [Details("recent")]
        Recent, // {returns all recent time logs}
        [Details("unlinked_fb")]
        RecentUnlinkedFreshBooks, // {returns all recent unlinked FreshBooks time logs}
        [Details("unlinked_fb_billable")]
        RecentUnlinkedFreshBooksAndBillable, // {returns all recent unlinked FreshBooks and Billable time logs}
        [Details("linked_fb")]
        RecentLinkedFreshBooks, // {returns all recent linked FreshBooks time logs}
        [Details("invoiced")]
        RecentInvoiced, // {returns all recent invoiced time Logs}
        [Details("not_invoiced")]
        RecentNotInvoiced, // {returns all recent not invoiced time Logs}
        [Details("not_invoiced_billable")]
        RecentNotInvoicedAndBillable, // {returns all recent not invoiced and Billable time Logs}
        [Details("not_invoiced_nonbillable")]
        RecentNotInvoicedAndNonBillable // {returns all recent not invoiced and Non Billable time Logs}
    }
}
