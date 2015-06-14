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

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	/// <summary>
	/// A stream which exposes for reading only a part of a given stream.
	/// </summary>
	internal class PartialReadStream : Stream
	{
		private readonly Stream mFullStream;
		private readonly long   mStreamSize;
		private long            mBytesLeft;

		public PartialReadStream(Stream originalStream, long streamSize)
		{
			Contract.Requires(originalStream != null);
			Contract.Requires(streamSize >= 0);

			mFullStream = originalStream;
			mStreamSize = streamSize;
			mBytesLeft  = streamSize;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			count = (int)Math.Min(count, mBytesLeft);
			if(count == 0)
				return 0;

			int readBytes = mFullStream.Read(buffer, offset, count);

			mBytesLeft -= readBytes;

			return readBytes;
		}

		public override long Length
		{
			get { return mBytesLeft; }
		}

		public override long Position
		{
			get { return mStreamSize - mBytesLeft; }
			set { throw new NotSupportedException(); }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
