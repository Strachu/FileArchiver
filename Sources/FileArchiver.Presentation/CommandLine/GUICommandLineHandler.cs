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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileArchiver.Presentation.CommandLine
{
	/// <summary>
	/// A base class for command line handlers with a graphical user interface.
	/// </summary>
	[ContractClass(typeof(GUICommandLineHandlerContractClass))]
	internal abstract class GUICommandLineHandler : ICommandLineHandler
	{
		public bool Handle(string[] args)
		{
			if(!ParseArguments(args))
				return false;

			Application.Idle += MessageLoopStarted;

			Application.Run();
			return true;
		}

		private async void MessageLoopStarted(object sender, EventArgs e)
		{
			Application.Idle -= MessageLoopStarted;

			await DoWork();

			Application.Exit();
		}

		/// <summary>
		/// Parses the arguments and indicates whether the handle can handle them.
		/// </summary>
		/// <param name="args">
		/// The arguments.
		/// </param>
		protected abstract bool ParseArguments(string[] args);

		/// <summary>
		/// Does the work of this handler.
		/// </summary>
		protected abstract Task DoWork();
	}
}
