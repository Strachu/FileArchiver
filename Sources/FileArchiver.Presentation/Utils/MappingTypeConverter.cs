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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace FileArchiver.Presentation.Utils
{
	/// <summary>
	/// A class with base implementation for TypeConverter doing a 1:1 value mapping between two types.
	/// </summary>
	/// <typeparam name="TSource">
	/// The type from which the conversion with happen.
	/// </typeparam>
	/// <typeparam name="TDestination">
	/// The type to which the conversion will be done.
	/// </typeparam>
	public abstract class MappingTypeConverter<TSource, TDestination> : TypeConverter
	{
		private readonly IDictionary<TSource, TDestination> mSourceToDestinationMapping = new Dictionary<TSource, TDestination>();
		private readonly IDictionary<TDestination, TSource> mDestinationToSourceMapping = new Dictionary<TDestination, TSource>();

		protected void AddMapping(TSource firstValue, TDestination secondValue)
		{
			Contract.Requires(firstValue  != null);
			Contract.Requires(secondValue != null);

			mSourceToDestinationMapping.Add(firstValue,  secondValue);
			mDestinationToSourceMapping.Add(secondValue, firstValue);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(TDestination);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return mDestinationToSourceMapping[(TDestination)value];
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(TDestination);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return mSourceToDestinationMapping[(TSource)value];
		}
	}
}
