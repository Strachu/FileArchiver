using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.Commands;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Progress;
using FileArchiver.Presentation.Utils;
using FileArchiver.TestUtils;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.Tests.Commands
{
	class SaveArchiveCommandTest
	{
		private IDialogLauncher      mDialogLauncher;
		private IProgressViewFactory mProgressViewFactory;
		private IArchive             mArchiveMock;
		private FileListViewModel    mFileListViewModel;

		private SaveArchiveCommand   mTestedCommand;

		[SetUp]
		public void SetUp()
		{
			CreateCommand();

			mFileListViewModel.SetArchive(mArchiveMock, new Path("ArchivePath"));
		}

		private void CreateCommand()
		{
			mDialogLauncher      = A.Fake<IDialogLauncher>();
			mProgressViewFactory = A.Fake<IProgressViewFactory>();
			mArchiveMock         = A.Fake<IArchive>();
			mFileListViewModel   = new FileListViewModel(A.Fake<IFileIconProvider>(),
			                                             A.Fake<IFromFileSystemFileAddingService>(),
			                                             A.Fake<IFileAddingPerFileErrorPresenterFactory>());

			mTestedCommand = new SaveArchiveCommand(mDialogLauncher, mProgressViewFactory, mFileListViewModel);		
		}

		[Test]
		public void CommandIsDisabledWhenNoArchiveIsLoaded()
		{
			CreateCommand();

			Assert.That(mTestedCommand.Enabled, Is.False);
		}

		[Test]
		public void CommandBecomesEnabledAfterArchiveLoading()
		{
			CreateCommand();
			var propertyChangedTester = new PropertyChangedTester(mTestedCommand);

			mFileListViewModel.SetArchive(mArchiveMock, new Path("ArchivePath"));

			Assert.That(mTestedCommand.Enabled, Is.True);
			propertyChangedTester.AssertPropertyChangedRaisedFor(() => mTestedCommand.Enabled);
		}

		[Test]
		public async void WhenArchiveIsNotModified_ExecuteIfRequiresDoesNotSaveArchive()
		{
			A.CallTo(() => mArchiveMock.IsModified).Returns(false);

			var returnValue = await mTestedCommand.ExecuteIfRequired();

			A.CallTo(mDialogLauncher).MustNotHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustNotHaveHappened();
			Assert.That(returnValue, Is.EqualTo(CancelOrContinue.Continue));
		}

		[Test]
		public async void WhenArchiveIsModifiedAndUserWantsToCancel_ExecuteIfRequiresDoesNotSaveArchiveAndTellsTheCallerToCancel()
		{
			A.CallTo(() => mArchiveMock.IsModified).Returns(true);
			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).Returns(SaveChangesAction.Cancel);

			var returnValue = await mTestedCommand.ExecuteIfRequired();

			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).MustHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustNotHaveHappened();
			Assert.That(returnValue, Is.EqualTo(CancelOrContinue.Cancel));
		}

		[Test]
		public async void WhenArchiveIsModifiedAndUserWantsToDiscardChanges_ExecuteIfRequiresDoesNotSaveArchiveAndTellsTheCallerToContinue()
		{
			A.CallTo(() => mArchiveMock.IsModified).Returns(true);
			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).Returns(SaveChangesAction.Discard);

			var returnValue = await mTestedCommand.ExecuteIfRequired();

			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).MustHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustNotHaveHappened();
			Assert.That(returnValue, Is.EqualTo(CancelOrContinue.Continue));
		}

		[Test]
		public async void WhenArchiveIsModifiedAndUserWantsToSave_ExecuteIfRequiresSavesArchiveAndTellsTheCallerToContinue()
		{
			A.CallTo(() => mArchiveMock.IsModified).Returns(true);
			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).Returns(SaveChangesAction.Save);

			var returnValue = await mTestedCommand.ExecuteIfRequired();

			A.CallTo(() => mDialogLauncher.AskForSaveChangesAction(A<FileName>.Ignored)).MustHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustHaveHappened();
			Assert.That(returnValue, Is.EqualTo(CancelOrContinue.Continue));
		}

		[Test]
		public async void DuringSaving_CommandShowsProgressViewAndPassesCorrectParametersToSaveMethod()
		{
			var progressObjectReturnedFromView = new Progress<double?>();
			var cancelTokenReturnedFromView    = new CancellationToken();
			var progressViewMock               = A.Fake<IProgressView>();

			A.CallTo(() => progressViewMock.CancelToken).Returns(cancelTokenReturnedFromView);
			A.CallTo(() => progressViewMock.Progress).Returns(progressObjectReturnedFromView);
			A.CallTo(() => mProgressViewFactory.ShowProgressForNextOperation(null, null)).WithAnyArguments()
			                                                                             .Returns(progressViewMock);

			await mTestedCommand.ExecuteAsync();

			A.CallTo(() => mArchiveMock.SaveAsync(cancelTokenReturnedFromView, progressObjectReturnedFromView)).MustHaveHappened();
		}

		[Test]
		public async void DuringSaving_CommandShowsProgressViewAndClosesItAfterSaving()
		{
			var progressViewMock = A.Fake<IProgressView>();
			A.CallTo(() => mProgressViewFactory.ShowProgressForNextOperation(null, null)).WithAnyArguments()
			                                                                             .Returns(progressViewMock);

			using(var scope = Fake.CreateScope())
			{
				await mTestedCommand.ExecuteAsync();

				using(scope.OrderedAssertions())
				{
					A.CallTo(() => mProgressViewFactory.ShowProgressForNextOperation(null, null)).WithAnyArguments().MustHaveHappened();

					A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustHaveHappened();

					A.CallTo(() => progressViewMock.Hide()).MustHaveHappened();
				}
			}
		}

		[Test]
		public void WhenExceptionOccurs_TheProgressViewIsClosed()
		{
			var progressViewMock = A.Fake<IProgressView>();
			A.CallTo(() => mProgressViewFactory.ShowProgressForNextOperation(null, null)).WithAnyArguments()
			                                                                             .Returns(progressViewMock);
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new OutOfMemoryException());

			Assert.Throws<OutOfMemoryException>(async () => await mTestedCommand.ExecuteAsync());

			A.CallTo(() => progressViewMock.Hide()).MustHaveHappened();
		}

		[Test]
		public async void WhenOperationIsCanceled_NoMessageIsDisplayedToTheUser()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new OperationCanceledException());

			await mTestedCommand.ExecuteAsync();

			A.CallTo(mDialogLauncher).MustNotHaveHappened();
		}

		[Test]
		public async void WhenTheArchiveIsReadOnly_ErrorMessageIsDisplayedToTheUser()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new UnauthorizedAccessException());

			await mTestedCommand.ExecuteAsync();

			A.CallTo(() => mDialogLauncher.DisplayError(A<string>.Ignored)).MustHaveHappened();
		}

		[Test]
		public async void WhenIOExceptionOccurs_ErrorMessageIsDisplayedToTheUser()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new IOException());

			await mTestedCommand.ExecuteAsync();

			A.CallTo(() => mDialogLauncher.DisplayError(A<string>.Ignored)).MustHaveHappened();
		}
	}
}
