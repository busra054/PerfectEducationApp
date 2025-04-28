
using WebApplication_Domain.Entities;

namespace WebApplication_Domain.Entities
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<Admin> Admins { get; set; }
        public IEnumerable<TeacherRequest> PendingRequests { get; set; }
    }
}
