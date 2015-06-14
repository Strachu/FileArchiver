using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Services;
using FileArchiver.Presentation.CommandLine.Presenters;
using FileArchiver.Presentation.PerFileErrorHandlers;
using FileArchiver.Presentation.Progress;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.Tests.CommandLine.Presenters
{
	[TestFixture]
	internal class EntireArchiveExtractionPresenterTests
	{
		private static readonly Path ARCHIVE1_PATH = new Path("C:\\ParentDirectory\\Archive1.zip");
		private static readonly Path ARCHIVE2_PATH = new Path("C:\\ParentDirectory\\Archive2.zip");
		private static readonly Path ARCHIVE3_PATH = new Path("C:\\ParentDirectory\\Archive3.zip");

		private IEntireArchivesExtractionView           mViewMock;
		private IArchiveExtractionService               mExtractionServiceMock;
		private IExtractionPerFileErrorPresenterFactory mPerFileErrorPresenterFactoryMock;
		private EntireArchivesExtractionPresenter       mTestedPresenter;

		private IProgressView mArchive1ProgressMock;
		private IProgressView mArchive2ProgressMock;
		private IProgressView mArchive3ProgressMock;

		[SetUp]
		public void SetUp()
		{
			mViewMock                         = A.Fake<IEntireArchivesExtractionView>();
			mExtractionServiceMock            = A.Fake<IArchiveExtractionService>();
			mPerFileErrorPresenterFactoryMock = A.Fake<IExtractionPerFileErrorPresenterFactory>();

			mTestedPresenter = new EntireArchivesExtractionPresenter(mViewMock, mExtractionServiceMock,
			                                                         mPerFileErrorPresenterFactoryMock);

			mArchive1ProgressMock = A.Fake<IProgressView>();
			mArchive2ProgressMock = A.Fake<IProgressView>();
			mArchive3ProgressMock = A.Fake<IProgressView>();

			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE1_PATH)).Returns(mArchive1ProgressMock);
			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE2_PATH)).Returns(mArchive2ProgressMock);
			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE3_PATH)).Returns(mArchive3ProgressMock);
		}

		[Test]
		public void ShowsProgressForEachOperation()
		{
			mTestedPresenter.ExtractArchives(ARCHIVE1_PATH, ARCHIVE2_PATH, ARCHIVE3_PATH).Wait();

			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE1_PATH)).MustHaveHappened();
			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE2_PATH)).MustHaveHappened();
			A.CallTo(() => mViewMock.ShowProgressForNextExtraction(ARCHIVE3_PATH)).MustHaveHappened();
			A.CallTo(() => mArchive1ProgressMock.Hide()).MustHaveHappened();
			A.CallTo(() => mArchive2ProgressMock.Hide()).MustHaveHappened();
			A.CallTo(() => mArchive3ProgressMock.Hide()).MustHaveHappened();
		}

		[Test]
		public void WhenExtractionOfOneArchiveIsCanceled_TheRestIsNotAffected()
		{
			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(ARCHIVE2_PATH,
			                                                          A<FileExtractionErrorHandler>.Ignored,
			                                                          A<CancellationToken>.Ignored,
			                                                          A<IProgress<double?>>.Ignored))
			 .Throws(new OperationCanceledException());

			mTestedPresenter.ExtractArchives(ARCHIVE1_PATH, ARCHIVE2_PATH, ARCHIVE3_PATH).Wait();

			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(ARCHIVE1_PATH,
			                                                          A<FileExtractionErrorHandler>.Ignored,
			                                                          A<CancellationToken>.Ignored,
			                                                          A<IProgress<double?>>.Ignored)).MustHaveHappened();

			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(ARCHIVE3_PATH,
			                                                          A<FileExtractionErrorHandler>.Ignored,
			                                                          A<CancellationToken>.Ignored,
			                                                          A<IProgress<double?>>.Ignored)).MustHaveHappened();
		}

		[Test]
		public void WhenExtractionIsCanceled_NoMessageAreDisplayed()
		{
			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(null, null, CancellationToken.None, null))
			 .WithAnyArguments()
			 .Throws(new OperationCanceledException());

			mTestedPresenter.ExtractArchives(ARCHIVE1_PATH).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>.Ignored)).MustNotHaveHappened();
		}

		[Test]
		public void WhenTheFormatIsUnsupported_ErrorMessageIsDisplayed()
		{
			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(null, null, CancellationToken.None, null))
			 .WithAnyArguments()
			 .Throws(new NotSupportedFormatException("test"));

			mTestedPresenter.ExtractArchives(new Path("/home/user/test")).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>.Ignored)).MustHaveHappened();
		}

		[Test]
		public void WhenIOExceptionIsThrown_ErrorMessageIsDisplayed()
		{
			A.CallTo(() => mExtractionServiceMock.ExtractArchiveAsync(null, null, CancellationToken.None, null))
			 .WithAnyArguments()
			 .Throws(new FileNotFoundException());

			mTestedPresenter.ExtractArchives(new Path("/home/user/test")).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>._)).MustHaveHappened();
		}
	}
}
