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
        
        private string _userName = string.Empty;
        [Required, MaxLength(80)]  
        public string UserName 
        { 
            get => _userName; 
            set => _userName = (value ?? string.Empty).Trim(); 
        }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        

        private string _email = string.Empty;
        [Required, EmailAddress, MaxLength(254)]  
        public string Email 
        { 
            get => _email; 
            set => _email = (value ?? string.Empty).Trim().ToLowerInvariant(); 
        }

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        [Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        [MaxLength(700)]
        public string Bio { get; set; } = string.Empty;
        [MaxLength(40)]
        public string Role {  get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<SkillRequest> SkillRequests { get; set; } = new List<SkillRequest>();
        public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
        public ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    }
}
