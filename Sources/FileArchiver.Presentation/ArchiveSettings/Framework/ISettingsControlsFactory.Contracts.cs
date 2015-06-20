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
using System.Linq.Expressions;

namespace FileArchiver.Presentation.ArchiveSettings.Framework
{
	[ContractClassFor(typeof(ISettingsControlsFactory))]
	internal abstract class ISettingsControlsFactoryContractClass : ISettingsControlsFactory
	{
		ISettingsControl ISettingsControlsFactory.CreateGroup(string title,
			Expression<Func<IEnumerable<ISettingsControl>>> controlsProperty,
			Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(controlsProperty != null);

			throw new NotImplementedException();
		}

		ISettingsControl ISettingsControlsFactory.CreateTextField<TValue>(string title,
			Expression<Func<TValue>> valueProperty,
			Expression<Func<bool>> enabledProperty,
			Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			throw new NotImplementedException();
		}

		ISettingsControl ISettingsControlsFactory.CreateChoiceBox<TValue>(string title,
			Expression<Func<TValue>> valueProperty,
			Expression<Func<IEnumerable<TValue>>> availableValuesProperty,
			Expression<Func<bool>> enabledProperty,
			Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);
			Contract.Requires(availableValuesProperty != null);

			throw new NotImplementedException();
		}

		public ISettingsControl CreateCheckBox(string title,
		                                       Expression<Func<bool>> valueProperty,
		                                       Expression<Func<bool>> enabledProperty = null,
		                                       Expression<Func<bool>> visibleProperty = null)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			throw new NotImplementedException();
		}

		ISettingsControl ISettingsControlsFactory.CreateDestinationPathBrowser<TValue>(string title,
			Expression<Func<TValue>> valueProperty,
			Expression<Func<bool>> enabledProperty,
			Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			throw new NotImplementedException();
		}

		ISettingsControl ISettingsControlsFactory.CreateSourcePathBrowser<TValue>(string title,
			Expression<Func<TValue>> valueProperty,
			Expression<Func<bool>> enabledProperty,
			Expression<Func<bool>> visibleProperty)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(title));
			Contract.Requires(valueProperty != null);

			throw new NotImplementedException();
		}
	}
}
