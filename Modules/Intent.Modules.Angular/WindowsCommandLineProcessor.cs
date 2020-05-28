using System.Diagnostics;

namespace Intent.Modules.Bower.Installer
{
    public class WindowsCommandLineProcessor
    {
        public string ExecuteCommand(string workingDirectory, params string[] commands)
        {
            var cmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory
                }
            };
            cmd.Start();

            foreach (var command in commands)
            {
                cmd.StandardInput.WriteLine(command);
            }

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}