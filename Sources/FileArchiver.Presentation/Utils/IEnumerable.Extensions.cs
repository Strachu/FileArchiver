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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace FileArchiver.Presentation.Utils
{
	internal static class IEnumerableExtensions
	{
		public static int FirstIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			var index = source.FirstIndexOrDefault(predicate);
			if(index == null)
			{
				throw new InvalidOperationException("Matching element has not been found.");
			}

			return index.Value;
		}

		public static int? FirstIndexOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			Contract.Requires(source != null);
			Contract.Requires(predicate != null);

			int index = 0;
			foreach(var item in source)
			{
				if(predicate(item))
					return index;

				++index;
			}

			return null;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			Contract.Requires(source != null);
			Contract.Requires(action != null);

			foreach(var item in source)
			{
				action(item);
			}
		}
	}
}
