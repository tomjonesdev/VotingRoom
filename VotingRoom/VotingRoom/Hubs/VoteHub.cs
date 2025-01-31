using Microsoft.AspNetCore.SignalR;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class VoteHub : Hub
{
    public async Task Vote(Voter voter, int voteItemId)
    {
        await Clients.All.SendAsync("ReceiveVote", voter.Id, voter.Name, voteItemId);
    }
}
