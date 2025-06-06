namespace VotingRoom.Common.Models;

public class Vote
{
    public required Voter Voter { get; set; }

    public required VoteItem Choice { get; set; }

    public required Guid RoomId { get; set; }
}
