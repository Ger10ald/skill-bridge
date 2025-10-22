

using System;
using SkillBridge.Core.Models;
using Xunit;

namespace SkillBridge.Tests.Domain;


public class SkillTest
{
    [Fact]
    public void Constructor_TrimsTitle()
    {
        var s = new Skill("  Guitar  ");
        Assert.Equal("Guitar", s.Title);
    }

    [Fact]
    public void Constructor_EmptyOrWhitespace_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Skill(""));
        Assert.Throws<ArgumentException>(() => new Skill("   "));
    }

    [Fact]
    public void Constructor_TitleTooShortOrTooLong_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Skill("G")); // 1 char
        var eightyOne = new string('x', 81);
        Assert.Throws<ArgumentOutOfRangeException>(() => new Skill(eightyOne));
    }

    [Fact]
    public void AssignCategory_SetsNullableId()
    {
        var s = new Skill("Piano");
        Assert.Null(s.CategoryId);

        s.AssignCategory(3);
        Assert.Equal(3, s.CategoryId);

        s.AssignCategory(null);
        Assert.Null(s.CategoryId);
    }

    [Fact]
    public void UserSkills_Collection_IsInitialized()
    {
        var s = new Skill("Cooking");
        Assert.NotNull(s.UserSkills);
    }

}
