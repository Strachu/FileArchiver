#region Copyright
/*
 * Copyright (C) 2015 Patryk Strach
 * 
 * This file is part of FileArchiver.
 * 
 * FileArchiver is free software: you can redistribute it and/or modify it under the terms of
 * the GNU Lesser General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * FileArchiver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with FileArchiver.
 * If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

using FileArchiver.Core.ValueTypes;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	/// <summary>
	/// A wrapper for invoking 7-zip application's process.
	/// </summary>
	internal class SevenZipProcess : IDisposable
	{
		private const int EXIT_CODE_SUCCESS = 0;

		private Process mProcess;

		private SevenZipProcess(Process process)
		{
			mProcess = process;
		}

		public static SevenZipProcess Execute(string parameters)
		{
			Contract.Requires(parameters != null);

			var processInfo = new ProcessStartInfo
			{
				FileName               = SevenZipBinaryLocation,
				Arguments              = parameters,
				UseShellExecute        = false,
				CreateNoWindow         = true,
				RedirectStandardInput  = true,
				RedirectStandardOutput = true,
				RedirectStandardError  = true
			};

			return new SevenZipProcess(Process.Start(processInfo));
		}

		private static Path SevenZipBinaryLocation
		{
			get
			{
				var assemblyLocation  = new Path(Assembly.GetExecutingAssembly().Location);
				var assemblyDirectory = assemblyLocation.ParentDirectory;

				return assemblyDirectory.Combine(new FileName("7za"));
			}
		}

		public StreamReader StandardError
		{
			get { return mProcess.StandardError; }
		}

		public StreamReader StandardOutput
		{
			get { return mProcess.StandardOutput; }
		}

		public bool ExitedWithoutError
		{
			get { return mProcess.ExitCode == EXIT_CODE_SUCCESS; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if(disposeManagedResources)
			{
				if(mProcess != null)
				{
					if(!mProcess.HasExited)
					{
						mProcess.Kill();
					}

					mProcess = null;
				}
			}
		}
	}
}
