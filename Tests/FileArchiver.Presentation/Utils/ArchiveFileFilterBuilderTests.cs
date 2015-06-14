using System;

using FileArchiver.Core.Loaders;
using FileArchiver.Presentation.Properties;
using FileArchiver.Presentation.Utils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.Utils
{
	[TestFixture]
	public class ArchiveFileFilterBuilderTests
	{
		[Test]
		public void BuildFilterWithSingleFormat()
		{
			var archiveInfos = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo
				(
					localizedDescription : "Zip archive",
					extension            : ".zip",
					supportsCompression  : false
				)
			};

			var builtFilter    = ArchiveFileFilterBuilder.BuildFilter(archiveInfos);
			var expectedFilter = String.Format("{0}|*.zip|Zip archive|*.zip|{1}|*.*", Resources.Filter_SupportedArchives, Resources.Filter_AllFiles);

			Assert.That(builtFilter, Is.EqualTo(expectedFilter));
		}

		[Test]
		public void BuildFilterWithMultipleFormats()
		{
			var archiveInfos = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo
				(
					localizedDescription : "Zip archive",
					extension            : ".zip",
					supportsCompression  : false
				),
				new ArchiveFormatInfo
				(
					localizedDescription : "Tar archive",
					extension            : ".tar",
					supportsCompression  : false
				),
				new ArchiveFormatInfo
				(
					localizedDescription : "7-zip archive",
					extension            : ".7z",
					supportsCompression  : false
				)
			};

			var builtFilter    = ArchiveFileFilterBuilder.BuildFilter(archiveInfos);
			var expectedFilter = String.Format("{0}|*.zip;*.tar;*.7z|" +
			                                   "Zip archive|*.zip|" +
			                                   "Tar archive|*.tar|" +
			                                   "7-zip archive|*.7z|" +
			                                   "{1}|*.*", Resources.Filter_SupportedArchives, Resources.Filter_AllFiles);

			Assert.That(builtFilter, Is.EqualTo(expectedFilter));
		}
	}
}
