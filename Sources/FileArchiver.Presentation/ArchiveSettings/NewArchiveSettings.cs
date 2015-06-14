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

using FileArchiver.Core;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Presentation.ArchiveSettings
{
	public class NewArchiveSettings
	{
		public NewArchiveSettings(Path destinationPath, IEnumerable<ArchiveFormatWithSettings> archiveSettings)
		{
			Contract.Requires(destinationPath != null);
			Contract.Requires(archiveSettings != null);
			Contract.Requires(Contract.ForAll(archiveSettings, x => x != null));

			DestinationPath = destinationPath;
			ArchiveSettings = archiveSettings.ToList();
		}

		public Path DestinationPath
		{
			get;
			private set;
		}

		public IEnumerable<ArchiveFormatWithSettings> ArchiveSettings
		{
			get;
			private set;
		}
	}
}
