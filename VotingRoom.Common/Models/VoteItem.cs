namespace VotingRoom.Common.Models;

public class VoteItem
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Voter> Voters { get; set; } = [];

    public int Votes => Voters.Count;
}
