using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace FileArchiver.Presentation.Tests.FileListView
{
	[TestFixture]
	internal class FileListViewModelFileAddingAndRemovingTest : FileListViewModelTestBase
	{
		[Test]
		public void WhenNoArchiveIsLoaded_FileAddingIsDisabled()
		{
			Assert.That(mTestedModel.AddFilesEnabled, Is.False);
		}

		[Test]
		public void AfterSettingTheArchive_FileAddingBecomesEnabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			Assert.That(mTestedModel.AddFilesEnabled, Is.True);
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.AddFilesEnabled);
		}

		[Test]
		public void WhenFileIsAddedToCurrentDirectory_FilesInCurrentDirectoryReflectsTheChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.AddFile(new Path("Directory2"), CreateFile("NewFile"));

			AssertFileListChangedTo("File1InDirectory2", "Directory1InDirectory2", "NewFile");
		}

		[Test]
		public void WhenFileIsAddedToDifferentDirectory_FilesInCurrentDirectoryDoesNotChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.AddFile(new Path("Directory2\\Directory1InDirectory2"), CreateFile("NewFile"));

			AssertFileListChangedTo("File1InDirectory2", "Directory1InDirectory2");
		}

		[Test]
		public void WhenFileIsAddedToRootDirectory_FilesInCurrentDirectoryReflectsTheChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			mArchiveMock.AddFile(Path.Root, CreateFile("NewFile"));

			AssertFileListChangedTo("NewFile", "File1", "File2", "File3", "Directory1", "Directory2", "Directory3");
		}

		[Test]
		public void WhenFileIsRemovedFromCurrentDirectory_FilesInCurrentDirectoryReflectsTheChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.RemoveFile(new Path("Directory2\\Directory1InDirectory2"));

			AssertFileListChangedTo("File1InDirectory2");
		}

		[Test]
		public void WhenFileIsRemovedFromDifferentDirectory_FilesInCurrentDirectoryDoesNotChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.RemoveFile(new Path("Directory2/Directory1InDirectory2\\File1InNestedDirectory"));

			AssertFileListChangedTo("File1InDirectory2", "Directory1InDirectory2");
		}

		[Test]
		public void WhenFileIsRemovedFromRootDirectory_FilesInCurrentDirectoryReflectsTheChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			mArchiveMock.RemoveFile(new Path("Directory2"));

			AssertFileListChangedTo("File1", "File2", "File3", "Directory1", "Directory3");
		}

		[Test]
		public void WhenDirectoryFileListIsUpdatedDirectly_FilesInCurrentDirectoryReflectsTheChange()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			var thisDirectory = mArchiveMock.GetFile(new Path("Directory2\\Directory1InDirectory2"));
			var newFile       = new FileEntry.Builder().WithName(new FileName("NewFile")).Build();

			thisDirectory     = thisDirectory.BuildCopy().WithoutFileNamed(new FileName("File1InNestedDirectory"))
			                                             .WithNewFile(newFile).Build();

			mArchiveMock.UpdateFile(thisDirectory);

			AssertFileListChangedTo("File2InNestedDirectory", "NewFile");
		}

		[Test]
		public void WhenDirectoryFileListIsUpdatedByModifyingItsAncestor_FilesInCurrentDirectoryReflectsTheChange()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			var parentDirectory = mArchiveMock.GetFile(new Path("Directory2"));
			var thisDirectory   = parentDirectory.Files.Single(x => x.Name.Equals(new FileName("Directory1InDirectory2")));
			var newFile         = new FileEntry.Builder().WithName(new FileName("NewFile")).Build();

			thisDirectory       = thisDirectory.BuildCopy().WithoutFileNamed(new FileName("File1InNestedDirectory"))
			                                               .WithNewFile(newFile).Build();

			parentDirectory     = parentDirectory.BuildCopy().WithoutFileNamed(thisDirectory.Name)
			                                                 .WithNewFile(thisDirectory).Build();

			mArchiveMock.UpdateFile(parentDirectory);

			AssertFileListChangedTo("File2InNestedDirectory", "NewFile");
		}

		[Test]
		public void WhenFileIsAddedToArchive_TheViewShouldScrollToIt()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.FirstDisplayedFileIndex = 5;

			mArchiveMock.AddFile(Path.Root, CreateFile("aaa"));

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(2));
		}

		[Test]
		public void WhenFileIsAddedToArchiveAtTheBeginning_TheViewShouldScrollBeforeFirstEntry()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.FirstDisplayedFileIndex = 5;

			mArchiveMock.AddFile(Path.Root, CreateFile("aaa", isDirectory: true));

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(0));
		}

		[Test]
		public void WhenFileIsAddedToArchive_ItIsSelected()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.AddFile(new Path("Directory2"), CreateFile("NewFile"));

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "NewFile");
		}

		[Test]
		public void WhenFileIsAddedToArchive_PreviousSelectionIsCleared()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.FilesInCurrentDirectory.ForEach(x => x.Selected = true);

			mArchiveMock.AddFile(new Path("Directory2"), CreateFile("NewFile"));

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "NewFile");
		}

		[Test]
		public void WhenFileIsAddedToArchive_TheFileListMaintainsSortOrder()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));

			mArchiveMock.AddFile(new Path("Directory1"), CreateFile("NewDirectory", isDirectory: true));

			Assert.That(mTestedModel.FilesInCurrentDirectory[0], IsFileNamed("Directory1InDirectory1"));
			Assert.That(mTestedModel.FilesInCurrentDirectory[1], IsFileNamed("NewDirectory"));
			Assert.That(mTestedModel.FilesInCurrentDirectory[2], IsFileNamed("File1InDirectory1"));
			Assert.That(mTestedModel.FilesInCurrentDirectory[3], IsFileNamed("File2InDirectory1"));
		}

		[Test]
		public void WhenFileIsRemovedAfterTheFirstVisible_FirstVisibleFileDoesNotChange()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FirstDisplayedFileIndex = 1;

			mArchiveMock.RemoveFile(new Path("Directory1\\File2InDirectory1"));

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(1));
		}

		[Test]
		public void WhenFileIsRemovedBeforeTheFirstVisible_TheIndexIsModifiedToPreventTheFirstVisibleFileFromChanging()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FirstDisplayedFileIndex = 1;

			mArchiveMock.RemoveFile(new Path("Directory1\\Directory1InDirectory1"));

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(0));
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.FirstDisplayedFileIndex);
		}

		[Test]
		public void WhenFirstFileIsTheFirstVisibleAndItIsRemoved_TheFirstVisibleIndexDoesNotChangeToPointToNextFile()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FirstDisplayedFileIndex = 0;

			mArchiveMock.RemoveFile(new Path("Directory1\\Directory1InDirectory1"));

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(0));
		}

		[Test]
		public void WhenFileIsRemoved_ItWasTheLastOneSelected_TheNextEntryIsSelected()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(new FileName("File1InDirectory1"))).Selected = true;

			mArchiveMock.RemoveFile(new Path("Directory1\\File1InDirectory1"));

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "File2InDirectory1");
		}

		[Test]
		public void WhenFileIsRemoved_ItWasTheLastOneSelectedAndLastOnList_ThePreviousEntryIsSelected()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(new FileName("File2InDirectory1"))).Selected = true;

			mArchiveMock.RemoveFile(new Path("Directory1\\File2InDirectory1"));

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "File1InDirectory1");
		}

		[Test]
		public void WhenLastFileIsRemoved_NoExceptionIsThrownDueToTryingToSelectNonExistingFile()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory3"));
			mTestedModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(new FileName("File1InDirectory3"))).Selected = true;

			mArchiveMock.RemoveFile(new Path("Directory3\\File1InDirectory3"));
			
			Assert.DoesNotThrow(() => mArchiveMock.RemoveFile(new Path("Directory3\\File2InDirectory3")));
		}
		
		[Test]
		public void WhenFileIsRemoved_SomeFilesAreStillSelected_TheSelectionDoesNotExpandToNewEntries()
		{			
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory1"));
			mTestedModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(new FileName("Directory1InDirectory1"))).Selected = true;

			mArchiveMock.RemoveFile(new Path("Directory1\\File1InDirectory1"));

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "Directory1InDirectory1");
		}

		[Test]
		public void WhenFilesAreAdded_TheyAreSelected()
		{
			A.CallTo(() => mFileAddingService.AddFiles(null, null, null, null)).WithAnyArguments().Invokes(x =>
			{
				var passedPaths = (IReadOnlyCollection<Path>)x.Arguments[2];

				passedPaths.ForEach(file => mArchiveMock.AddFile(new FileName("Directory2"), CreateFile(file.FileName)));
			});

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mTestedModel.AddFiles(new Path[]
			{
				new Path("C:\\file1.txt"),
				new Path("C:\\file2.txt"),
				new Path("C:\\file3.txt")
			});

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "file1.txt", "file2.txt", "file3.txt");
		}

		[Test]
		public void WhenFilesAreAddedButSomeOfThemAreRenamedDuringTheAddition_TheRenamedOnesAreSelectedToo()
		{
			A.CallTo(() => mFileAddingService.AddFiles(null, null, null, null)).WithAnyArguments().Invokes(x =>
			{
				mArchiveMock.AddFile(new FileName("Directory2"), CreateFile("file1.txt"));
				mArchiveMock.AddFile(new FileName("Directory2"), CreateFile("this file was renamed"));
				mArchiveMock.AddFile(new FileName("Directory2"), CreateFile("renamed too"));
			});

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mTestedModel.AddFiles(new Path[]
			{
				new Path("C:\\file1.txt"),
				new Path("C:\\file2.txt"),
				new Path("C:\\file3.txt")
			});

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel,
			                                                              "file1.txt",
			                                                              "this file was renamed",
			                                                              "renamed too");
		}

		[Test]
		public void WhenFilesAreAdded_TheViewScrollToTheOneWhichHasLowestIndex()
		{
			A.CallTo(() => mFileAddingService.AddFiles(null, null, null, null)).WithAnyArguments().Invokes(x =>
			{
				var passedPaths = (IReadOnlyCollection<Path>)x.Arguments[2];

				passedPaths.ForEach(file => mArchiveMock.AddFile(Path.Root, CreateFile(file.FileName)));
			});

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			mTestedModel.AddFiles(new Path[]
			{
				new Path("C:\\z last file.txt"),
				new Path("C:\\a first file.txt"),
				new Path("C:\\some file.txt")
			});

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(2));
		}

		[Test]
		public void WhenAllFilesAreSkipped_TheSelectionAndScrollDoesNotChange()
		{
			A.CallTo(() => mFileAddingService.AddFiles(null, null, null, null)).WithAnyArguments().DoesNothing();

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.FirstDisplayedFileIndex = 3;
			mTestedModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(new FileName("Directory2"))).Selected = true;

			mTestedModel.AddFiles(new Path[]
			{
				new Path("C:\\file1.txt"),
				new Path("C:\\file2.txt"),
				new Path("C:\\file3.txt")
			});

			Assert.That(mTestedModel.FirstDisplayedFileIndex, Is.EqualTo(3));
			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mTestedModel, "Directory2");
		}

		private void AssertFileListChangedTo(params string[] fileNames)
		{
			FileListViewModelTestUtil.AssertFileListIsSetTo(mTestedModel, fileNames);
		}
		
		private Constraint IsFileNamed(string fileName)
		{
			var propertyName = PropertyName.Of<FileEntryViewModel>(x => x.Name);

			return Has.Property(propertyName).EqualTo(new FileName(fileName));			
		}
	}
}
