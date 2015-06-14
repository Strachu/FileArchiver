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
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Services;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Properties;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.CommandLine.Presenters
{
	/// <summary>
	/// A presenter for extraction of entire archives such as in the case of extracting from a file manager's context menu.
	/// </summary>
	internal class EntireArchivesExtractionPresenter
	{
		private readonly IEntireArchivesExtractionView           mView;
		private readonly IArchiveExtractionService               mExtractionService;
		private readonly IExtractionPerFileErrorPresenterFactory mPerFileErrorPresenterFactory;

		public EntireArchivesExtractionPresenter(IEntireArchivesExtractionView view,
		                                         IArchiveExtractionService extractionService,
		                                         IExtractionPerFileErrorPresenterFactory perFileErrorPresenterFactory)
		{
			Contract.Requires(view != null);
			Contract.Requires(extractionService != null);
			Contract.Requires(perFileErrorPresenterFactory != null);

			mView                         = view;
			mExtractionService            = extractionService;
			mPerFileErrorPresenterFactory = perFileErrorPresenterFactory;
		}

		/// <summary>
		/// Starts the asynchronous operation of archives extraction.
		/// </summary>
		/// <param name="archivePaths">
		/// The paths of archives to extract.
		/// </param>
		/// <remarks>
		/// The extracted files will be placed inside subdirectory of directory in which the archives are located.
		/// </remarks>
		public Task ExtractArchives(params Path[] archivePaths)
		{
			Contract.Requires(archivePaths != null);
			Contract.Requires(Contract.ForAll(archivePaths, path => path != null));
			Contract.Ensures(Contract.Result<Task>() != null);

			var tasks = new List<Task>();

			foreach(var file in archivePaths)
			{
				var task = ExtractArchive(file);

				tasks.Add(task);
			}

			return Task.WhenAll(tasks);
		}

		private async Task ExtractArchive(Path archivePath)
		{
			try
			{
				var progressView = mView.ShowProgressForNextExtraction(archivePath);
				try
				{
					var errorPresenter = mPerFileErrorPresenterFactory.GetErrorPresenterForNextOperation();

					await mExtractionService.ExtractArchiveAsync(archivePath, errorPresenter.ExceptionThrown,
					                                             progressView.CancelToken, progressView.Progress);
				}
				finally
				{
					progressView.Hide();
				}
			}
			catch(OperationCanceledException)
			{
				// Nothing
			}
			catch(NotSupportedFormatException)
			{
				mView.DisplayError(String.Format(Resources.NotSupportedFormatError, archivePath.FileName));
			}
			catch(IOException e)
			{
				mView.DisplayError(String.Format(Resources.ExtractError, e.Message));
			}
		}
	}
}
