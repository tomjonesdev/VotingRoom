using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using VotingRoom.Client.Services;

namespace VotingRoom.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddScoped<IVotingService, VotingService>();
        builder.Services.AddSingleton<ClientStateProvider>();
        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }
}
