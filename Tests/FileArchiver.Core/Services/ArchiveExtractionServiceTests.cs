using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FakeItEasy;

using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Services;
using FileArchiver.Core.ValueTypes;

using FileArchiver.TestUtils;

using NUnit.Framework;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Core.Tests.Services
{
	internal class ArchiveExtractionServiceTests
	{
		private static readonly Path ARCHIVE_PATH = new Path("C:\\ParentDirectory\\Archive.zip");

		private IArchive                  mArchiveMock;
		private IArchiveLoadingService    mLoadingServiceMock;
		private IArchiveExtractionService mTestedService;

		[SetUp]
		public void SetUp()
		{
			mArchiveMock         = A.Fake<IArchive>();
			mLoadingServiceMock  = A.Fake<IArchiveLoadingService>();

			mTestedService       = new ArchiveExtractionService(mLoadingServiceMock);

			A.CallTo(() => mLoadingServiceMock.LoadAsync(ARCHIVE_PATH, A<CancellationToken>.Ignored,
			                                             A<IProgress<double?>>.Ignored))
			 .Returns(mArchiveMock);
		}

		[Test]
		public async void AllFilesAreExtracted()
		{
			var file1            = new FileEntry.Builder().WithName(new FileName("File1.txt")).Build();
			var file2            = new FileEntry.Builder().WithName(new FileName("File2.txt")).Build();
			var file1InDirectory = new FileEntry.Builder().WithName(new FileName("File1InDirectory.txt")).Build();
			var file2InDirectory = new FileEntry.Builder().WithName(new FileName("File2InDirectory.txt")).Build();

			var directory        = new FileEntry.Builder().WithName(new FileName("Directory"))
																		 .AsDirectory()
																		 .WithFiles(file1InDirectory, file2InDirectory)
																		 .Build();

			A.CallTo(() => mArchiveMock.RootFiles).Returns(new FileEntry[] { file1, file2, directory });

			await mTestedService.ExtractArchiveAsync(ARCHIVE_PATH, A.Fake<FileExtractionErrorHandler>(), CancellationToken.None);

			ArchiveTestUtil.AssertFilesExtracted(mArchiveMock, "Directory", "File1.txt", "File2.txt");
		}

		[Test]
		public async void IfTopLevelDirectoryIsAlreadyInArchive_ArchiveIsExtractedWithoutCreatingAdditionalOne()
		{
			A.CallTo(() => mArchiveMock.RootFiles).Returns(new FileEntry[]
			{
				new FileEntry.Builder().WithName(new FileName("Directory")).AsDirectory().Build()
			});

			await mTestedService.ExtractArchiveAsync(ARCHIVE_PATH, A.Fake<FileExtractionErrorHandler>(), CancellationToken.None);

			ArchiveTestUtil.AssertFilesExtractedTo(mArchiveMock, ARCHIVE_PATH.ParentDirectory);
		}

		[Test]
		public async void DirectoryIsCreatedForExtractedFilesIfTheArchiveDoesNotContainOne()
		{
			var file1            = new FileEntry.Builder().WithName(new FileName("File1.txt")).Build();
			var file2            = new FileEntry.Builder().WithName(new FileName("File2.txt")).Build();
			var directory        = new FileEntry.Builder().WithName(new FileName("Directory"))
																		 .AsDirectory()
																		 .Build();

			A.CallTo(() => mArchiveMock.RootFiles).Returns(new FileEntry[] { file1, file2, directory });

			await mTestedService.ExtractArchiveAsync(ARCHIVE_PATH, A.Fake<FileExtractionErrorHandler>(), CancellationToken.None);

			ArchiveTestUtil.AssertFilesExtractedTo(mArchiveMock, ARCHIVE_PATH.RemoveExtension());
		}
	}
}
