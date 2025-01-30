using Microsoft.AspNetCore.SignalR;

namespace VotingRoom.Hubs;

public class VoteHub : Hub
{
    public async Task Vote(string user, int vote)
    {
        await Clients.All.SendAsync("ReceiveVote", user, vote);
    }
}
