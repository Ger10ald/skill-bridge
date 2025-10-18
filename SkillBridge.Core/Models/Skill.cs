using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Core.Models
{
    /// <summary>
    /// A learnable/teachable skill like "Piano", "Violin", or "Cooking"
    /// Used by SkillOffer to describe what is being taught
    /// </summary>

    public sealed class Skill
    {
        public int Id { get; private set; }
        
        [Required, MaxLength(80)]
        public string Title { get; private set; } = string.Empty;

        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }

        public ICollection<UserSkill> UserSkills { get; } = new List<UserSkill>();

        private Skill() { }

        public Skill(string title, int? categoryId = null)
        {
            SetTitle(title);
            CategoryId = categoryId;
        }

        // Centralize Title rules so they're always applied consistently.
        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Skill title is required.", nameof(title));

            title = title.Trim();

            if (title.Length < 2 || title.Length > 80)
                throw new ArgumentOutOfRangeException(nameof(title), "Skill title must be 2–80 characters.");

            Title = title;
        }

        // Keep category assignment explicit. You can add more rules later if needed.
        public void AssignCategory(int? categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
