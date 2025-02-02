using Microsoft.AspNetCore.SignalR;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class RoomHub : Hub
{
    public async Task Vote(string roomId, Voter voter, int voteItemId)
    {
        await Clients.Group(roomId).SendAsync("ReceiveVote", new CastVoteResponse
        {
            VoterId = voter.Id,
            VoterName = voter.Name,
            VoteItemId = voteItemId,
        });
    }

    public async Task JoinRoom(Voter voter, string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("JoinRoom", voter);
    }
}
