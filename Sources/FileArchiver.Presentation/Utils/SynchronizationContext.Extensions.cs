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
using System.Threading;

namespace FileArchiver.Presentation.Utils
{
	internal static class SynchronizationContextExtensions
	{
		/// <summary>
		/// Shortcut for context.Send(x => action(), null);
		/// </summary>
		public static void Send(this SynchronizationContext context, Action action)
		{
			context.Send(x => action(), null);
		}

		/// <summary>
		/// A SynchronizationContext.Send() method with return value.
		/// </summary>
		/// <typeparam name="TResult">
		/// The type of the result.
		/// </typeparam>
		/// <param name="context">
		/// The synchronization context.
		/// </param>
		/// <param name="action">
		/// The action to execute.
		/// </param>
		/// <returns>
		/// The return value of passed action.
		/// </returns>
		public static TResult Send<TResult>(this SynchronizationContext context, Func<TResult> action)
		{
			var retVal = default(TResult);

			context.Send(x => retVal = action(), null);

			return retVal;
		}

		/// <summary>
		/// Shortcut for context.Post(x => action(), null);
		/// </summary>
		public static void Post(this SynchronizationContext context, Action action)
		{
			context.Post(x => action(), null);
		}
	}
}
