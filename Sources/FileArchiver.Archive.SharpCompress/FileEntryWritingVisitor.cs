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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using FileArchiver.Core.Archive;
using FileArchiver.Core.DirectoryTraversing;
using FileArchiver.Core.Utils.File;

using SharpCompress.Writer;

namespace FileArchiver.Archive.SharpCompress
{
	internal class FileEntryWritingVisitor : DirectoryHierarchyVisitor<FileEntry>
	{
		private readonly IWriter               mWriter;
		private readonly CancellationToken     mCancelToken;
		private readonly CompositeFileProgress mProgress;

		public FileEntryWritingVisitor(IWriter writer, CancellationToken cancelToken, IProgress<long> progress = null)
		{
			Contract.Requires(writer != null);

			mWriter      = writer;
			mCancelToken = cancelToken;
			mProgress    = (progress != null) ? new CompositeFileProgress(progress.Report)
			                                  : new CompositeFileProgress(x => {});
		}

		public override void VisitFile(FileEntry file)
		{
			// TODO get file data from temp if its already there to skip decompression
			using(var entryStream = (file.DataFilePath != null) ? File.Open(file.DataFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
			                                                    : file.GetSharpCompressEntry().OpenEntryStream())
			{
				var progressReportingDataStream = new ProgressReportingStream(entryStream, mProgress.GetProgressForNextFile());
				var dataStream                  = new StreamWithCancellationSupport(progressReportingDataStream, mCancelToken);

				WriteEntry(file, dataStream);
			}
		}

		protected virtual void WriteEntry(FileEntry file, Stream dataStream)
		{
			Contract.Requires(file != null);
			Contract.Requires(dataStream != null);

			mWriter.Write(file.Path, dataStream, file.LastModificationTime);
		}

		public override bool OnDirectoryEntering(FileEntry directory)
		{
			// TODO SharpCompress does not support writing information about directories...
			return true;
		}
	}
}
