using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using VotingRoom.Common.Models;
using VotingRoom.Models;

namespace VotingRoom.Hubs;

public class RoomHub(IMemoryCache memoryCache) : Hub
{
    public async Task Vote(string roomId, Voter voter, int voteItemId)
    {
        await Clients.Group(roomId).SendAsync("Vote", new CastVoteResponse
        {
            VoterId = voter.Id,
            VoterName = voter.Name,
            VoteItemId = voteItemId,
        });
    }

    public async Task JoinRoom(Voter voter, string roomId)
    {
        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (cached)
        {
            room?.Members.Add(voter);
        }
        else
        {
            room = new Room { Members = [voter] };
        }

        memoryCache.Set(cacheKey, room, new MemoryCacheEntryOptions
        {
            /* TODO: Reduce expiration time span
             * Can be safely reduced if the same cache entry is used for voting
             * Optionally set a PostEvictionCallbacks to trigger a warning to room members that the room should be recreated
             */
            SlidingExpiration = TimeSpan.FromMinutes(60),
        });

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("JoinRoom", room?.Members);
    }
}
