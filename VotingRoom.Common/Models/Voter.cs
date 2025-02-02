using VotingRoom.Common.Enums;

namespace VotingRoom.Common.Models;

public class Voter : Member
{
    public VoterType Type { get; set; }
}
