
namespace SherpaDesk.Models
{
    public enum eTimeType
    {
        recent, // {returns all recent time logs}
        unlinked_fb, // {returns all recent unlinked FreshBooks time logs}
        unlinked_fb_billable, // {returns all recent unlinked FreshBooks and Billable time logs}
        linked_fb, // {returns all recent linked FreshBooks time logs}
        invoiced, // {returns all recent invoiced time Logs}
        not_invoiced, // {returns all recent not invoiced time Logs}
        not_invoiced_billable, // {returns all recent not invoiced and Billable time Logs}
        not_invoiced_nonbillable // {returns all recent not invoiced and Non Billable time Logs}
    }
}
