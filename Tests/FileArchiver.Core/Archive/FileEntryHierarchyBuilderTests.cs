using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Core.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Archive
{
	internal class FileEntryHierarchyBuilderTests
	{
		private FileEntryHierarchyBuilder mTestedBuilder;

		[SetUp]
		public void SetUp()
		{
			mTestedBuilder = new FileEntryHierarchyBuilder();
		}

		[Test]
		public void WhenNoFilesWereAdded_EmptyListIsReturned()
		{
			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList, Is.Empty);
		}

		[Test]
		public void WhenSingleFileWasAddedToRoot_ItIsReturned()
		{
			var fileToAdd = new FileEntry.Builder().WithName(new FileName("TestFile")).Build();

			mTestedBuilder.AddFile(Path.Root, fileToAdd);

			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList, Is.EquivalentTo(fileToAdd.ToSingleElementList()));
		}

		[Test]
		public void WhenDirectoryAndFileAreAdded_TheDirectoryWithFileIsReturned()
		{
			var directoryToAdd = new FileEntry.Builder().AsDirectory().WithName(new FileName("TestDirectory")).Build();
			var fileToAdd      = new FileEntry.Builder().WithName(new FileName("TestFile")).Build();

			mTestedBuilder.AddFile(Path.Root, directoryToAdd);
			mTestedBuilder.AddFile(directoryToAdd.Name, fileToAdd);

			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList,           Is.EquivalentTo(directoryToAdd.ToSingleElementList()));
			Assert.That(builtFileList.Flatten(), Is.EquivalentTo(new FileEntry[] { directoryToAdd, fileToAdd }));
		}

		[Test]
		public void WhenFileIsAddedToNonExistingDirectory_AllRequiredDirectoriesAreAutomaticallyCreated()
		{
			var fileToAdd = new FileEntry.Builder().WithName(new FileName("TestFile")).Build();

			mTestedBuilder.AddFile(new Path("Directory/SubDirectory"), fileToAdd);

			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList, new HasOnlyFiles("Directory", "Directory/SubDirectory",
			                                            "Directory/SubDirectory/TestFile"));
		}

		[Test]
		public void WhenFileIsAddedToNonExistingDirectoryAndLaterTheDirectoryIsAdded_OldDirectoryIsReplaced()
		{
			var directoryToAdd = new FileEntry.Builder().AsDirectory().WithName(new FileName("TestDirectory")).Build();
			var fileToAdd      = new FileEntry.Builder().WithName(new FileName("TestFile")).Build();

			mTestedBuilder.AddFile(new Path("TestDirectory"), fileToAdd);
			mTestedBuilder.AddFile(Path.Root, directoryToAdd);

			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList,           Is.EquivalentTo(directoryToAdd.ToSingleElementList()));
			Assert.That(builtFileList.Flatten(), Is.EquivalentTo(new FileEntry[] { directoryToAdd, fileToAdd }));
		}

		[Test]
		public void WhenMultipleFilesAndDirectoriesAreAdded_AllFilesAreReturned()
		{
			var directoryToAdd = new FileEntry.Builder().AsDirectory().WithName(new FileName("TestDirectory")).Build();
			var fileToAdd      = new FileEntry.Builder().WithName(new FileName("TestFile")).Build();
			var file2ToAdd     = new FileEntry.Builder().WithName(new FileName("TestFile2")).Build();
			var file3ToAdd     = new FileEntry.Builder().WithName(new FileName("TestFile3")).Build();

			mTestedBuilder.AddFile(new Path("TestDirectory"), fileToAdd);
			mTestedBuilder.AddFile(Path.Root, directoryToAdd);
			mTestedBuilder.AddFile(Path.Root, file2ToAdd);
			mTestedBuilder.AddFile(new Path("TestDirectory"), file3ToAdd);

			var builtFileList = mTestedBuilder.Build();

			Assert.That(builtFileList, new HasOnlyFiles("TestDirectory", "TestDirectory/TestFile",
			                                            "TestDirectory/TestFile3", "TestFile2"));
			Assert.That(builtFileList.Flatten(), Is.EquivalentTo(new FileEntry[] { directoryToAdd, fileToAdd, file2ToAdd, file3ToAdd }));
		}
	}
}
