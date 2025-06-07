namespace VotingRoom.Common.Models;

public class Room
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string AdminConnectionId { get; set; } = string.Empty;

    public List<Voter> Voters { get; set; } = [];

    public int MaxVotes { get; set; } = 1;

    public bool ShowVotes { get; set; }
}
