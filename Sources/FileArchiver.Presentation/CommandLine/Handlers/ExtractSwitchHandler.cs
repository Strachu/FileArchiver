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
using System.Threading.Tasks;
using FileArchiver.Core;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.CommandLine.Presenters;

namespace FileArchiver.Presentation.CommandLine.Handlers
{
	/// <summary>
	/// A command line handler for archive extraction.
	/// </summary>
	internal class ExtractSwitchHandler : GUICommandLineHandler
	{
		private readonly EntireArchivesExtractionPresenter mExtractionPresenter;

		public ExtractSwitchHandler(EntireArchivesExtractionPresenter extractionPresenter)
		{
			Contract.Requires(extractionPresenter != null);

			mExtractionPresenter = extractionPresenter;
		}

		private IEnumerable<Path> mFilesToExtract;

		protected override bool ParseArguments(string[] args)
		{
			if(!args.Any())
				return false;

			if(!String.Equals(args[0], "-e",        StringComparison.InvariantCultureIgnoreCase) &&
				!String.Equals(args[0], "--extract", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			mFilesToExtract = args.Skip(1).Select(path => new Path(path));
			return true;
		}

		protected override Task DoWork()
		{
			return mExtractionPresenter.ExtractArchives(mFilesToExtract.ToArray());
		}
	}
}
