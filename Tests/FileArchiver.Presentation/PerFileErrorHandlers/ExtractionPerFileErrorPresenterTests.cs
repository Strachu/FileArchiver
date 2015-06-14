using System;
using System.IO;
using System.IO.Abstractions;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Services;
using FileArchiver.Core.Utils;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.TestUtils;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.Tests.PerFileErrorHandlers
{
	internal class ExtractionPerFileErrorPresenterTests
	{
		private IPerFileErrorView               mViewMock;
		private IFileSystem                     mFileSystemMock;
		private IFileNameGenerator              mFileNameGeneratorMock;
		private ExtractionPerFileErrorPresenter mTestedPresenter;

		[SetUp]
		public void SetUp()
		{
			mViewMock              = A.Fake<IPerFileErrorView>();
			mFileSystemMock        = A.Fake<IFileSystem>();
			mFileNameGeneratorMock = A.Fake<IFileNameGenerator>();

			mTestedPresenter = new ExtractionPerFileErrorPresenter(mViewMock, mFileSystemMock, mFileNameGeneratorMock,
			                                                       new NullSynchronizationContext());
		}

		[Test]
		public void WhenUnhandledExceptionIsPassed_NothingIsDone()
		{
			var destinationPath = new Path("DestinationFile");

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new OutOfMemoryException());

			A.CallTo(mViewMock).MustNotHaveHappened();
			A.CallTo(mFileSystemMock).MustNotHaveHappened();
			A.CallTo(mFileNameGeneratorMock).MustNotHaveHappened();
			Assert.That(actionToPerform, Is.EqualTo(RetryAction.RethrowException));
		}

		[Test]
		public void WhenOperationCanceledExceptionIsPassed_AbortWithoutShowingAnyMessage()
		{
			var destinationPath = new Path("DestinationFile");

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new OperationCanceledException());

			A.CallTo(mViewMock).MustNotHaveHappened();
			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Abort));
		}

		[Test]
		public void WhenIOExceptionIsPassed_AskWhatToDoAndSetActionToPerformCorrespodingly()
		{
			var destinationPath = new Path("DestinationFile");

			A.CallTo(() => mViewMock.AskForRetryAction(A<string>.Ignored)).Returns(RetryAction.Retry);

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new IOException());

			A.CallTo(() => mViewMock.AskForRetryAction(A<string>.Ignored)).MustHaveHappened();
			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Retry));
		}

		[Test]
		public void WhenFileExistsExceptionIsPassed_TellThatFileExistsAndAskWhatToDo()
		{
			var destinationPath = new Path("DestinationFile");

			mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(destinationPath, A<Path>.Ignored, out unused)).MustHaveHappened();
		}

		[Test]
		public void WhenFileExistsAndUserChooseRename_UseTheNameGeneratorAndChangeTheName()
		{
			var destinationPath = new Path("DestinationFile");
			var newPath         = new Path("New name");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments()
			                                                                        .Returns(FileExistsAction.Rename);
			A.CallTo(() => mFileNameGeneratorMock.GenerateFreeFileName(null, null)).WithAnyArguments().Returns(newPath);

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Retry));
			Assert.That(destinationPath, Is.EqualTo(newPath));
		}

		[Test]
		public void WhenFileExistsAndUserChooseAbort_ThrowsOperationCanceledException()
		{
			var destinationPath = new Path("DestinationFile");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments()
			                                                                        .Returns(FileExistsAction.Abort);

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Abort));
		}

		[Test]
		public void WhenFileExistsAndUserChooseSkip_TellTheCallerToIgnoreIt()
		{
			var destinationPath = new Path("DestinationFile");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments()
			                                                                        .Returns(FileExistsAction.Skip);

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Ignore));
		}

		[Test]
		public void WhenFileExistsAndUserChooseOverwrite_DeleteTheFileAndTellTheCallerToRetry()
		{
			var destinationPath = new Path("DestinationFile");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments()
			                                                                        .Returns(FileExistsAction.Overwrite);

			var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

			A.CallTo(() => mFileSystemMock.File.Delete(destinationPath)).MustHaveHappened();
			Assert.That(actionToPerform, Is.EqualTo(RetryAction.Retry));
		}

		[Test]
		public void WhenFileExistsAndUserChooseOverwriteAndUnknownExceptionThrowsDuringFileDeleting_RethrowIt()
		{
			var destinationPath = new Path("DestinationFile");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments().Returns(FileExistsAction.Overwrite);
			A.CallTo(() => mFileSystemMock.File.Delete(A<string>.Ignored)).Throws(new OutOfMemoryException());

			Assert.Throws<OutOfMemoryException>(() =>
			{
				mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));
			});
		}

		[Test]
		public void WhenFileExistsAndUserChooseOverwriteAndIOExceptionThrowsDuringFileDeleting_AskTheUserWhatToDoNowAndRetryIfUserChosenToDoSo()
		{
			var destinationPath = new Path("DestinationFile");

			bool unused;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out unused)).WithAnyArguments().Returns(FileExistsAction.Overwrite);
			A.CallTo(() => mViewMock.AskForRetryAction(A<string>.Ignored)).ReturnsNextFromSequence(RetryAction.Retry, RetryAction.Abort);
			A.CallTo(() => mFileSystemMock.File.Delete(A<string>.Ignored)).Throws(new IOException());

			Assert.Throws<OperationCanceledException>(() =>
			{
				mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));
			});

			A.CallTo(() => mFileSystemMock.File.Delete(destinationPath)).MustHaveHappened(Repeated.Exactly.Twice);
		}

		[Test]
		public void WhenFileExistsAndUserChooseSkipAndSelectApplyToAll_DoNotBotherHimForNextExceptions()
		{
			var destinationPath = new Path("DestinationFile");

			bool applyToAll = true;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out applyToAll)).WithAnyArguments()
			                                                                            .Returns(FileExistsAction.Skip);

			for(int i = 0; i < 5; ++i)
			{
				var actionToPerform = mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));

				Assert.That(actionToPerform, Is.EqualTo(RetryAction.Ignore));
			}

			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out applyToAll)).WithAnyArguments()
			 .MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void ApplyToAllForFileExistsDoesNotApplyToOtherExceptions()
		{
			var destinationPath = new Path("DestinationFile");

			bool applyToAll = true;
			A.CallTo(() => mViewMock.AskForFileExistsAction(null, null, out applyToAll)).WithAnyArguments()
			                                                                            .Returns(FileExistsAction.Skip);

			mTestedPresenter.ExceptionThrown(ref destinationPath, new FileExistsException(destinationPath));
			mTestedPresenter.ExceptionThrown(ref destinationPath, new IOException());

			A.CallTo(() => mViewMock.AskForRetryAction(A<string>.Ignored)).MustHaveHappened();
		}
	}
}
