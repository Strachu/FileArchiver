using System.Collections.Generic;
using System.Threading;

using FileArchiver.Core.Utils.File;

using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Core.Tests.Utils.File
{
	[TestFixture]
	public class CompositeFileProgressTests
	{
		[Test]
		public void ProgressReportingWithSingleFile()
		{
			var reports = new List<long>();

			var testedObject = new CompositeFileProgress(reports.Add);

			var progressObject = testedObject.GetProgressForNextFile();

			progressObject.Report(10000);
			progressObject.Report(20000);
			progressObject.Report(30000);

			Assert.That(reports, Has.Count.EqualTo(3));
			Assert.That(reports[0], Is.EqualTo(10000));
			Assert.That(reports[1], Is.EqualTo(20000));
			Assert.That(reports[2], Is.EqualTo(30000));
		}

		[Test]
		public void ProgressReportingWithMultipleFiles()
		{
			var reports = new List<long>();

			var testedObject = new CompositeFileProgress(reports.Add);

			var progressObjectForFirstFile  = testedObject.GetProgressForNextFile();
			var progressObjectForSecondFile = testedObject.GetProgressForNextFile();
			var progressObjectForThirdFile  = testedObject.GetProgressForNextFile();

			progressObjectForFirstFile .Report(10000);
			progressObjectForSecondFile.Report( 5000);
			progressObjectForFirstFile .Report(20000);
			progressObjectForThirdFile .Report(50000);
			progressObjectForSecondFile.Report(20000);
			progressObjectForFirstFile .Report(30000);

			Assert.That(reports, Has.Count.EqualTo(6));
			Assert.That(reports[0], Is.EqualTo( 10000));
			Assert.That(reports[1], Is.EqualTo( 15000));
			Assert.That(reports[2], Is.EqualTo( 25000));
			Assert.That(reports[3], Is.EqualTo( 75000));
			Assert.That(reports[4], Is.EqualTo( 90000));
			Assert.That(reports[5], Is.EqualTo(100000));
		}
	}
}
