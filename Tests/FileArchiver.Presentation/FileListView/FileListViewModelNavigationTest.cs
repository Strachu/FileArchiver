using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FileArchiver.Core;
using FileArchiver.Core.ValueTypes;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.FileListView
{
	[TestFixture]
	internal class FileListViewModelNavigationTest : FileListViewModelTestBase
	{
		[Test]
		public void WhenNoArchiveIsLoaded_ThereAreNoFiles()
		{
			Assert.That(mTestedModel.CurrentDirectory,            Is.EqualTo(Path.Root));
			Assert.That(mTestedModel.CurrentDirectoryFullAddress, Is.EqualTo(Path.Root));
			Assert.That(mTestedModel.FilesInCurrentDirectory,     Is.Empty);
		}

		[Test]
		public void WhenNoArchiveIsLoaded_NavigateToParentDirectoryIsDisabled()
		{
			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.False);
		}

		[Test]
		public void AfterSettingTheArchive_DirectoryIsSetToRootDirectory()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			Assert.That(mTestedModel.CurrentDirectory, Is.EqualTo(Path.Root));
			AssertDirectoryAddressHasBeenSetTo("C:\\archive.zip");
			AssertFileListChangedTo("File1", "File2", "File3", "Directory1", "Directory2", "Directory3");
		}

		[Test]
		public void AfterSettingTheArchive_NavigateToParentDirectoryIsStillDisabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.False);
		}

		[Test]
		public void AfterOpeningADirectory_DirectoryIsChangedToIt()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			
			mTestedModel.Open(new FileName("Directory2"));

			AssertDirectoryHasBeenSetTo("Directory2");
			AssertDirectoryAddressHasBeenSetTo("C:\\archive.zip\\Directory2");
			AssertFileListChangedTo("File1InDirectory2", "Directory1InDirectory2");
		}

		[Test]
		public void AfterOpeningADirectory_NavigateToParentDirectoryBecomesEnabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));

			mTestedModel.Open(new FileName("Directory2"));

			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.True);
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.NavigateToParentDirectoryEnabled);
		}

		[Test]
		public void AfterOpeningASubDirectory_DirectoryIsChangedToIt()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			AssertDirectoryHasBeenSetTo("Directory2/Directory1InDirectory2");
			AssertDirectoryAddressHasBeenSetTo("C:\\archive.zip\\Directory2\\Directory1InDirectory2");
			AssertFileListChangedTo("File1InNestedDirectory", "File2InNestedDirectory");
		}

		[Test]
		public void AfterOpeningASubDirectory_NavigateToParentDirectoryIsStillEnabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.True);
		}

		[Test]
		public void AfterReturningToRootDirectory_DirectoryIsSetToRootDirectory()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mTestedModel.NavigateToParentDirectory();

			AssertDirectoryHasBeenSetTo(String.Empty);
			AssertDirectoryAddressHasBeenSetTo("C:\\archive.zip");
			AssertFileListChangedTo("File1", "File2", "File3", "Directory1", "Directory2", "Directory3");
		}

		[Test]
		public void AfterReturningToRootDirectory_NavigateToParentDirectoryBecomesDisabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mTestedModel.NavigateToParentDirectory();

			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.False);
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.NavigateToParentDirectoryEnabled);
		}

		[Test]
		public void AfterNavigatingToParentFromASubDirectory_DirectoryIsSetToParent()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			mTestedModel.NavigateToParentDirectory();

			AssertDirectoryHasBeenSetTo("Directory2");
			AssertDirectoryAddressHasBeenSetTo("C:\\archive.zip\\Directory2");
			AssertFileListChangedTo("File1InDirectory2", "Directory1InDirectory2");
		}

		[Test]
		public void AfterNavigatingToParentFromASubDirectory_NavigateToParentDirectoryIsStillEnabled()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			mTestedModel.NavigateToParentDirectory();

			Assert.That(mTestedModel.NavigateToParentDirectoryEnabled, Is.True);
		}

		[Test]
		public void AfterChangingCurrentArchive_WhenDirectoryWasSetToSubDirectory_CurrentDirectoryIsChangedToRoot()
		{
			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));
			mTestedModel.Open(new FileName("Directory1InDirectory2"));

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive2.zip"));

			AssertDirectoryHasBeenSetTo(String.Empty);
			AssertDirectoryAddressHasBeenSetTo("C:\\archive2.zip");
			AssertFileListChangedTo("File1", "File2", "File3", "Directory1", "Directory2", "Directory3");
		}

		[Test]
		public void WhenFileOpeningIsRequested_ViewModelRaisesEvent()
		{
			Path eventArgument = null;
			mTestedModel.FileOpeningRequested += (sender, e) => eventArgument = e.FileToOpen;

			mTestedModel.SetArchive(mArchiveMock, new Path("C:\\archive.zip"));
			mTestedModel.Open(new FileName("Directory2"));

			mTestedModel.Open(new FileName("File1InDirectory2"));

			Assert.That(eventArgument, Is.EqualTo(new Path("Directory2/File1InDirectory2")));
		}

		private void AssertDirectoryHasBeenSetTo(string directoryPath)
		{
			Assert.That(mTestedModel.CurrentDirectory, Is.EqualTo(new Path(directoryPath)));

			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.CurrentDirectory);						
		}

		private void AssertDirectoryAddressHasBeenSetTo(string directoryAddress)
		{
			Assert.That(mTestedModel.CurrentDirectoryFullAddress, Is.EqualTo(new Path(directoryAddress)));
	
			mPropertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedModel.CurrentDirectoryFullAddress);			
		}

		private void AssertFileListChangedTo(params string[] fileNames)
		{
			FileListViewModelTestUtil.AssertFileListIsSetTo(mTestedModel, fileNames);
		}
	}
}
