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
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Presentation
{
	// TODO Move container initialization from ApplicationBootstraper to this class??
	public class DependencyInjectionContainer
	{
		private readonly CompositionContainer mContainer;

		private DependencyInjectionContainer(CompositionContainer container)
		{
			Contract.Requires(container != null);

			mContainer = container;			
		}

		public static void Initialize(CompositionContainer container)
		{
			if(Instance != null)
				throw new InvalidOperationException("Container can be initialized only once.");

			Instance = new DependencyInjectionContainer(container);
		}

		public static DependencyInjectionContainer Instance { get; private set; }

		public T Get<T>()
		{
			return mContainer.GetExportedValue<T>();
		}
	}
}
