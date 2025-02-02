using System.ComponentModel.DataAnnotations;

namespace VotingRoom.Common.Models;

public class Member
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}