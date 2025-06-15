using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using VotingRoom.Common.Models;

namespace VotingRoom.Client.Services;

public class VotingService(NavigationManager navigationManager) : IVotingService//, IDisposable
{
    private HubConnection _hubConnection;
    public event Action<Room, Voter>? OnRoomCreated;
    public event Action<Voter>? OnUserJoined;
    public event Action<Room, Voter>? OnUpdateCurrentUser;
    public event Action<Vote>? OnVoteReceived;
    public event Action? OnRevealVotes;
    public event Action? OnResetVotes;
    public event Action<string>? OnUserLeft;

    public async Task Connect()
    {
        if (_hubConnection != null &&
            _hubConnection.State == HubConnectionState.Connected)
        {
            return;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/roomHub"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<Room, Voter>("CreateRoom", (room, initialUser) => OnRoomCreated?.Invoke(room, initialUser));
        _hubConnection.On<Voter>("UserJoined", (voter) => OnUserJoined?.Invoke(voter));
        _hubConnection.On<Room, Voter>("UpdateCurrentUser", (room, voter) => OnUpdateCurrentUser?.Invoke(room, voter));
        _hubConnection.On<Vote>("ReceiveVote", vote => OnVoteReceived?.Invoke(vote));
        _hubConnection.On("RevealVotes", () => OnRevealVotes?.Invoke());
        _hubConnection.On("ResetVotes", () => OnResetVotes?.Invoke());
        _hubConnection.On<string>("UserLeft", name => OnUserLeft?.Invoke(name));

        await _hubConnection.StartAsync();
    }

    public async Task CreateRoom(RoomCreateRequest request)
    {
        await _hubConnection.InvokeAsync("CreateRoom", request);
    }

    public async Task JoinRoom(
        Voter voter,
        Guid roomId)
    {
        await _hubConnection.InvokeAsync("JoinRoom", voter, roomId);
    }

    public async Task SendVote(Vote vote)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendVote", vote);
        }
    }

    public async Task RevealVotes(Guid roomId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("RevealVotes", roomId);
        }
    }

    public async Task ResetVotes(Guid roomId)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("ResetVotes", roomId);
        }
    }

    //public async Task DisposeAsync()
    //{
    //    if (_hubConnection.State == HubConnectionState.Connected)
    //    {
    //        await _hubConnection.InvokeAsync("UserLeft", roomId, _hubConnection.ConnectionId);
    //    }
    //}
}
