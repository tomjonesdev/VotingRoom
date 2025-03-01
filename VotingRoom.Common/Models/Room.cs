namespace VotingRoom.Common.Models;

public class Room
{
    public required string AdminConnectionId { get; set; }

    public List<Voter> Voters { get; set; } = [];

    public bool ShowVotes { get; set; }
}
