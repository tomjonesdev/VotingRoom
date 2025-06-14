namespace VotingRoom.Common.Models;

public class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AdminConnectionId { get; set; } = string.Empty;

    public List<Voter> Voters { get; set; } = [];

    public List<VoteItem> VoteItems { get; set; } = [
        new() { Id = 1, Name = "1" },
        new() { Id = 2, Name = "2" },
        new() { Id = 3, Name = "3" },
        new() { Id = 4, Name = "5" },
        new() { Id = 5, Name = "8" },
        new() { Id = 6, Name = "13" },
        new() { Id = 7, Name = "21" }
    ];

    public int MaxVotes { get; set; } = 1;

    public bool ShowVotes { get; set; }
}
