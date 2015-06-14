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
using System.Threading;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	internal class FileAddingPerFileErrorPresenterFactory : IFileAddingPerFileErrorPresenterFactory
	{
		private readonly IPerFileErrorView      mView;
		private readonly IFileNameGenerator     mFileNameGenerator;
		private readonly SynchronizationContext mUIThread;

		public FileAddingPerFileErrorPresenterFactory(IPerFileErrorView view, IFileNameGenerator fileNameGenerator)
		{
			Contract.Requires(view != null);
			Contract.Requires(fileNameGenerator != null);

			mView              = view;
			mFileNameGenerator = fileNameGenerator;
			mUIThread          = SynchronizationContext.Current;
		}

		public FileAddingPerFileErrorPresenter GetErrorPresenterForNextOperation(IArchive archive)
		{
			return new FileAddingPerFileErrorPresenter(mView, archive, mFileNameGenerator, mUIThread);
		}
	}
}
