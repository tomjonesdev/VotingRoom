using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using VotingRoom.Common.Models;

namespace VotingRoom.Client.Services;

public class VotingService(NavigationManager navigationManager) : IVotingService
{
    private HubConnection _hubConnection;

    public event Action<Vote>? OnVoteReceived;
    public event Action<Room>? OnUserJoined;
    public event Action<Voter>? OnUpdateCurrentUser;
    public event Action<string>? OnUserLeft;
    public event Action? OnRevealVotes;
    public event Action? OnResetVotes;

    public async Task Connect(string roomId)
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

        // Register incoming SignalR event handlers
        _hubConnection.On<Vote>("ReceiveVote", vote => OnVoteReceived?.Invoke(vote));
        _hubConnection.On<Room>("UserJoined", (room) => OnUserJoined?.Invoke(room));
        _hubConnection.On<Voter>("UpdateUser", (voter) => OnUpdateCurrentUser?.Invoke(voter));
        _hubConnection.On<string>("UserLeft", name => OnUserLeft?.Invoke(name));
        _hubConnection.On("RevealVotes", () => OnRevealVotes?.Invoke());
        _hubConnection.On("ResetVotes", () => OnResetVotes?.Invoke());

        await _hubConnection.StartAsync();
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

    public async Task LeaveRoom(Guid roomId, string voterId)
    {
        await _hubConnection.InvokeAsync("LeaveRoom", roomId, voterId);
    }

    public async Task Disconnect()
    {
        if (_hubConnection != null)
            await _hubConnection.StopAsync();
    }
}
