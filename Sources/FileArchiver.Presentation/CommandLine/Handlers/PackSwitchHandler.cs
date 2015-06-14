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

using FileArchiver.Core;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.CommandLine.Presenters;

namespace FileArchiver.Presentation.CommandLine.Handlers
{
	/// <summary>
	/// A command line handler for packing of files into an archive.
	/// </summary>
	internal class PackSwitchHandler : GUICommandLineHandler
	{
		private readonly PackIntoArchivePresenter mPackingPresenter;

		public PackSwitchHandler(PackIntoArchivePresenter packingPresenter)
		{
			Contract.Requires(packingPresenter != null);

			mPackingPresenter = packingPresenter;
		}

		private IEnumerable<Path> mFilesToPack;

		protected override bool ParseArguments(string[] args)
		{
			if(!args.Any())
				return false;

			if(!String.Equals(args[0], "-p",     StringComparison.InvariantCultureIgnoreCase) &&
				!String.Equals(args[0], "--pack", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			mFilesToPack = args.Skip(1).Select(arg => new Path(arg));
			return true;
		}

		protected override Task DoWork()
		{
			return mPackingPresenter.PackFiles(mFilesToPack.ToArray());
		}
	}
}
