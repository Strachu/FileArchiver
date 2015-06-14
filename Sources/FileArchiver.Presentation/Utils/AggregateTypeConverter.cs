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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.Presentation.Utils
{
	/// <summary>
	/// A TypeConverter which uses multiple child type converters to do conversion.
	/// </summary>
	/// <remarks>
	/// Only a single TypeConverter attribute is allowed for types.
	/// This class allows to use multiple TypeConverter to split the conversion of different types
	/// to different converters instead of doing every possible conversion in single class.
	/// </remarks>
	public abstract class AggregateTypeConverter : TypeConverter
	{
		private readonly IList<TypeConverter> mConverters = new List<TypeConverter>();

		protected void AddConverter(TypeConverter converter)
		{
			Contract.Requires(converter != null);

			mConverters.Add(converter);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return mConverters.Any(x => x.CanConvertFrom(context, sourceType));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var converterToUse = mConverters.Single(x => x.CanConvertFrom(context, value.GetType()));

			return converterToUse.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return mConverters.Any(x => x.CanConvertTo(context, destinationType));
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			var converterToUse = mConverters.Single(x => x.CanConvertTo(context, destinationType));

			return converterToUse.ConvertTo(context, culture, value, destinationType);
		}
	}
}
