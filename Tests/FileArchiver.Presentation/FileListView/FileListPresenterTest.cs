using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Services;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Utils;
using FileArchiver.Presentation.FileListView;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.FileListView
{
	internal class FileListPresenterTest
	{
		private IFileListView      mView;
		private IFileListViewModel mViewModel;
		private FileListPresenter  mTestedPresenter;

		[SetUp]
		public void SetUp()
		{
			mViewModel = new FileListViewModel(new GenericFileIconProvider(),
			                                   A.Fake<IFromFileSystemFileAddingService>(),
			                                   A.Fake<IFileAddingPerFileErrorPresenterFactory>());

			mViewModel.SetArchive(SetUpFakeArchive(), new Path("C:/archive.zip"));

			mView            = A.Fake<IFileListView>();
			mTestedPresenter = new FileListPresenter(mView, mViewModel, A.Fake<IProgressViewFactory>(),
			                                         A.Fake<FileOpeningService>());
		}

		private IArchive SetUpFakeArchive()
		{
			var archiveMock = A.Fake<ArchiveBase>();

			A.CallTo(archiveMock).CallsBaseMethod();

			archiveMock.AddFile(Path.Root, CreateFile("Directory1", isDirectory: true));
			archiveMock.AddFile(Path.Root, CreateFile("Directory2", isDirectory: true));
			archiveMock.AddFile(Path.Root, CreateFile("Directory3", isDirectory: true));
			archiveMock.AddFile(Path.Root, CreateFile("File1"));
			archiveMock.AddFile(Path.Root, CreateFile("File2"));
			archiveMock.AddFile(Path.Root, CreateFile("File3"));

			return archiveMock;
		}

		[Test]
		public void ValidatingEmptyName_ReturnsErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "   ";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Not.Empty);
		}

		[Test]
		public void ValidatingNameContainingForwardSlash_ReturnsErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "Directory/File";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Not.Empty);
		}

		[Test]
		public void ValidatingNameContainingBackSlash_ReturnsErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "Directory\\File";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Not.Empty);
		}

		[Test]
		public void ValidatingValidName_DoesNotReturnAnyErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "ValidFileName.Extension123456789.!@$%^&*().StillValid";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Empty);
		}

		[Test]
		public void ValidatingAlreadyExistingName_ReturnsErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "File1";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Not.Empty);
		}

		[Test]
		public void ValidatingWithoutChangingTheName_DoesNotReturnAnyErrorMessage()
		{
			var currentName = new FileName("File2");
			var newName     = "File2";
			var eventArgs   = new FileNameValidatingEventArgs(currentName, newName);

			mView.FileNameValidating += Raise.With(eventArgs);

			Assert.That(eventArgs.ErrorMessage, Is.Empty);
		}

		[Test]
		public void AcceptingTheName_RenamesTheFile()
		{
			var currentName = new FileName("File2");
			var newName     = new FileName("RenamedFile2");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;

			mView.FileNameAccepted += Raise.With(eventArgs);

			FileListViewModelTestUtil.AssertFileListIsSetTo(mViewModel, "File1", "RenamedFile2", "File3",
			                                                            "Directory1", "Directory2", "Directory3");
		}

		[Test]
		public void AcceptingTheNameWithoutChangingTheFileOrder_DoesNotChangeChangeScrollPosition()
		{
			var currentName = new FileName("File2");
			var newName     = new FileName("File222");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;
			mViewModel.FirstDisplayedFileIndex = 2;

			mView.FileNameAccepted += Raise.With(eventArgs);

			Assert.That(mViewModel.FirstDisplayedFileIndex, Is.EqualTo(2));
		}

		[Test]
		public void AcceptingTheNameCausingTheReorderingOfFiles_ScrollsToTheRenamedFileEnsuringItIsDisplayedInTheSamePosition()
		{
			var currentName = new FileName("File1");
			var newName     = new FileName("File4");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;
			mViewModel.FirstDisplayedFileIndex = 2;

			mView.FileNameAccepted += Raise.With(eventArgs);

			Assert.That(mViewModel.FirstDisplayedFileIndex, Is.EqualTo(4));
		}

		[Test]
		public void AcceptingTheNameCausingTheFileToBecomeFirstWhenTheFirstDisplayedFileIndexIsZero_ScrollsToFirstEntryInsteadOfReturningNegativeValue()
		{
			var currentName = new FileName("Directory3");
			var newName     = new FileName("Directory0");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;
			mViewModel.FirstDisplayedFileIndex = 0;

			mView.FileNameAccepted += Raise.With(eventArgs);

			Assert.That(mViewModel.FirstDisplayedFileIndex, Is.EqualTo(0));
		}

		[Test]
		public void RenamedFileIsSelected()
		{
			var currentName = new FileName("File2");
			var newName     = new FileName("RenamedFile2");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;

			mView.FileNameAccepted += Raise.With(eventArgs);

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mViewModel, newName);
		}

		[Test]
		public void RenamingFileRemovesSelectionFromOtherFiles()
		{
			var currentName = new FileName("File2");
			var newName     = new FileName("RenamedFile2");
			var eventArgs   = new FileNameAcceptedEventArgs(currentName, newName);

			// The view automatically updates the name in a view model when renaming due to data binding
			mViewModel.FilesInCurrentDirectory.Single(x => x.Name.Equals(currentName)).Name = newName;
			mViewModel.FilesInCurrentDirectory.ForEach(x => x.Selected = true);

			mView.FileNameAccepted += Raise.With(eventArgs);

			FileListViewModelTestUtil.AssertOnlyFollowingFilesAreSelected(mViewModel, newName);
		}

		private FileEntry CreateFile(string name, bool isDirectory = false)
		{
			var builder = new FileEntry.Builder().WithName(new FileName(name));

			if(isDirectory)
			{
				builder.AsDirectory();
			}

			return builder.Build();
		}
	}
}
