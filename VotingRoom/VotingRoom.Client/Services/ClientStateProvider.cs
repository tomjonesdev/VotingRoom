using VotingRoom.Common.Models;

namespace VotingRoom.Client.Services;

public class ClientStateProvider
{
    public Voter? CurrentUser { get; set; }

    public Room? Room { get; set; }
}
