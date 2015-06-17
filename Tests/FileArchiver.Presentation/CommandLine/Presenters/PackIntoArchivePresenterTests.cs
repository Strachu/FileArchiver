using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.CommandLine.Presenters;
using FileArchiver.Presentation.Progress;

using NUnit.Framework;

using IFileSystem = System.IO.Abstractions.IFileSystem;
using Path        = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Presentation.Tests.CommandLine.Presenters
{
	internal class PackIntoArchivePresenterTests
	{
		private IPackIntoArchiveView             mViewMock;
		private INewArchiveSettingsScreen        mArchiveSettingsViewMock;
		private IFileSystem                      mFileSystemMock;
		private IArchiveLoadingService           mLoadingServiceMock;
		private IFromFileSystemFileAddingService mFileAddingServiceMock;
		private IArchive                         mArchiveMock;

		private PackIntoArchivePresenter         mTestedPresenter;

		[SetUp]
		public void SetUp()
		{
			mViewMock                = A.Fake<IPackIntoArchiveView>();
			mArchiveSettingsViewMock = A.Fake<INewArchiveSettingsScreen>();
			mFileSystemMock          = A.Fake<IFileSystem>();
			mLoadingServiceMock      = A.Fake<IArchiveLoadingService>();
			mFileAddingServiceMock   = A.Fake<IFromFileSystemFileAddingService>();
			mArchiveMock             = A.Fake<IArchive>();

			mTestedPresenter = new PackIntoArchivePresenter(mViewMock, mArchiveSettingsViewMock, mFileSystemMock,
			                                                mLoadingServiceMock, mFileAddingServiceMock);

			A.CallTo(() => mLoadingServiceMock.CreateNew(null, null)).WithAnyArguments().Returns(mArchiveMock);

			var fakeArchiveSettings = new NewArchiveSettings(new Path("Fake"), A.Fake<IEnumerable<ArchiveFormatWithSettings>>());

			A.CallTo(() => mArchiveSettingsViewMock.Show(null, false)).WithAnyArguments().Returns(fakeArchiveSettings);
		}

		[Test]
		public void ShowsProgressForTheOperation()
		{
			var archiveProgressMock = A.Fake<IProgressView>();

			A.CallTo(() => mViewMock.ShowProgress(A<string>.Ignored)).Returns(archiveProgressMock);

			mTestedPresenter.PackFiles(new Path("C:\\Directory\\File")).Wait();

			A.CallTo(() => mViewMock.ShowProgress(A<string>.Ignored)).MustHaveHappened();
			A.CallTo(() => archiveProgressMock.Hide()).MustHaveHappened();
		}

		[Test]
		public void ShowsTheWindowWithArchiveSettings()
		{
			mTestedPresenter.PackFiles(new Path("C:\\Directory\\File")).Wait();

			A.CallTo(() => mArchiveSettingsViewMock.Show(null, false)).WithAnyArguments().MustHaveHappened();
		}

		[Test]
		public void WhenUserClickedCancelInArchiveSettings_ArchiveShouldNotBeCreated()
		{
			A.CallTo(() => mArchiveSettingsViewMock.Show(null, false)).WithAnyArguments().Returns(null);

			mTestedPresenter.PackFiles(new Path("C:\\Directory\\File")).Wait();

			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments().MustNotHaveHappened();
		}

		[Test]
		public async Task WhenOnlySingleFileIsPacked_DefaultArchivePathIsThePathOfFileMinusItsExtension()
		{
			await mTestedPresenter.PackFiles(new Path("C:\\Directory\\File.txt"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(new Path("C:\\Directory\\File"), A<bool>.Ignored)).MustHaveHappened();
		}

		[Test]
		public async Task WhenMultipleFilesAreToBePacked_DefaultArchiveNameIsParentDirectoryName()
		{
			await mTestedPresenter.PackFiles(new Path("C:\\Directory\\File.txt"),
			                                 new Path("C:\\Directory\\File2.txt"),
			                                 new Path("C:\\Directory\\Directory\\File.txt"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(new Path("C:\\Directory\\Directory"), A<bool>.Ignored)).MustHaveHappened();
		}

		[Test]
		public async Task WhenMultipleFilesFromRootDirectoryArePacked_TheNameOfFirstFileIsUsedAsArchiveName()
		{
			await mTestedPresenter.PackFiles(new Path("C:\\File.txt"),
			                                 new Path("C:\\File2.txt"),
			                                 new Path("C:\\Directory\\File.txt"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(new Path("C:\\File"), A<bool>.Ignored)).MustHaveHappened();
		}

		[Test]
		public async Task WhenOnlySingleFileIsPacked_AllArchiveTypesAreAllowed()
		{
			await mTestedPresenter.PackFiles(new Path("C:\\Directory\\File.txt"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(A<Path>.Ignored, true)).MustHaveHappened();
		}

		[Test]
		public async Task WhenMultipleFilesAreToBePacked_OnlyArchiveTypesSupportingMultipleFilesAreAllowed()
		{
			await mTestedPresenter.PackFiles(new Path("C:\\Directory\\File.txt"),
			                                 new Path("C:\\Directory\\File2.txt"),
			                                 new Path("C:\\Directory\\Directory\\File.txt"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(A<Path>.Ignored, false)).MustHaveHappened();
		}

		[Test]
		public async Task WhenDirectoryIsPacked_OnlyArchiveTypesSupportingMultipleFilesAreAllowed()
		{
			var fileInfo = A.Fake<FileInfoBase>();
			A.CallTo(() => fileInfo.Attributes).Returns(FileAttributes.Directory);
			A.CallTo(() => mFileSystemMock.FileInfo.FromFileName(A<string>.Ignored)).Returns(fileInfo);

			await mTestedPresenter.PackFiles(new Path("C:\\Directory"));

			A.CallTo(() => mArchiveSettingsViewMock.Show(A<Path>.Ignored, false)).MustHaveHappened();
		}

		[Test]
		public void TheArchiveSettingsSpecifiedByTheUserAreUsed()
		{
			var destinationPath            = new Path("C:\\1.zip");
			var archiveFormatSettingsArray = new [] { new ArchiveFormatWithSettings(".zip", null) };
			var newArchiveSettings         = new NewArchiveSettings(destinationPath, archiveFormatSettingsArray);

			A.CallTo(() => mArchiveSettingsViewMock.Show(null, false)).WithAnyArguments().Returns(newArchiveSettings);

			mTestedPresenter.PackFiles(new Path("C:\\Directory\\File.txt")).Wait();

			A.CallTo(() => mLoadingServiceMock.CreateNew(destinationPath, A<IReadOnlyCollection<ArchiveFormatWithSettings>>.That.IsSameSequenceAs(archiveFormatSettingsArray)))
			 .MustHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(A<CancellationToken>.Ignored, A<IProgress<double?>>.Ignored))
			 .MustHaveHappened();
		}

		[Test]
		public void PacksAllSpecifiedFiles()
		{
			var files = new Path[]
			{
				new Path("C:\\Directory\\File.txt"),
				new Path("C:\\Directory\\File2.txt"),
				new Path("C:\\Directory\\Directory")
			};

			mTestedPresenter.PackFiles(files).Wait();

			A.CallTo(() => mFileAddingServiceMock.AddFiles(mArchiveMock, Path.Root, files, A<FileAddingErrorHandler>.Ignored))
			 .MustHaveHappened();
			A.CallTo(() => mArchiveMock.SaveAsync(A<CancellationToken>.Ignored, A<IProgress<double?>>.Ignored))
			 .MustHaveHappened();
		}

		[Test]
		public void WhenOperationIsCanceled_DoesNotShowAnyMessage()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new OperationCanceledException());

			mTestedPresenter.PackFiles(new Path("/home/user/File")).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>._)).MustNotHaveHappened();
		}

		[Test]
		public void WhenCannotSaveInSpecifiedDirectory_DisplaysErrorMessage()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new UnauthorizedAccessException());

			mTestedPresenter.PackFiles(new Path("/home/user/File")).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>._)).MustHaveHappened();
		}

		[Test]
		public void WhenIOExceptionIsThrown_DisplaysErrorMessage()
		{
			A.CallTo(() => mArchiveMock.SaveAsync(CancellationToken.None, null)).WithAnyArguments()
			                                                                    .Throws(new IOException());

			mTestedPresenter.PackFiles(new Path("/home/user/File")).Wait();

			A.CallTo(() => mViewMock.DisplayError(A<string>._)).MustHaveHappened();
		}
	}
}
