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
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.Core
{
	public static class IFromFileSystemFileAddingServiceExtensions
	{
		public static void AddFiles(this IFromFileSystemFileAddingService service,
		                            IArchive archive,
		                            Path destinationDirectory,
		                            IReadOnlyCollection<Path> pathsOnFileSystem)
		{
			Contract.Requires(service != null);

			service.AddFiles(archive, destinationDirectory, pathsOnFileSystem, NullFileErrorHandler);
		}

		private static RetryAction NullFileErrorHandler(ref Path destinationPath, Exception exception)
		{
			return RetryAction.RethrowException;
		}
	}
}
