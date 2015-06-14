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

using FileArchiver.Core.Utils;

namespace FileArchiver.Core.DirectoryTraversing
{
	/// <summary>
	/// A visitor to be used together with a <see cref="DirectoryHierarchyTraverser{T}"/> which will be notified about visited files and directories
	/// as the traversing proceeds.
	/// </summary>
	/// <typeparam name="T">
	/// The type of entry.
	/// </typeparam>
	[ContractClass(typeof(DirectoryHierarchyVisitorContractClass<>))]
	public abstract class DirectoryHierarchyVisitor<T>
	{
		public abstract void VisitFile(T file);

		/// <summary>
		/// Called just before entering a directory.
		/// </summary>
		/// <param name="directory">
		/// The directory to which the traverser is about to enter.
		/// </param>
		/// <returns>
		/// True if the traverser should enter the directory or false if it should be skipped.
		/// </returns>
		/// <remarks>
		/// The default implementation does nothing and allows the traverser to enter every directory.
		/// </remarks>
		public virtual bool OnDirectoryEntering(T directory)
		{
			Contract.Requires(directory != null);

			return true;
		}

		/// <summary>
		/// Called after the directory and all its files had been visited.
		/// </summary>
		/// <param name="directory">
		/// The directory which is about to be left.
		/// </param>
		/// <remarks>
		/// The default implementation does nothing.
		/// </remarks>
		public virtual void OnDirectoryLeaving(T directory)
		{
			Contract.Requires(directory != null);
		}

		/// <summary>
		/// A hook which allows the visitor to handle exceptions occurred during traversing without stopping it.
		/// </summary>
		/// <param name="entry">
		/// The last visited entry before the exception has been thrown.
		/// </param>
		/// <param name="exception">
		/// The exception which has been thrown.
		/// </param>
		/// <returns>
		/// The action determining whether the traversing should continue.
		/// </returns>
		/// <remarks>
		/// The default implementation stops further traversing by rethrowing the exception.
		/// </remarks>
		public virtual RetryAction OnException(T entry, Exception exception)
		{
			Contract.Requires(entry != null);
			Contract.Requires(exception != null);

			return RetryAction.RethrowException;
		}
	}
}
