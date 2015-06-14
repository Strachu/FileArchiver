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
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

using SharpCompress.Writer.Tar;

namespace FileArchiver.Archive.Tar
{
	internal class FileEntryWritingVisitor : SharpCompress.FileEntryWritingVisitor
	{
		private readonly TarWriter mWriter;

		public FileEntryWritingVisitor(TarWriter writer, CancellationToken cancelToken, IProgress<long> progress = null)
			: base(writer, cancelToken, progress)
		{
			Contract.Requires(writer != null);

			mWriter = writer;
		}

		protected override void WriteEntry(FileEntry file, Stream dataStream)
		{
			mWriter.Write(file.Path, dataStream, file.LastModificationTime, file.Size);
		}
	}
}
