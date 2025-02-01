using Microsoft.AspNetCore.SignalR;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class VoteHub : Hub
{
    public override Task OnConnectedAsync()
    {
        // TODO: Implement for joining behaviour?
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // TODO: Implement for leaving behaviour?
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Vote(Voter voter, int voteItemId)
    {
        await Clients.All.SendAsync("ReceiveVote", voter.Id, voter.Name, voteItemId);
    }

    public async Task VoterJoin(Voter voter)
    {
        await Clients.All.SendAsync("VoterJoin", voter);
    }
}
