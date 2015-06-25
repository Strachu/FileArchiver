using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.FileListView;
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
		public void AfterSettingTheArchive_WhenArchiveSupportsOnlySingleFileAndItAlreadyContainsSomeFile_FileAddingIsDisabled()
		{
			A.CallTo(() => mArchiveMock.SupportsMultipleFiles).Returns(false);

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			Assert.That(mTestedModel.AddFilesEnabled, Is.False);
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
		public void WhenFileIsAddedToSubDirectory_SubDirectoryEntryIsUpdated()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.AddFile(new Path("Directory2/Directory1InDirectory2"), CreateFile("SomeFile"));

			var changedDirectoryViewModel = mTestedModel.FilesInCurrentDirectory.Single(x =>
			{
				return x.Name.Equals(new FileName("Directory1InDirectory2"));
			});

			Assert.That(changedDirectoryViewModel.FileCount, Is.EqualTo(3), "The directory was not updated");
		}

		[Test]
		public void WhenFileIsRemovedFromSubDirectory_SubDirectoryEntryIsUpdated()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mArchiveMock.RemoveFile(new Path("Directory2/Directory1InDirectory2/File1InNestedDirectory"));

			var changedDirectoryViewModel = mTestedModel.FilesInCurrentDirectory.Single(x =>
			{
				return x.Name.Equals(new FileName("Directory1InDirectory2"));
			});

			Assert.That(changedDirectoryViewModel.FileCount, Is.EqualTo(1), "The directory was not updated");
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
		public void WhenInvalidOperationExceptionIsThrownInAddFiles_ErrorOccuredEventIsRaised()
		{
			bool eventRaised = false;
			mTestedModel.ErrorOccured += (sender, e) => eventRaised = true;

			A.CallTo(() => mFileAddingService.AddFiles(null, null, null, null)).WithAnyArguments()
			                                                                   .Throws<InvalidOperationException>();


			mTestedModel.AddFiles(new Path[0]);

			Assert.That(eventRaised);
		}

		[Test]
		public void WhenArchiveDoesNotSupportMultipleFiles_FileIsAdded_FileAddingBecomesDisabled()
		{
			var emptyArchive = A.Fake<ArchiveBase>();
			A.CallTo(emptyArchive).CallsBaseMethod();
			A.CallTo(() => emptyArchive.SupportsMultipleFiles).Returns(false);

			mTestedModel.SetArchive(emptyArchive, new Path("C:\\archive.zip"));

			emptyArchive.AddFile(Path.Root, CreateFile("NewFile"));

			Assert.That(mTestedModel.AddFilesEnabled, Is.False);
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.AddFilesEnabled);
		}

		[Test]
		public void WhenArchiveDoesNotSupportMultipleFiles_FileIsRemoved_FileAddingBecomesEnabledAgain()
		{
			var emptyArchive = A.Fake<ArchiveBase>();
			A.CallTo(emptyArchive).CallsBaseMethod();
			A.CallTo(() => emptyArchive.SupportsMultipleFiles).Returns(false);

			mTestedModel.SetArchive(emptyArchive, new Path("C:\\archive.zip"));
			emptyArchive.AddFile(Path.Root, CreateFile("NewFile"));

			emptyArchive.RemoveFile(new Path("NewFile"));

			Assert.That(mTestedModel.AddFilesEnabled, Is.True);
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.AddFilesEnabled);
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
