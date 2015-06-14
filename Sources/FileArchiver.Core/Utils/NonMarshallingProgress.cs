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
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Core.Utils
{
	/// <summary>
	/// An IProgress{T} implementation which does not marshal the callback execution
	/// with the usage of SynchronizationContext.
	/// </summary>
	/// <remarks>
	/// Progress{T} causes problems when it is created outside of GUI thread because
	/// the callback is then executed on a thread pool causing the reports to come in incorrect order.
	/// </remarks>
	public class NonMarshallingProgress<T> : IProgress<T>
	{
		private readonly Action<T> mHandler;

		public NonMarshallingProgress(Action<T> handler)
		{
			Contract.Requires(handler != null);

			mHandler = handler;
		}

		public void Report(T value)
		{
			mHandler(value);
		}
	}
}
