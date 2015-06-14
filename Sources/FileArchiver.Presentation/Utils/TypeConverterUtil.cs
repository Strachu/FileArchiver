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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Presentation.Utils
{
	public static class TypeConverterUtil
	{
		/// <summary>
		/// Converts given argument to specified type using type converters.
		/// </summary>
		/// <typeparam name="TSource">
		/// The type of the source.
		/// </typeparam>
		/// <typeparam name="TDestination">
		/// The type to convert to.
		/// </typeparam>
		/// <param name="source">
		/// The value to convert.
		/// </param>
		/// <exception cref="NoConversionPossibleException">
		/// The conversion of given value to destination type is not possible.
		/// </exception>
		/// <returns>
		/// The value converted to destination type.
		/// </returns>
		public static TDestination Convert<TSource, TDestination>(TSource source)
		{
			Contract.Requires(source != null);

			var sourceConverter = TypeDescriptor.GetConverter(typeof(TSource));
			if(sourceConverter.CanConvertTo(typeof(TDestination)))
				return (TDestination)sourceConverter.ConvertTo(source, typeof(TDestination));

			var destinationConverter = TypeDescriptor.GetConverter(typeof(TDestination));
			if(destinationConverter.CanConvertFrom(typeof(TSource)))
				return (TDestination)destinationConverter.ConvertFrom(source);

			throw new NoConversionPossibleException(
				String.Format("Cannot convert from {0} to {1} because there is no TypeConverter allowing the conversion.",
				              typeof(TSource).FullName, typeof(TDestination).FullName));
		}
	}
}
