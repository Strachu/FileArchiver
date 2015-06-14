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

namespace FileArchiver.Core.Utils.File
{
	/// <summary>
	/// Utility class whose purpose is to accumulate total write progress for multiple files.
	/// </summary>
	/// <remarks>
	/// This class opposite to Progress{T} does not marshal the execution of delegate to synchronization context.
	/// </remarks>
	public class CompositeFileProgress
	{
		private readonly IList<long>  mFileSizes = new List<long>();
		private readonly Action<long> mProgressDelegate;
		private readonly object       mLock = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeFileProgress"/> class.
		/// </summary>
		/// <param name="progressDelegate">
		/// The progress delegate which will be invoked when some of the files reports progress.
		/// Passed value = total bytes read / written at the moment of report for all files.
		/// </param>
		public CompositeFileProgress(Action<long> progressDelegate)
		{
			Contract.Requires(progressDelegate != null);

			mProgressDelegate = progressDelegate;
		}

		/// <summary>
		/// Gets the progress object for next file.
		/// </summary>
		/// <remarks>
		/// Calling the IProgress.Report() method will cause ProgressDelegate passed to this object
		/// to be called with total amount of bytes processed.
		/// </remarks>
		public IProgress<long> GetProgressForNextFile()
		{
			int nextFileIndex;

			lock(mLock)
			{
				nextFileIndex = mFileSizes.Count;
				mFileSizes.Add(0);
			}

			return new NonMarshallingProgress<long>(bytesProcessed => OnFileBytesProcessed(nextFileIndex, bytesProcessed));
		}

		private void OnFileBytesProcessed(int fileIndex, long bytesProcessed)
		{
			long totalBytesProcessed;

			lock(mLock)
			{
				mFileSizes[fileIndex] = bytesProcessed;

				totalBytesProcessed = mFileSizes.Sum();
			}

			mProgressDelegate(totalBytesProcessed);
		}
	}
}
