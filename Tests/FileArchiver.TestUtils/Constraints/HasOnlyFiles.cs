using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FileArchiver.Core.Archive;
using FileArchiver.Core.ValueTypes;

using NUnit.Framework.Constraints;

namespace FileArchiver.TestUtils
{
	/// <summary>
	/// Checks whether the list contains only files with specified paths.
	/// </summary>
	internal class HasOnlyFiles : Constraint
	{
		private readonly IList<Path> mPaths;

		public HasOnlyFiles(params string[] paths)
		{
			mPaths = paths.Select(x => new Path(x)).ToList();
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;

			var fileList = actual as IEnumerable<FileEntry>;
			if(fileList == null)
				return false;

			var flattenedFileList = fileList.Flatten().ToList();
			
			return flattenedFileList.Count() == mPaths.Count() &&
			       flattenedFileList.All(file => mPaths.Contains(file.Path));
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WriteCollectionElements(mPaths, 0, mPaths.Count);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			var fileList = actual as IEnumerable<FileEntry>;
			if(fileList == null)
				return;

			var flattenedFileListPaths = fileList.Flatten().Select(x => x.Path).ToList();

			writer.WriteCollectionElements(flattenedFileListPaths, 0, flattenedFileListPaths.Count);
		}
	}
}
