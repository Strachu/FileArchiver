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

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

using FileArchiver.Core.Utils;

using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.FileListView
{
	/// <summary>
	/// Custom BindingList for file entries with sorting support.
	/// </summary>
	internal class FileEntryBindingList : BindingList<FileEntryViewModel>
	{
		private bool               mIsSorted;
		private PropertyDescriptor mSortByProperty;
		private ListSortDirection  mSortDirection;

		public FileEntryBindingList()
		{
			mIsSorted       = false;
			mSortByProperty = TypeDescriptor.GetProperties(typeof(FileEntryViewModel)).Find(PropertyName.Of<FileEntryViewModel>(x => x.Name), true);
			mSortDirection  = ListSortDirection.Ascending;
		}

		protected override bool SupportsSortingCore
		{
			get { return true; }
		}

		protected override bool IsSortedCore
		{
			get { return mIsSorted; }
		}

		protected override void RemoveSortCore()
		{
			mIsSorted = false;
		}

		protected override ListSortDirection SortDirectionCore
		{
			get { return mSortDirection; }
		}

		protected override PropertyDescriptor SortPropertyCore
		{
			get { return mSortByProperty; }
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			mSortByProperty = property;
			mSortDirection  = direction;

			var itemsSortedByDirectory = base.Items.OrderByDescending(file => file.IsDirectory);

			// ToList() is necessary to create a copy because original list is cleared later
			var sortedItems = (direction == ListSortDirection.Ascending) ? itemsSortedByDirectory.ThenBy(property.GetValue).ToList()
			                                                             : itemsSortedByDirectory.ThenByDescending(property.GetValue).ToList();

			base.Items.Clear();
			sortedItems.CopyTo(base.Items);

			mIsSorted = true;

			ResetBindings();
		}

		public void Sort()
		{
			this.ApplySortCore(mSortByProperty, mSortDirection);
		}

		/// <summary>
		/// Finds the index at which the given entry should be inserted in order to maintain the list's current sort order.
		/// </summary>
		/// <param name="entry">
		/// The entry which will be inserted.
		/// </param>
		/// <remarks>
		/// This function assumes that the list is already sorted. 
		/// It will not work correctly if it is not the case.
		/// </remarks>
		public int FindOrderedIndex(FileEntryViewModel entry)
		{
			Contract.Requires(entry != null);

			var index = base.Items.FirstIndexOrDefault(entryAtCurrentIndex =>
			{
				return Compare(entry, entryAtCurrentIndex) < 0;
			});

			return index ?? base.Items.Count;
		}

		private int Compare(FileEntryViewModel first, FileEntryViewModel second)
		{
			if(first.IsDirectory && !second.IsDirectory)
				return -1;

			if(!first.IsDirectory && second.IsDirectory)
				return 1;

			var firstComparedValue  = mSortByProperty.GetValue(first);
			var secondComparedValue = mSortByProperty.GetValue(second);

			var order = Comparer<object>.Default.Compare(firstComparedValue, secondComparedValue);

			return (mSortDirection == ListSortDirection.Ascending) ? order : -order;
		}
	}
}
