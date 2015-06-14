using System;
using System.IO;
using System.Linq;

using FileArchiver.Archive.SevenZip.SevenZipCommunication;

using NUnit.Framework;

namespace FileArchiver.Archive.SevenZip.Tests.SevenZipCommunication
{
	[TestFixture]
	public class FileListingReaderTests
	{
		private const string SampleListing =
@"
7-Zip (A) 9.20  Copyright (c) 1999-2010 Igor Pavlov  2010-11-18

Listing archive: Test.7z

--
Path = Test.7z
Type = 7z
Method = LZMA
Solid = +
Blocks = 2
Physical Size = 481
Headers Size = 230

----------
Path = Directory\1.txt
Size = 15
Packed Size = 25
Modified = 2014-12-12 18:53:04
Attributes = ....A
CRC = 346D5F73
Encrypted = -
Method = LZMA:16
Block = 0

Path = Directory\Test.7z
Size = 225
Packed Size = 226
Modified = 2014-12-13 15:58:41
Attributes = ....A
CRC = BFA06178
Encrypted = -
Method = LZMA:16
Block = 1

Path = Directory
Size = 0
Packed Size = 0
Modified = 2014-12-12 18:53:09
Attributes = D....
CRC =
Encrypted = -
Method =
Block =

";

		private FileListingReader mTestedReader;

		[SetUp]
		public void SetUp()
		{
			mTestedReader = new FileListingReader(new StringReader(SampleListing));
		}

		[Test]
		public void ReadArchiveProperties()
		{
			var archiveProperties = mTestedReader.ReadArchiveProperties();

			Assert.That(archiveProperties["Path"],          Is.EqualTo("Test.7z"));
			Assert.That(archiveProperties["Type"],          Is.EqualTo("7z"));
			Assert.That(archiveProperties["Method"],        Is.EqualTo("LZMA"));
			Assert.That(archiveProperties["Solid"],         Is.EqualTo("+"));
			Assert.That(archiveProperties["Blocks"],        Is.EqualTo("2"));
			Assert.That(archiveProperties["Physical Size"], Is.EqualTo("481"));
			Assert.That(archiveProperties["Headers Size"],  Is.EqualTo("230"));
		}

		[Test]
		public void ReadFirstEntry()
		{
			var entryProperties = mTestedReader.ReadEntries().First();

			Assert.That(entryProperties["Path"],        Is.EqualTo("Directory\\1.txt"));
			Assert.That(entryProperties["Size"],        Is.EqualTo("15"));
			Assert.That(entryProperties["Packed Size"], Is.EqualTo("25"));
			Assert.That(entryProperties["Modified"],    Is.EqualTo("2014-12-12 18:53:04"));
			Assert.That(entryProperties["Attributes"],  Is.EqualTo("....A"));
			Assert.That(entryProperties["CRC"],         Is.EqualTo("346D5F73"));
			Assert.That(entryProperties["Encrypted"],   Is.EqualTo("-"));
			Assert.That(entryProperties["Method"],      Is.EqualTo("LZMA:16"));
			Assert.That(entryProperties["Block"],       Is.EqualTo("0"));
		}

		[Test]
		public void ParseLastEntry()
		{
			var entryProperties = mTestedReader.ReadEntries().Last();

			Assert.That(entryProperties["Path"],        Is.EqualTo("Directory"));
			Assert.That(entryProperties["Size"],        Is.EqualTo("0"));
			Assert.That(entryProperties["Packed Size"], Is.EqualTo("0"));
			Assert.That(entryProperties["Modified"],    Is.EqualTo("2014-12-12 18:53:09"));
			Assert.That(entryProperties["Attributes"],  Is.EqualTo("D...."));
			Assert.That(entryProperties["CRC"],         Is.EqualTo(String.Empty));
			Assert.That(entryProperties["Encrypted"],   Is.EqualTo("-"));
			Assert.That(entryProperties["Method"],      Is.EqualTo(String.Empty));
			Assert.That(entryProperties["Block"],       Is.EqualTo(String.Empty));
		}
	}
}
