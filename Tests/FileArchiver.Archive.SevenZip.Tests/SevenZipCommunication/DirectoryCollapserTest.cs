using FileArchiver.Archive.SevenZip.SevenZipCommunication;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using NUnit.Framework;

namespace FileArchiver.Archive.SevenZip.Tests.SevenZipCommunication
{
	[TestFixture]
	class DirectoryCollapserTest
	{
		[Test]
		public void RootFileListIsReturnedAsIs()
		{
			var originalList = new FileEntry[]
			{
				CreateFile("File1"),
				CreateFile("File2"),
				CreateFile("Directory")
			};

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(originalList));
		}

		[Test]
		public void SingleDirectoryWithoutAllFilesIsNotCollapsed()
		{
			var file1 = CreateFile("File1");
			var file2 = CreateFile("File2");
			var file3 = CreateFile("File3");

			var originalList = new FileEntry[]
			{
				file1,
				file2,
			};

			var directory = CreateDirectory("Directory", file1, file2, file3);

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(originalList));
		}

		[Test]
		public void SingleDirectoryWithAllFilesIsCollapsed()
		{
			var originalList = new FileEntry[]
			{
				CreateFile("File1"),
				CreateFile("File2"),
				CreateFile("File3")
			};

			var directory = CreateDirectory("Directory", originalList);

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(new FileEntry[] { directory }));
		}

		[Test]
		public void WhenMultipleDirectoriesAreGivenOnlyThoseWithAllFilesAreCollapsed()
		{
			var file11 = CreateFile("File11");
			var file12 = CreateFile("File12");
			var file13 = CreateFile("File13");
			var file21 = CreateFile("File21");
			var file22 = CreateFile("File22");

			var originalList = new FileEntry[]
			{
				file11,
				file13,
				file21,
				file22
			};

			var directory  = CreateDirectory("Directory", file11, file12, file13);
			var directory2 = CreateDirectory("Directory2", file21, file22);

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(new FileEntry[] { file11, file13, directory2 }));
		}

		[Test]
		public void WhenMultiLevelHierarchyIsGivenWithAllFilesInsideTheListIsCollapsedToSingleDirectory()
		{
			var file11 = CreateFile("File11");
			var file12 = CreateFile("File12");
			var file13 = CreateFile("File13");
			var file21 = CreateFile("File21");
			var file22 = CreateFile("File22");

			var originalList = new FileEntry[]
			{
				file11,
				file12,
				file13,
				file21,
				file22
			};

			var directory    = CreateDirectory("Directory", file11, file12, file13);
			var directory2   = CreateDirectory("Directory2", file21, file22);
			var topDirectory = CreateDirectory("TopDirectory", directory, directory2);

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(new FileEntry[] { topDirectory }));
		}

		[Test]
		public void WhenMultiLevelHierarchyIsGivenWithOnlySomeDirectoriesFullTheListIsPartiallyCollapsed()
		{
			var file11 = CreateFile("File11");
			var file12 = CreateFile("File12");
			var file13 = CreateFile("File13");
			var file21 = CreateFile("File21");
			var file22 = CreateFile("File22");

			var originalList = new FileEntry[]
			{
				file11,
				file12,
				file13,
				file22
			};

			var directory    = CreateDirectory("Directory", file11, file12, file13);
			var directory2   = CreateDirectory("Directory2", file21, file22);
			var topDirectory = CreateDirectory("TopDirectory", directory, directory2);

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(new FileEntry[] { directory, file22 }));
		}

		[Test]
		public void CollapseWhenTheListIsAlreadyPartiallyCollapsed()
		{
			var file11 = CreateFile("File11");
			var file12 = CreateFile("File12");
			var file13 = CreateFile("File13");
			var file21 = CreateFile("File21");
			var file22 = CreateFile("File22");
			var file31 = CreateFile("File31");
			var file32 = CreateFile("File32");

			var directory    = CreateDirectory("Directory", file11, file12, file13);
			var directory2   = CreateDirectory("Directory2", file21, file22);
			var directory3   = CreateDirectory("Directory3", file31, file32);
			var topDirectory = CreateDirectory("TopDirectory", directory, directory2, directory3);

			var originalList = new FileEntry[]
			{
				file11,
				file12,
				file13,
				directory2,
				file31,
				file32
			};

			var collapsedList = DirectoryCollapser.Collapse(originalList);

			Assert.That(collapsedList, Is.EquivalentTo(new FileEntry[] { topDirectory }));
		}

		private FileEntry CreateFile(string name)
		{
			return new FileEntry.Builder().WithName(new FileName(name)).Build();
		}

		private FileEntry CreateDirectory(string name, params FileEntry[] files)
		{
			return new FileEntry.Builder().AsDirectory().WithName(new FileName(name)).WithFiles(files).Build();
		}
	}
}
