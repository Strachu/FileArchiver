using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq.Expressions;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.TestUtils;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Tests.FromFileSystemFileAdding
{
	[TestFixture]
	internal class FromFileSystemFileAddingServiceTest
	{
		private Path                              mPathMock = Path.Root;

		private MockFileSystem                    mFileSystemMock;
		private IArchive                          mArchiveMock;
		private FileAddingErrorHandler            mExceptionHandlerMock;

		private IFromFileSystemFileAddingService  mTestedService;
		
		[SetUp]
		public void SetUp()
		{
			mFileSystemMock       = new MockFileSystem();
			mArchiveMock          = A.Fake<IArchive>();
			mExceptionHandlerMock = A.Fake<FileAddingErrorHandler>();

			mTestedService = new FromFileSystemFileAddingService(mFileSystemMock);

			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory"), new MockDirectoryData()); // AddDirectory adds following slash to directory name
			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory/file1"), new MockFileData(""));
			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory/file2"), new MockFileData(""));
			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory/directory1"), new MockDirectoryData());
			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory/directory1/file1"), new MockFileData(""));
			mFileSystemMock.AddFile(new Path("C:/directory/subdirectory/directory1/file2"), new MockFileData(""));
		}

		[Test]
		public void WhenFileIsAdded_TheFileIsAddedCorrectly()
		{
			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory/file1")
			};

			mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, mExceptionHandlerMock);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("file1"), isDirectory: false);
		}

		[Test]
		public void WhenFileIsAdded_WithDestinationDirectoryOtherThanRoot_TheFileIsAddedCorrectly()
		{
			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory/file1")
			};

			mTestedService.AddFiles(mArchiveMock, new Path("ParentDirectory/ParentSubDir"), filesToAdd, mExceptionHandlerMock);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/ParentSubDir/file1"), isDirectory: false);
		}

		[Test]
		public void WhenFileIsAdded_ButItDoesNotExistAndTheErrorHandlerTellsToRethrow_TheExceptionIsRethrown()
		{
			A.CallTo(() => mExceptionHandlerMock(ref mPathMock, null)).WithAnyArguments().Returns(RetryAction.RethrowException);

			Assert.Throws<FileNotFoundException>(() =>
			{
				mTestedService.AddFiles(mArchiveMock, Path.Root, new Path[] { new Path("file") }, mExceptionHandlerMock);
			});
		}

		[Test]
		public void WhenFileIsAdded_ButItDoesNotExistAndTheErrorHandlerTellsToAbort_TheOperationCanceledExceptionIsThrown()
		{
			A.CallTo(() => mExceptionHandlerMock(ref mPathMock, null)).WithAnyArguments().Returns(RetryAction.Abort);

			Assert.Throws<OperationCanceledException>(() =>
			{
				mTestedService.AddFiles(mArchiveMock, Path.Root, new Path[] { new Path("file") }, mExceptionHandlerMock);
			});
		}

		[Test]
		public void WhenFileIsAdded_ButItDoesNotExistAndTheErrorHandlerTellsToIgnoreTheFile_TheFileIsSkipped()
		{
			A.CallTo(() => mExceptionHandlerMock(ref mPathMock, null)).WithAnyArguments().Returns(RetryAction.Ignore);

			mTestedService.AddFiles(mArchiveMock, Path.Root, new Path[] { new Path("file") }, mExceptionHandlerMock);

			A.CallTo(() => mArchiveMock.AddFile(null, null)).WithAnyArguments().MustNotHaveHappened();
		}

		[Test]
		public void WhenFileIsAdded_AndItAlreadyExistsAndTheErrorHandlerRenamesIt_TheFileIsAddedWithCorrectName()
		{
			ArchiveTestUtil.AddMockFileToArchive(mArchiveMock, new Path("file1"));

			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory/directory1/file1")
			};

			mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, RenameFileErrorHandler);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("newName"), isDirectory: false);
		}

		private RetryAction RenameFileErrorHandler(ref Path path, Exception e)
		{
			path = path.ChangeFileName(new FileName("newName"));

			return RetryAction.Retry;
		}

		[Test]
		public void WhenDirectoryHierarchyIsAdded_TheDirectoryAndAllItsFilesAreAddedCorrectly()
		{
			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory")
			};

			mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, mExceptionHandlerMock);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory"), isDirectory: true);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/file2"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/directory1"), isDirectory: true);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/directory1/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/directory1/file2"), isDirectory: false);
		}

		[Test]
		public void WhenDirectoryHierarchyIsAdded_ButSomeFileAlreadyExistsAndTheErrorHandlerTellsToRethrow_TheExceptionIsRethrown()
		{
			ArchiveTestUtil.AddMockFileToArchive(mArchiveMock, new Path("subdirectory/directory1/file1"));

			A.CallTo(() => mExceptionHandlerMock(ref mPathMock, null)).WithAnyArguments().Returns(RetryAction.RethrowException);

			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory")
			};

			Assert.Throws<FileExistsException>(() =>
			{
				mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, mExceptionHandlerMock);
			});
		}

		[Test]
		public void WhenDirectoryHierarchyIsAdded_ButSomeFileAlreadyExistsAndTheErrorHandlerTellsToAbort_TheOperationCanceledExceptionIsThrown()
		{
			ArchiveTestUtil.AddMockFileToArchive(mArchiveMock, new Path("subdirectory/directory1/file1"));

			A.CallTo(() => mExceptionHandlerMock(ref mPathMock, null)).WithAnyArguments().Returns(RetryAction.Abort);

			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory")
			};

			Assert.Throws<OperationCanceledException>(() =>
			{
				mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, mExceptionHandlerMock);
			});
		}

		[Test]
		public void WhenDirectoryHierarchyIsAdded_ButDirectoryAlreadyExistsAndTheErrorHandlerRenamesIt_AllItsFilesAreAddedWithCorrectPath()
		{
			ArchiveTestUtil.AddMockFileToArchive(mArchiveMock, new Path("subdirectory/directory1"));

			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory")
			};

			mTestedService.AddFiles(mArchiveMock, Path.Root, filesToAdd, RenameFileErrorHandler);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory"), isDirectory: true);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/file2"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/newName"), isDirectory: true);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/newName/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("subdirectory/newName/file2"), isDirectory: false);
		}

		[Test]
		public void WhenMultipleFilesAndDirectoriesAreAdded_AllFilesAreAddedCorrectly()
		{
			var filesToAdd = new Path[]
			{
				new Path("C:/directory/subdirectory/file1"),
				new Path("C:/directory/subdirectory/directory1"),
				new Path("C:/directory/subdirectory/file2")
			};

			mTestedService.AddFiles(mArchiveMock, new Path("ParentDirectory"), filesToAdd, mExceptionHandlerMock);

			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/file2"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/directory1"), isDirectory: true);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/directory1/file1"), isDirectory: false);
			ArchiveTestUtil.AssertFileAdded(mArchiveMock, new Path("ParentDirectory/directory1/file2"), isDirectory: false);
		}
	}
}
