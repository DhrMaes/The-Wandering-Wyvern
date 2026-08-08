using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Microsoft.Playwright;

namespace DhrMaes.WanderingWyvern.Web.Tests;

public sealed class BrowserFixture
{
    private Process? _webProcess;

    public string BaseUrl { get; private set; } = string.Empty;

    public IPlaywright Playwright { get; private set; } = null!;

    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var repositoryRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(
            repositoryRoot,
            "WanderingWyvern.Web",
            "WanderingWyvern.Web.csproj");
        var port = GetFreePort();
        BaseUrl = $"http://127.0.0.1:{port}";

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--no-build");
        startInfo.ArgumentList.Add("--no-launch-profile");
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add(BaseUrl);

        _webProcess = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The Web host process could not be started.");

        await WaitForHostAsync();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
            await Browser.CloseAsync();

        Playwright?.Dispose();

        if (_webProcess is { HasExited: false })
            _webProcess.Kill(entireProcessTree: true);

        _webProcess?.Dispose();
    }

    private async Task WaitForHostAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };
        for (var attempt = 0; attempt < 60; attempt++)
        {
            if (_webProcess?.HasExited == true)
                throw new InvalidOperationException("The Web host exited before becoming ready.");

            try
            {
                using var response = await client.GetAsync(BaseUrl);
                if (response.StatusCode is >= HttpStatusCode.OK and < HttpStatusCode.MultipleChoices)
                    return;
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(250);
        }

        throw new TimeoutException($"The Web host did not become ready at {BaseUrl}.");
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "WanderingWyvern.slnx")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("The repository root could not be located.");
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
