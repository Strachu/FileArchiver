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

namespace FileArchiver.Core.Utils
{
	public static class IEnumerableExtensions
	{
		public static void CopyTo<T>(this IEnumerable<T> source, ICollection<T> destination)
		{
			Contract.Requires(source != null);
			Contract.Requires(destination != null);

			foreach(var entry in source)
			{
				destination.Add(entry);
			}
		}

		public static IEnumerable<T> WithoutLastElement<T>(this IEnumerable<T> collection)
		{
			Contract.Requires(collection != null);

			return collection.Reverse().Skip(1).Reverse();
		}

		public static bool SequenceEqualIgnoringOrder<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			Contract.Requires(first != null);
			Contract.Requires(second != null);

			var firstList  = first.ToList();
			var secondList = second.ToList();

			if(firstList.Count != secondList.Count)
				return false;

			return firstList.All(secondList.Contains);
		}
	}
}
