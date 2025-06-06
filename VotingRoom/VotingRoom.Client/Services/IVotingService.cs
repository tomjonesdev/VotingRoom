using VotingRoom.Common.Models;

namespace VotingRoom.Client.Services;

public interface IVotingService
{
    event Action<Vote>? OnVoteReceived;
    event Action<Room>? OnUserJoined;
    event Action<Voter>? OnUpdateCurrentUser;
    event Action<string>? OnUserLeft;
    event Action? OnRevealVotes;
    event Action? OnResetVotes;

    Task Connect(string roomId);

    Task SendVote(Vote vote);

    Task JoinRoom(
        Voter voter,
        Guid roomId);

    Task RevealVotes(Guid roomId);

    Task ResetVotes(Guid roomId);

    Task LeaveRoom(
        Guid roomId,
        string voterId);

    Task Disconnect();
}
