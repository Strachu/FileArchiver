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

using FileArchiver.Presentation.Progress;

namespace FileArchiver.Presentation.CommandLine.Presenters
{
	[ContractClassFor(typeof(IEntireArchivesExtractionView))]
	internal abstract class IEntireArchivesExtractionViewContractClass : IEntireArchivesExtractionView
	{
		IProgressView IEntireArchivesExtractionView.ShowProgressForNextExtraction(string archivePath)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(archivePath));
			Contract.Ensures(Contract.Result<IProgressView>() != null);

			throw new NotImplementedException();
		}

		void IEntireArchivesExtractionView.DisplayError(string message)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(message));

			throw new NotImplementedException();
		}
	}
}
