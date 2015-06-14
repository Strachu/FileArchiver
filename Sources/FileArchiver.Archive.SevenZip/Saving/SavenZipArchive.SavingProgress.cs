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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;

namespace FileArchiver.Archive.SevenZip
{
	public partial class SevenZipArchive
	{
		// An effort to make progress reporting during saving a bit more accurate by taking into account the relative
		// cost of every operation.
		private class SavingProgress
		{
			private const int EXTRACTION_COST_MULTIPLIER    = 4;
			private const int COMPRESSION_COST_MULTIPLIER   = 8;
			private const int RECOMPRESSION_COST_MULTIPLIER = 10;

			private readonly SevenZipArchive    mArchive;
			private readonly IProgress<double?> mTotalProgress;

			private long   mExtractionCost            = 0;
			private long   mRemovalCost               = 0;
			private long   mAdditionCost              = 0;

			private double mCurrentExtractionProgress = 0;
			private double mCurrentRemovingProgress   = 0;
			private double mCurrentAddingProgress     = 0;

			public SavingProgress(SevenZipArchive archive,
			                      IProgress<double?> savingProgress,
			                      IReadOnlyCollection<FileEntry> filesToAdd,
			                      IReadOnlyCollection<FileEntry> filesToRemove)
			{
				mArchive       = archive;
				mTotalProgress = savingProgress ?? new Progress<double?>();

				CalculateCost(filesToAdd, filesToRemove);

				InitProgressObjects(savingProgress);
			}

			private void CalculateCost(IReadOnlyCollection<FileEntry> filesToAdd,
			                           IReadOnlyCollection<FileEntry> filesToRemove)
			{
				var bytesToExtract = mArchive.GetTotalBytesToExtract(filesToAdd.ToList());
				var bytesInArchive = mArchive.mOriginalFiles.Sum(file => file.Size);
				var bytesToDelete  = filesToRemove.Sum(file => file.Size);

				if(filesToAdd.Any())
				{
					mExtractionCost = bytesToExtract * EXTRACTION_COST_MULTIPLIER;
					mAdditionCost   = bytesInArchive - bytesToDelete + bytesToExtract * COMPRESSION_COST_MULTIPLIER;
				}

				if(filesToRemove.Any())
				{
					mRemovalCost = bytesInArchive - bytesToDelete;

					if(mArchive.mIsSolid)
					{
						var filesToRemoveFlattenedList = filesToRemove.Flatten().Where(x => !x.IsDirectory).ToList();
						var bytesInModifiedSolidBlocks = mArchive.GetAllFilesInSolidBlocksOf(filesToRemoveFlattenedList)
						                                         .Sum(x => x.Size);

						mRemovalCost += (bytesInModifiedSolidBlocks - bytesToDelete) * RECOMPRESSION_COST_MULTIPLIER;
					}
				}
			}

			private void InitProgressObjects(IProgress<double?> savingProgress)
			{
				var totalCost     = mExtractionCost + mAdditionCost + mRemovalCost;

				var extractWeight = (double)mExtractionCost / totalCost;
				var addWeight     = (double)mAdditionCost   / totalCost;
				var deleteWeight  = (double)mRemovalCost    / totalCost;

				ExtractionProgress = new Progress<double?>(percentage =>
				{
					mCurrentExtractionProgress = percentage.Value * extractWeight;

					ReportCurrentProgress();
				});

				RemovalProgress = new Progress<double>(percentage =>
				{
					mCurrentRemovingProgress = percentage * deleteWeight;

					ReportCurrentProgress();
				});

				AdditionProgress = new Progress<double>(percentage =>
				{
					mCurrentAddingProgress = percentage * addWeight;

					ReportCurrentProgress();
				});
			}

			private void ReportCurrentProgress()
			{
				mTotalProgress.Report(mCurrentExtractionProgress + mCurrentRemovingProgress + mCurrentAddingProgress);
			}

			public IProgress<double?> ExtractionProgress
			{
				get;
				private set;
			}

			public IProgress<double> RemovalProgress
			{
				get;
				private set;
			}

			public IProgress<double> AdditionProgress
			{
				get;
				private set;
			}
		}
	}
}
