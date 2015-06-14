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

namespace FileArchiver.Core.Utils.File
{
	/// <summary>
	/// Base class for stream decorators.
	/// </summary>
	public abstract class StreamDecorator : Stream
	{
		private readonly Stream mOriginalStream;

		protected StreamDecorator(Stream originalStream)
		{
			Contract.Requires(originalStream != null);

			mOriginalStream = originalStream;
		}

		public override bool CanRead
		{
			get { return mOriginalStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return mOriginalStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return mOriginalStream.CanWrite; }
		}

		public override void Flush()
		{
			mOriginalStream.Flush();
		}

		public override long Length
		{
			get { return mOriginalStream.Length; }
		}

		public override long Position
		{
			get
			{
				return mOriginalStream.Position;
			}

			set
			{
				mOriginalStream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return mOriginalStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return mOriginalStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			mOriginalStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			mOriginalStream.Write(buffer, offset, count);
		}
	}
}
