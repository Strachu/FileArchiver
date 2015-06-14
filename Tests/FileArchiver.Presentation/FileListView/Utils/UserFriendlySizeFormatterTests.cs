using System.Globalization;
using System.Threading;

using FileArchiver.Presentation.FileListView.Utils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.FileListView.Utils
{
	[TestFixture]
	public class UserFriendlySizeFormatterTests
	{
		private const long KibiByte = 1024;
		private const long MebiByte = 1024 * KibiByte;
		private const long GibiByte = 1024 * MebiByte;

		[SetUp]
		public void SetUp()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}

		[Test]
		public void FormatBytes()
		{
			var formattedString = UserFriendlySizeFormatter.Format(825);

			Assert.That(formattedString, Is.EqualTo("825 B"));
		}

		[Test]
		public void FormatKibiBytes()
		{
			var formattedString = UserFriendlySizeFormatter.Format(1100);

			Assert.That(formattedString, Is.EqualTo("1.07 KiB"));
		}

		[Test]
		public void FormatGibiBytes()
		{
			var formattedString = UserFriendlySizeFormatter.Format(3 * GibiByte);

			Assert.That(formattedString, Is.EqualTo("3 GiB"));
		}

		[Test]
		public void ShowFractionValueInsteadOf4Digits()
		{
			var formattedString = UserFriendlySizeFormatter.Format(1012 * KibiByte);

			Assert.That(formattedString, Is.EqualTo("0.988 MiB"));
		}
	}
}
