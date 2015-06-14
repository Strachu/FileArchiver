using System.ComponentModel;
using System.Linq;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using NUnit.Framework;

using FileArchiver.Core;

using FileArchiver.Presentation.FileListView;

namespace FileArchiver.Presentation.Tests.FileListView
{
	internal class FileEntryBindingListTests
	{
		[Test]
		public void SortingByDefaultIsDoneByName()
		{
			var data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt"),
				CreateTestEntry("0123.doc"),
				CreateTestEntry("abc.exe"),
				CreateTestEntry("zew"),
				CreateTestEntry("longer name")
			};

			data.Sort();

			Assert.That(data, Is.Ordered.By("Name"));
		}

		[Test]
		public void SortingByFileNameWithoutDirectories()
		{
			IBindingList data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt"),
				CreateTestEntry("0123.doc"),
				CreateTestEntry("abc.exe"),
				CreateTestEntry("zew"),
				CreateTestEntry("longer name")
			};

			SortBy(data, "Name", ListSortDirection.Ascending);

			Assert.That(data, Is.Ordered.By("Name"));
		}

		[Test]
		public void ComparingByFileNameWithDirectories()
		{
			IBindingList data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt",         isDirectory: true),
				CreateTestEntry("0123.doc",        isDirectory: false),
				CreateTestEntry("xyzxyzxyzxyzxyz", isDirectory: false),
				CreateTestEntry("zew",             isDirectory: true),
				CreateTestEntry("abc",             isDirectory: false),
				CreateTestEntry("longer name",     isDirectory: true)
			};

			var expectedOrder = data.Cast<FileEntryViewModel>().OrderByDescending(file => file.IsDirectory)
			                                             .ThenBy(file => file.Name);

			SortBy(data, "Name", ListSortDirection.Ascending);

			Assert.That(data, Is.EqualTo(expectedOrder));
		}

		[Test]
		public void ComparingByFileSizeWithDirectories()
		{
			IBindingList data = new FileEntryBindingList
			{
				CreateTestEntry("0123.doc",        size:  10000, isDirectory: false),
				CreateTestEntry("abc.txt",         size:   1234, isDirectory: true),
				CreateTestEntry("xyzxyzxyzxyzxyz", size:     53, isDirectory: false),
				CreateTestEntry("zew",             size: 954235, isDirectory: true),
				CreateTestEntry("abc",             size:   7547, isDirectory: false),
				CreateTestEntry("longer name",     size:    100, isDirectory: true)
			};

			var expectedOrder = data.Cast<FileEntryViewModel>().OrderByDescending(file => file.IsDirectory)
			                                             .ThenBy(file => file.Size);

			SortBy(data, "Size", ListSortDirection.Ascending);

			Assert.That(data, Is.EqualTo(expectedOrder));
		}

		[Test]
		public void DirectoriesAreAlwaysListedFirstEvenAfterReversingTheSortOrder()
		{
			IBindingList data = new FileEntryBindingList
			{
				CreateTestEntry("0123.doc",        size:  10000, isDirectory: false),
				CreateTestEntry("abc.txt",         size:   1234, isDirectory: true),
				CreateTestEntry("xyzxyzxyzxyzxyz", size:     53, isDirectory: false),
				CreateTestEntry("zew",             size: 954235, isDirectory: true),
				CreateTestEntry("abc",             size:   7547, isDirectory: false),
				CreateTestEntry("longer name",     size:    100, isDirectory: true)
			};

			var expectedOrder = data.Cast<FileEntryViewModel>().OrderByDescending(file => file.IsDirectory)
			                                             .ThenByDescending(file => file.Size);

			SortBy(data, "Size", ListSortDirection.Descending);

			Assert.That(data, Is.EqualTo(expectedOrder));
		}

		[Test]
		public void WhenDefaultSortOrderIsUsed_TheCorrectIndexToRetainSortOrderAfterInsertingIsReturned()
		{
			var data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt",         isDirectory: true),
				CreateTestEntry("0123.doc",        isDirectory: false),
				CreateTestEntry("xyzxyzxyzxyzxyz", isDirectory: false),
				CreateTestEntry("zew",             isDirectory: true),
				CreateTestEntry("abc",             isDirectory: false),
				CreateTestEntry("longer name",     isDirectory: true)
			};

			data.Sort();

			var returnedIndex = data.FindOrderedIndex(CreateTestEntry("aaaaa.txt", isDirectory: false));

			Assert.That(returnedIndex, Is.EqualTo(4));
		}

		[Test]
		public void WhenSortingByFileSizeInReverseOrder_TheCorrectIndexToRetainSortOrderAfterInsertingIsReturned()
		{
			var data = new FileEntryBindingList
			{
				CreateTestEntry("0123.doc",        size:     53, isDirectory: false),
				CreateTestEntry("abc.txt",         size:   1234, isDirectory: true),
				CreateTestEntry("xyzxyzxyzxyzxyz", size:  10000, isDirectory: false),
				CreateTestEntry("SomeFile",        size:   1001, isDirectory: false),
				CreateTestEntry("zew",             size: 954235, isDirectory: true),
				CreateTestEntry("abc",             size:   7547, isDirectory: false),
				CreateTestEntry("longer name",     size:    100, isDirectory: true)
			};

			SortBy(data, "Size", ListSortDirection.Descending);

			var returnedIndex = data.FindOrderedIndex(CreateTestEntry("NewFile", size: 1000, isDirectory: false));

			Assert.That(returnedIndex, Is.EqualTo(6));
		}

		[Test]
		public void WhenNewEntryIsDirectoryToBePlacedAsTheLastDirectory_TheCorrectIndexIsNotHigherThanTheNumberOfDirectories()
		{
			var data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt",         isDirectory: true),
				CreateTestEntry("0123.doc",        isDirectory: false),
				CreateTestEntry("xyzxyzxyzxyzxyz", isDirectory: false),
				CreateTestEntry("zew",             isDirectory: true),
				CreateTestEntry("abc",             isDirectory: false),
				CreateTestEntry("longer name",     isDirectory: true)
			};

			data.Sort();

			var returnedIndex = data.FindOrderedIndex(CreateTestEntry("zzz", isDirectory: true));

			Assert.That(returnedIndex, Is.EqualTo(3));
		}

		[Test]
		public void WhenNewEntryIsToBePlacedAtTheEnd_TheFindOrderedIndexReturnsTheListSize()
		{
			var data = new FileEntryBindingList
			{
				CreateTestEntry("abc.txt",         isDirectory: true),
				CreateTestEntry("0123.doc",        isDirectory: false),
				CreateTestEntry("xyzxyzxyzxyzxyz", isDirectory: false),
				CreateTestEntry("zew",             isDirectory: true),
				CreateTestEntry("abc",             isDirectory: false),
				CreateTestEntry("longer name",     isDirectory: true)
			};

			data.Sort();

			var returnedIndex = data.FindOrderedIndex(CreateTestEntry("zzz", isDirectory: false));

			Assert.That(returnedIndex, Is.EqualTo(data.Count));
		}

		private FileEntryViewModel CreateTestEntry(string name, bool isDirectory = false, long size = 0)
		{
			var builder = new FileEntry.Builder();

			builder.WithName(new FileName(name));
			builder.WithSize(size);

			if(isDirectory)
			{
				builder.AsDirectory();
			}

			return new FileEntryViewModel(builder.Build(), new GenericFileIconProvider());
		}

		private void SortBy(IBindingList list, string propertyName, ListSortDirection direction)
		{
			var property = TypeDescriptor.GetProperties(typeof(FileEntryViewModel)).Find(propertyName, true);

			list.ApplySort(property, direction);
		}
	}
}
