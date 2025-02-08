namespace VotingRoom.Common.Models;

public class CastVoteResponse
{
    public required string VoterId { get; set; }

    public required string VoterName { get; set; }

    public required int VoteItemId { get; set; }
}
