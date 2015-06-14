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

namespace FileArchiver.Presentation.CommandLine
{
	[ContractClass(typeof(ICommandLineHandlerContractClass))]
	public interface ICommandLineHandler
	{
		/// <summary>
		/// Handles the operation specified by given command line arguments.
		/// </summary>
		/// <param name="args">
		/// The arguments specified on command line.
		/// </param>
		/// <returns>
		/// Indicates whether the command was handled.
		/// Returning true causes the application to exit.
		/// </returns>
		bool Handle(string[] args);
	}
}
