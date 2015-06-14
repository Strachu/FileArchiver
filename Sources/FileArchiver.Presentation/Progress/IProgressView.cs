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
using System.Threading;

namespace FileArchiver.Presentation.Progress
{
	/// <summary>
	/// A view displaying the progress of single operation and giving the user ability to cancel it.
	/// </summary>
	[ContractClass(typeof(IProgressViewContractClass))]
	public interface IProgressView
	{
		/// <summary>
		/// An object which should be used to report the progress.
		/// </summary>
		/// <remarks>
		/// Value passed to progress.Report() should represent the percentage of operation already done.
		/// Pass null to progress.Report() if the progress cannot be determined for current execution of the operation.
		/// </remarks>
		IProgress<double?> Progress { get; }

		/// <summary>
		/// An object which can be used to check whether the operation should be canceled.
		/// </summary>
		CancellationToken CancelToken { get; }

		void Hide();
	}
}
