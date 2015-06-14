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

using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;

namespace FileArchiver.Core.Utils.File
{
	/// <summary>
	/// Decorator which adds cancellation support for reading and writing to streams.
	/// </summary>
	public class StreamWithCancellationSupport : StreamDecorator
	{
		private CancellationToken mCancelToken;

		public StreamWithCancellationSupport(Stream originalStream, CancellationToken cancelToken) : base(originalStream)
		{
			Contract.Requires(originalStream != null);

			mCancelToken = cancelToken;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			mCancelToken.ThrowIfCancellationRequested();

			return base.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			mCancelToken.ThrowIfCancellationRequested();

			return base.Seek(offset, origin);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			mCancelToken.ThrowIfCancellationRequested();

			base.Write(buffer, offset, count);
		}
	}
}
