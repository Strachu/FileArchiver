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
using System.Diagnostics.Contracts;
using System.IO;

namespace FileArchiver.Core.Utils.File
{
	/// <summary>
	/// A decorator which adds progress reporting support for reading and writing to streams.
	/// </summary>
	public class ProgressReportingStream : StreamDecorator
	{
		private long                     mTotalBytesProcessed = 0;
		private readonly IProgress<long> mProgress;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressReportingStream"/> class.
		/// </summary>
		/// <param name="originalStream">
		/// The original stream to add progress reporting to.
		/// </param>
		/// <param name="progress">
		/// The object which will be notified about the progress. Progress value = total bytes
		/// processed at the moment of report (read + written).
		/// </param>
		public ProgressReportingStream(Stream originalStream, IProgress<long> progress) : base(originalStream)
		{
			Contract.Requires(originalStream != null);

			mProgress = progress ?? new Progress<long>();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int bytesRead = base.Read(buffer, offset, count);

			mTotalBytesProcessed += bytesRead;

			mProgress.Report(mTotalBytesProcessed);

			return bytesRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			var previousPosition = base.Position;
			var newPosition      = base.Seek(offset, origin);

			mTotalBytesProcessed += newPosition - previousPosition;

			mProgress.Report(mTotalBytesProcessed);

			return newPosition;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			base.Write(buffer, offset, count);

			mTotalBytesProcessed += count;

			mProgress.Report(mTotalBytesProcessed);
		}
	}
}
