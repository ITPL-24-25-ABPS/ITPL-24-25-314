using System.Diagnostics;

namespace ServerManager.Servers;

public class ServerEntry
{
    public required string ServerName { get; set; }
    public required string Owner { get; set; }
    public required int Port { get; set; }
    public required List<string> Players { get; set; }
    public required bool IsOnline { get; set; }
    public required bool IsPrivate { get; set; }
    public string? Passcode { get; set; }

    private static readonly string ServerPath = "../win/PR.exe";
    private Process Process { get; set; }

    public ServerEntry()
    {

    }

    public void StartProcess()
    {
        Process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = ServerPath,
                Arguments = $"-port {Port}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(ServerPath)
            }
        };
        Process.Start();
        Console.WriteLine(Process.Id);
    }

    public void KillProcess()
    {
        Process.Kill();
    }
}