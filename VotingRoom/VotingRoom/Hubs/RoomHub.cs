using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class RoomHub(IMemoryCache memoryCache) : Hub
{
    public async Task CreateRoom(RoomCreateRequest request)
    {
        var initialMember = new Voter
        {
            Id = Context.ConnectionId,
            Name = request.AdminName,
            Type = request.AdminVoterType,
            RemainingVotes = request.MaxVotes,
            IsAdmin = true,
        };

        var room = new Room
        {
            Id = Guid.NewGuid().ToString(),
            Name = string.IsNullOrWhiteSpace(request.RoomName) ? "Your room" : request.RoomName,
            AdminConnectionId = initialMember.Id,
            Voters = [initialMember],
            MaxVotes = request.MaxVotes,
        };

        var cacheKey = "room:" + room.Id;

        memoryCache.Set(cacheKey, room, new MemoryCacheEntryOptions
        {
            /* TODO: Reduce expiration time span
             * Can be safely reduced if the same cache entry is used for voting
             * Optionally set a PostEvictionCallbacks to trigger a warning to room members that the room should be recreated
             */
            SlidingExpiration = TimeSpan.FromMinutes(60),
        });

        await Groups.AddToGroupAsync(initialMember.Id, room.Id);
        await Clients.Client(initialMember.Id).SendAsync("CreateRoom", room, initialMember);
        //await Clients.Group(room.Id).SendAsync("UserJoined", room);
    }

    public async Task JoinRoom(
        Voter voter,
        string roomId)
    {
        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (!cached)
        {
            throw new Exception("Room not found");
        }

        voter.Id = Context.ConnectionId;
        voter.RemainingVotes = room.MaxVotes;
        room?.Voters.Add(voter);

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

    public async Task SendVote(Vote vote)
    {
        await Clients.Group(vote.RoomId.ToString()).SendAsync("ReceiveVote", vote);
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
