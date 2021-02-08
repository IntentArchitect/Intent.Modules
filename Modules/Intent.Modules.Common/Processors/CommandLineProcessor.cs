using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Intent.Utils;

namespace Intent.Modules.Common.Processors
{
    [Obsolete("Only works in windows environments")]
    public class CommandLineProcessor
    {
        public string ExecuteCommand(string workingDirectory, params string[] commands)
        {
            // TODO: Mac
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

            return cmd.StandardOutput.ReadToEnd();
        }
    }
}
