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

namespace FileArchiver.Core.Utils
{
	public enum RetryAction
	{
		Retry,
		Ignore,
		Abort,
		RethrowException
	}

	/// <summary>
	/// A helper for retry logic with the possibility of canceling, retrying or ignoring the error depending on the value returned from the strategy.
	/// </summary>
	public static class RetryLogicHelper
	{
		/// <summary>
		/// Tries to do specified action until it succeeds or is aborted.
		/// </summary>
		/// <param name="action">
		/// The action to execute.
		/// </param>
		/// <param name="strategy">
		/// The strategy deciding what to do when given exception throws.
		/// </param>
		/// <exception cref="System.OperationCanceledException">
		/// When strategy returned <see cref="RetryAction.Abort"/>
		/// </exception>
		/// <example>
		/// The usage of this function is similar to the usage of try...catch block:
		/// <code>
		/// RetryLogicHelper.Try(() =>
		/// {
		///	// Some action which can fail...
		/// },
		/// exception =>
		/// {
		///	if(exception is IOException)
		///   {
		///      return RetryAction.Retry;
		///   }
		///
		///   return RetryAction.RethrowException;
		/// }
		/// </code>
		/// </example>
		public static void Try(Action action, Func<Exception, RetryAction> strategy)
		{
			Contract.Requires(action != null);
			Contract.Requires(strategy != null);

			while(true)
			{
				try
				{
					action();
					return;
				}
				catch(Exception e)
				{
					var answer = strategy(e);

					if(answer == RetryAction.Ignore)
						return;

					if(answer == RetryAction.Abort)
						throw new OperationCanceledException();

					if(answer == RetryAction.RethrowException)
						throw;
				}
			}
		}
	}

	/// <summary>
	/// A version of <see cref="RetryLogicHelper"/> for use in recursive methods.
	/// </summary>
	/// <remarks>
	/// The difference between <see cref="RetryLogicHelper"/> and this class is that this class will continuously rethrow
	/// caught exception without again asking the strategy what to do till the recursive method is left, while <see cref="RetryLogicHelper"/>
	/// would ask the strategy what to do for every recursive call, annoying the user with the same question if the strategy ask the user what to do.
	/// <para>
	/// For the class to work correctly, it has to be passed as a parameter of the recursive method.
	/// </para>
	/// <example>
	/// <code>
	/// void SomeRecursiveMethod(string arg, RecursiveMethodRetryLogicHelper tryHelper)
	/// {
	///    tryHelper.Try(() =>
	///    {
	///       // Something
	///       SomeRecursiveMethod(arg, tryHelper);
	///    }
	///    exception =>
	///    {
	///       // Ask the user what to do or anything just want
	///    }
	/// }
	/// </code>
	/// </example>
	/// </remarks>
	public class RecursiveMethodRetryLogicHelper
	{
		private bool mExceptionThrown;

		public RecursiveMethodRetryLogicHelper()
		{
			mExceptionThrown = false;
		}

		public void Try(Action action, Func<Exception, RetryAction> strategy)
		{
			Contract.Requires(strategy != null);

			RetryLogicHelper.Try(action, exception =>
			{
				if(mExceptionThrown)
					return RetryAction.RethrowException;

				var answer = strategy(exception);

				if(answer == RetryAction.RethrowException || answer == RetryAction.Abort)
				{
					mExceptionThrown = true;
				}

				return answer;
			});
		}
	}
}
