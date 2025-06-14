using System.ComponentModel.DataAnnotations;
using VotingRoom.Common.Enums;

namespace VotingRoom.Common.Models;

public class RoomCreateRequest
{
    [Required(ErrorMessage = "Please enter a name for the room")]
    public string RoomName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int MaxVotes { get; set; } = 1;

    [Required(ErrorMessage = "Please enter your name")]
    public string AdminName { get; set; } = string.Empty;

    [Required]
    public VoterType AdminVoterType { get; set; }
}
