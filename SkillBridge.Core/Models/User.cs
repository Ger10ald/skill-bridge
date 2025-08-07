using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        

        public string Bio { get; set; } = string.Empty;
        public string Role {  get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<SkillRequest> SkillRequests { get; set; } = new List<SkillRequest>();
        public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
        public ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    }
}
