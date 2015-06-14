using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using FakeItEasy;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;

namespace FileArchiver.TestUtils
{
	internal static class ArchiveTestUtil
	{
		public static void AddMockFileToArchive(IArchive archiveMock, Path filePath)
		{
			Expression<Func<FileEntry, bool>> predicate = x => x.Name.Equals(filePath.FileName);

			A.CallTo(() => archiveMock.AddFile(filePath.ParentDirectory, A<FileEntry>.That.Matches(predicate)))
			 .Throws(new FileExistsException(filePath));
		}

		public static void AssertFileAdded(IArchive archiveMock, Path filePath, bool isDirectory)
		{
			Func<FileEntry, bool> predicate = x => x.Name.Equals(filePath.FileName) && x.IsDirectory == isDirectory;

			var fileOrDirectoryString = isDirectory ? "Directory" : "File";

			A.CallTo(() => archiveMock.AddFile(filePath.ParentDirectory,
				A<FileEntry>.That.Matches(predicate, fileOrDirectoryString + " named " + filePath.FileName)))
			 .MustHaveHappened();
		}

		public static void AssertFilesExtracted(IArchive sourceArchive, params string[] files)
		{
			var filePaths = files.Select(file => new Path(file));

			A.CallTo(() => sourceArchive.ExtractFilesAsync(A<IReadOnlyCollection<SourceDestinationPathPair>>.That.Matches
			(
				x => x.Select(y => y.SourcePath).SequenceEqualIgnoringOrder(filePaths)
			),
																		  A<FileExtractionErrorHandler>.Ignored,
																		  A<CancellationToken>.Ignored,
																		  A<IProgress<double?>>.Ignored)).MustHaveHappened();
		}

		public static void AssertFilesExtractedTo(IArchive sourceArchive, Path expectedPath)
		{
			A.CallTo(() => sourceArchive.ExtractFilesAsync(A<IReadOnlyCollection<SourceDestinationPathPair>>.That.Matches
			(
				x => x.All(y => y.DestinationPath.ParentDirectory.Equals(expectedPath))
			),
																		  A<FileExtractionErrorHandler>.Ignored,
																		  A<CancellationToken>.Ignored,
																		  A<IProgress<double?>>.Ignored)).MustHaveHappened();
		}
	}
}
