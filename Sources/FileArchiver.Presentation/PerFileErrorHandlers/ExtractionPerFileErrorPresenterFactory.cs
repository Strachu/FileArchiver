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
using System.IO.Abstractions;
using System.Threading;

using FileArchiver.Core;
using FileArchiver.Core.Services;

namespace FileArchiver.Presentation.PerFileErrorHandlers
{
	internal class ExtractionPerFileErrorPresenterFactory : IExtractionPerFileErrorPresenterFactory
	{
		private readonly IPerFileErrorView      mView;
		private readonly IFileSystem            mFileSystem;
		private readonly IFileNameGenerator     mFileNameGenerator;
		private readonly SynchronizationContext mUIThread;

		public ExtractionPerFileErrorPresenterFactory(IPerFileErrorView view,
		                                              IFileSystem fileSystem,
		                                              IFileNameGenerator fileNameGenerator)
		{
			Contract.Requires(view != null);
			Contract.Requires(fileSystem != null);
			Contract.Requires(fileNameGenerator != null);

			mView              = view;
			mFileSystem        = fileSystem;
			mFileNameGenerator = fileNameGenerator;
			mUIThread          = SynchronizationContext.Current;
		}

		public ExtractionPerFileErrorPresenter GetErrorPresenterForNextOperation()
		{
			return new ExtractionPerFileErrorPresenter(mView, mFileSystem, mFileNameGenerator, mUIThread);
		}
	}
}
