using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class RoomHub(IMemoryCache memoryCache) : Hub
{
    public async Task SendVote(Vote vote)
    {
        await Clients.Group(vote.RoomId.ToString()).SendAsync("ReceiveVote", vote);
    }

    public async Task JoinRoom(Voter voter, string roomId)
    {
        voter.Id = Context.ConnectionId;

        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (cached)
        {
            room?.Voters.Add(voter);
        }
        else
        {
            room = new Room
            {
                AdminConnectionId = voter.Id,
                Voters = [voter],
            };

            voter.IsAdmin = true;
        }

        memoryCache.Set(cacheKey, room, new MemoryCacheEntryOptions
        {
            /* TODO: Reduce expiration time span
             * Can be safely reduced if the same cache entry is used for voting
             * Optionally set a PostEvictionCallbacks to trigger a warning to room members that the room should be recreated
             */
            SlidingExpiration = TimeSpan.FromMinutes(60),
        });

        await Groups.AddToGroupAsync(voter.Id, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", room);
        await Clients.Client(voter.Id).SendAsync("UpdateUser", voter);
    }

    public async Task LeaveRoom(Guid roomId, string voterId)
    {
        await Groups.RemoveFromGroupAsync(voterId, roomId.ToString());
        await Clients.Group(roomId.ToString()).SendAsync("LeaveRoom", voterId);

        // TODO: If the user is admin, pass the admin role to another user
    }

    public async Task RevealVotes(string roomId)
    {
        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (!cached || Context.ConnectionId == room?.AdminConnectionId)
        {
            await Clients.Group(roomId).SendAsync("RevealVotes");
        }
    }

    public async Task ResetVotes(string roomId)
    {
        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (!cached || Context.ConnectionId == room?.AdminConnectionId)
        {
            await Clients.Group(roomId).SendAsync("ResetVotes");
        }
    }
}
