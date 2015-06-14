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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils.File;

using SharpCompress.Common;
using SharpCompress.Writer;
using SharpCompress.Writer.Tar;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.Tar
{
	public class TarArchive : SharpCompress.ArchiveBase
	{
		public TarArchive(global::SharpCompress.Archive.Tar.TarArchive archive,
		                  Path archivePath,
		                  TempFileProvider tempFileProvider,
		                  CancellationToken cancelToken)
			: base(archive, archivePath, tempFileProvider, cancelToken)
		{
		}

		protected override void WriteEntries(Path destinationPath, CancellationToken cancelToken, CompositeFileProgress progress)
		{
			using(var destinationStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write))
			using(var writer            = InitializeWriter(destinationStream) as TarWriter)
			{
				foreach(var file in RootFiles)
				{
					var hierarchyTraverser = new FileEntryHierarchyTraverser();
					var writingVisitor     = new FileEntryWritingVisitor(writer, cancelToken, progress.GetProgressForNextFile());

					hierarchyTraverser.Traverse(writingVisitor, file);
				}
			}
		}

		protected override IWriter InitializeWriter(Stream destinationStream)
		{
			return new TarWriter(destinationStream, new CompressionInfo());
		}
	}
}
