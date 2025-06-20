using VotingRoom.Common.Models;

namespace VotingRoom.Client.Services;

public interface IVotingService
{
    event Action<Room, Voter>? OnRoomCreated;
    event Action<Voter>? OnUserJoined;
    event Action<Vote>? OnVoteReceived;
    event Action<Room, Voter>? OnUpdateCurrentUser;
    event Action<string>? OnUserLeft;
    event Action? OnRevealVotes;
    event Action? OnResetVotes;

    Task Connect();

    Task CreateRoom(RoomCreateRequest request);

    Task JoinRoom(
        Voter voter,
        Guid roomId);

    Task SendVote(Vote vote);


    Task RevealVotes(Guid roomId);

    Task ResetVotes(Guid roomId);
}
