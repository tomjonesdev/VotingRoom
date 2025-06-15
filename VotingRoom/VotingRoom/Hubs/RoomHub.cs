using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using VotingRoom.Common.Models;

namespace VotingRoom.Hubs;

public class RoomHub(IMemoryCache memoryCache) : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        // loop through all rooms and remove the user? Seems ridiculous
        // state tracking service suggestion: https://chatgpt.com/c/684ea6b8-7fbc-800f-8d97-17ef08803184
        return base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(RoomCreateRequest request)
    {
        var initialUser = new Voter
        {
            Id = Context.ConnectionId,
            Name = request.AdminName,
            Type = request.AdminVoterType,
            RemainingVotes = request.MaxVotes,
            IsAdmin = true,
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(request.RoomName) ? "Your room" : request.RoomName,
            AdminConnectionId = initialUser.Id,
            Voters = [initialUser],
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

        await Groups.AddToGroupAsync(initialUser.Id, room.Id.ToString());
        await Clients.Client(initialUser.Id).SendAsync("CreateRoom", room, initialUser);
    }

    public async Task JoinRoom(
        Voter newUser,
        string roomId)
    {
        var cacheKey = "room:" + roomId;
        var room = GetRoom(roomId);

        newUser.Id = Context.ConnectionId;
        newUser.RemainingVotes = room.MaxVotes;
        room?.Voters.Add(newUser);

        memoryCache.Set(cacheKey, room);

        await Groups.AddToGroupAsync(newUser.Id, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", newUser);
        await Clients.Client(newUser.Id).SendAsync("UpdateCurrentUser", room, newUser);
    }

    public async Task SendVote(Vote vote)
    {
        var room = GetRoom(vote.RoomId.ToString());
        room.VoteItems.First(vi => vi.Id == vote.Choice.Id).Voters.Add(vote.Voter);
        room.Voters.First(vi => vi.Id == vote.Voter.Id).RemainingVotes--;

        var cacheKey = "room:" + vote.RoomId;
        var cachedVoter = room.Voters.FirstOrDefault(v => v.Id == vote.Voter.Id);

        memoryCache.Set(cacheKey, room);

        await Clients.Group(vote.RoomId.ToString()).SendAsync("ReceiveVote", vote);
    }

    public async Task RevealVotes(string roomId)
    {
        var room = GetRoom(roomId);

        if (room is not null && Context.ConnectionId == room?.AdminConnectionId)
        {
            room.ShowVotes = true;

            var cacheKey = "room:" + roomId;
            memoryCache.Set(cacheKey, room);

            await Clients.Group(roomId).SendAsync("RevealVotes");
        }
    }

    public async Task ResetVotes(string roomId)
    {
        var room = GetRoom(roomId);

        if (room is not null && Context.ConnectionId == room?.AdminConnectionId)
        {
            room.VoteItems.ForEach(vi => vi.Voters.Clear());
            room.Voters.ForEach(v => v.RemainingVotes = room.MaxVotes);

            var cacheKey = "room:" + roomId;
            memoryCache.Set(cacheKey, room);

            await Clients.Group(roomId).SendAsync("ResetVotes");
        }
    }

    private Room GetRoom(string roomId)
    {
        var cacheKey = "room:" + roomId;
        var cached = memoryCache.TryGetValue<Room>(cacheKey, out var room);

        if (!cached || room is null)
        {
            throw new Exception("Room not found");
        }

        return room;
    }
}
