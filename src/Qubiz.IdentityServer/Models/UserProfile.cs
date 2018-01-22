using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qubiz.IdentityServer.Models
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Company { get; set; }
        public DateTime BirthDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string SkypeID { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime WorkStartDate { get; set; }
        public string CarModel { get; set; }
        public string CarRegistration { get; set; }
        public string TShirtSize { get; set; }
        public DateTime IDIssueDate { get; set; }
        public DateTime IDExpiryDate { get; set; }
        public DateTime PassIssueDate { get; set; }
        public DateTime PassExpiryDate { get; set; }
    }
}
