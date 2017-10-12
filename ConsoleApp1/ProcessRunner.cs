using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPR.YardiRestoreTask
{
    public class ProcessRunner
    {
		private readonly string Executable;
		private readonly string Arguments;

		public ProcessRunner(string executableFileFullPath, string arguments)
		{

			if (string.IsNullOrEmpty(executableFileFullPath)) { throw new ArgumentNullException(nameof(executableFileFullPath)); }

			if (!File.Exists(executableFileFullPath)) { throw new FileNotFoundException($"{nameof(ProcessRunner)} could not find path: {executableFileFullPath}"); }

			this.Executable = executableFileFullPath;
			this.Arguments = arguments;
		}

		public bool Run()
		{
			TaskLogger.Log($"Running {this.Executable} {this.Arguments}");

			Process process = new Process();
			process.StartInfo.FileName = this.Executable;
			process.StartInfo.Arguments = this.Arguments;

			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			bool isSuccess = true;

			process.OutputDataReceived += (sender, args) => TaskLogger.Log(args.Data);
			process.ErrorDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					TaskLogger.Log(args.Data);
					isSuccess = false;
				}
			};

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			sw.Stop();

			TaskLogger.Log($"Process complete.  Duration: {sw.Elapsed}");


			if (isSuccess)
			{
				TaskLogger.Log($"Process completed successfully.");
			}
			else
			{
				TaskLogger.Log("Process failed");
			}

			return isSuccess;
		}
    }
}
