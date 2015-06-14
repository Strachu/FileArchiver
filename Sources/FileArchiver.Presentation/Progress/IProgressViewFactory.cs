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

namespace FileArchiver.Presentation.Progress
{
	/// <summary>
	/// A factory for IProgressView objects.
	/// </summary>
	[ContractClass(typeof(IProgressViewFactoryContractClass))]
	public interface IProgressViewFactory
	{
		/// <summary>
		/// Shows a view which can used to show progress of an operation.
		/// </summary>
		/// <param name="operationTitle">
		/// Localized operation title for displaying purposes.
		/// </param>
		/// <param name="operationDescription">
		/// Localized operation description for displaying purposes.
		/// </param>
		/// <returns>
		/// The object which can be used to control the view.
		/// </returns>
		/// <remarks>
		/// The view has to be hidden by calling <see cref="IProgressView.Hide()"/> after the operation ends.
		/// Note that the displaying of the view CAN be deferred until the currently executing event handler ends.
		/// </remarks>
		IProgressView ShowProgressForNextOperation(string operationTitle, string operationDescription);
	}
}
