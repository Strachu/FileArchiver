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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using FileArchiver.Core.Utils;

namespace FileArchiver.Core.DirectoryTraversing
{
	/// <summary>
	/// A class encapsulating the logic of traversing hierarchy in the form of directory structure.
	/// </summary>
	/// <typeparam name="T">
	/// The type of entries which will be traversed.
	/// </typeparam>
	[ContractClass(typeof(DirectoryHierarchyTraverserContractClass<>))]
	public abstract class DirectoryHierarchyTraverser<T>
	{
		private readonly RecursiveMethodRetryLogicHelper mRetryLogicHelper = new RecursiveMethodRetryLogicHelper();

		/// <summary>
		/// Starts the traversing of given directory.
		/// </summary>
		/// <param name="visitor">
		/// The visitor which will visit every file and directory as the traversing proceeds.
		/// </param>
		/// <param name="directoryToTraverse">
		/// The directory to traverse. It is allowed to pass a file here, but there will not be too much traversing.
		/// </param>
		public void Traverse(DirectoryHierarchyVisitor<T> visitor, T directoryToTraverse)
		{
			Contract.Requires(visitor != null);
			Contract.Requires(directoryToTraverse != null);

			mRetryLogicHelper.Try(() =>
			{
				if(IsDirectory(directoryToTraverse))
				{
					bool enter = visitor.OnDirectoryEntering(directoryToTraverse);
					if(!enter)
						return;

					foreach(var childEntry in GetFiles(directoryToTraverse))
					{
						Traverse(visitor, childEntry);
					}

					visitor.OnDirectoryLeaving(directoryToTraverse);
				}
				else
				{
					visitor.VisitFile(directoryToTraverse);
				}
			},
			exception =>
			{
				return visitor.OnException(directoryToTraverse, exception);
			});
		}

		/// <summary>
		/// Determines whether the specified entry is a directory.
		/// </summary>
		/// <param name="entry">
		/// The entry.
		/// </param>
		protected abstract bool IsDirectory(T entry);

		/// <summary>
		/// Gets the files belonging to the specified directory.
		/// </summary>
		/// <param name="directory">
		/// The directory.
		/// </param>
		protected abstract IEnumerable<T> GetFiles(T directory);
	}
}
