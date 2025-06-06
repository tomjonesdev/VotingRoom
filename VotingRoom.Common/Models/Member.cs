using System.ComponentModel.DataAnnotations;

namespace VotingRoom.Common.Models;

public class Member
{
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }
}