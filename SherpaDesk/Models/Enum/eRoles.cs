
using System;
namespace SherpaDesk.Models
{
    [Flags]
    public enum eRoles : int
    {
        [Details("user")]
        EndUser = 1,
        [Details("tech")]
        Technician = 2,
        [Details("alt_tech")]
        AltTechnician = 4
    }
}
