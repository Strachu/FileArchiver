using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using FakeItEasy;

using FileArchiver.Core.Loaders;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.TestUtils;

using NUnit.Framework;

namespace FileArchiver.Presentation.Tests.ArchiveSettings
{
	[TestFixture]
	internal class NewArchiveViewModelTests
	{
		private IArchiveSettingsViewModel mZipSettingsViewModelMock;
		private IArchiveSettingsViewModel mTarSettingsViewModelMock;

		private NewArchiveViewModel       mTestedViewModel;

		[SetUp]
		public void SetUp()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true,  supportsMultipleFiles: true),
				new ArchiveFormatInfo(".tar", "Test", supportsCompression: false, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true,  supportsMultipleFiles: true),
			};

			mZipSettingsViewModelMock = A.Fake<IArchiveSettingsViewModel>(x => x.Implements(typeof(INotifyPropertyChanged)));
			mTarSettingsViewModelMock = A.Fake<IArchiveSettingsViewModel>(x => x.Implements(typeof(INotifyPropertyChanged)));

			var settingsViewModelFactories = new List<IArchiveSettingsViewModelFactory>
			{
				CreateViewModelFactoryFor(".zip", mZipSettingsViewModelMock),
				CreateViewModelFactoryFor(".tar", mTarSettingsViewModelMock),
			};

			mTestedViewModel = new NewArchiveViewModel(supportedFormats, settingsViewModelFactories,
			                                           allowSingleFileArchives: true);
		}

		private IArchiveSettingsViewModelFactory CreateViewModelFactoryFor(string extension,
		                                                                   IArchiveSettingsViewModel viewModel)
		{
			var viewModelFactory = A.Fake<IArchiveSettingsViewModelFactory>();

			A.CallTo(() => viewModelFactory.ArchiveExtension).Returns(extension);
			A.CallTo(() => viewModelFactory.CreateSettingsViewModel()).Returns(viewModel);

			return viewModelFactory;
		}

		[Test]
		public void AllArchivesSupportingCompressionAreNotEmbeddedIntoEachOther()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".tar", "Test", supportsCompression: true, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true, supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { }, true);

			Assert.That(viewModel.AvailableArchiveFormats, Is.EquivalentTo(new string[] { ".zip", ".tar", ".7z" }));
		}

		[Test]
		public void ArchivesNotSupportingCompressionCanBeEmbeddedIntoArchivesSupportingCompression()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true,  supportsMultipleFiles: true),
				new ArchiveFormatInfo(".tar", "Test", supportsCompression: false, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true,  supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { }, true);

			Assert.That(viewModel.AvailableArchiveFormats,
			            Is.EquivalentTo(new string[] { ".zip", ".tar", ".tar.zip", ".tar.7z", ".7z" }));
		}

		[Test]
		public void WhenSingleFileArchivesAreAllowed_AllFormatsAreListedAsAvailable()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".gz",  "Test", supportsCompression: true, supportsMultipleFiles: false),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true, supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { },
																 allowSingleFileArchives: true);

			Assert.That(viewModel.AvailableArchiveFormats, Is.EquivalentTo(new string[] { ".zip", ".gz", ".7z" }));
		}

		[Test]
		public void WhenSingleFileArchivesAreNotAllowed_OnlyFormatsSupportingMultipleFilesAreListedAsAvailable()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".gz",  "Test", supportsCompression: true, supportsMultipleFiles: false),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true, supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { },
																 allowSingleFileArchives: false);

			Assert.That(viewModel.AvailableArchiveFormats, Is.EquivalentTo(new string[] { ".zip", ".7z" }));
		}

		[Test]
		public void WhenSingleFileArchivesAreNotAllowed_SingleFileArchivesCanStillBeUsedAsCompressionArchives()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".tar", "Test", supportsCompression: false, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".gz",  "Test", supportsCompression: true,  supportsMultipleFiles: false),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true,  supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { },
																 allowSingleFileArchives: false);

			Assert.That(viewModel.AvailableArchiveFormats,
			            Is.EquivalentTo(new string[] { ".tar", ".tar.gz", ".tar.7z", ".7z" }));
		}

		[Test]
		public void SelectedFormatDefaultsToOneOfSupportedFormats()
		{
			var supportedFormats = new ArchiveFormatInfo[]
			{
				new ArchiveFormatInfo(".zip", "Test", supportsCompression: true, supportsMultipleFiles: true),
				new ArchiveFormatInfo(".7z",  "Test", supportsCompression: true, supportsMultipleFiles: true)
			};

			var viewModel = new NewArchiveViewModel(supportedFormats, new IArchiveSettingsViewModelFactory[] { }, true);

			Assert.That(supportedFormats.Select(f => f.Extension), Has.Member(viewModel.ArchiveFormat));
		}

		[Test]
		public void DestinationFileExtensionIsAutomaticallyUpdatedWhenArchiveFormatChanges()
		{
			mTestedViewModel.DestinationPath = "C:\\Directory\\Archive.txt";

			mTestedViewModel.ArchiveFormat = ".zip";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo("C:\\Directory\\Archive.zip"));
		}

		[Test]
		public void DestinationFileExtensionUpdatingWorksCorrectlyEvenWithCompositeExtension()
		{
			mTestedViewModel.DestinationPath = "C:\\Directory\\Archive.zip.tar.gz.7z.zip";

			mTestedViewModel.ArchiveFormat = ".tar.7z";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo("C:\\Directory\\Archive.tar.7z"));
		}

		[Test]
		public void DestinationFileExtensionUpdatingWorksCorrectlyForFilesWithoutExtension()
		{
			mTestedViewModel.DestinationPath = "C:\\Directory\\Archive";

			mTestedViewModel.ArchiveFormat = ".zip";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo("C:\\Directory\\Archive.zip"));
		}

		[Test]
		public void DestinationFileExtensionIsNotUpdatedWhenDestinationPathHasNotBeenSetYet()
		{
			mTestedViewModel.ArchiveFormat = ".zip";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo(String.Empty));
		}
		
		[Test]
		public void WhenDestinationPathIsSetWithoutExtension_AndArchiveFormatIsSet_ArchiveExtensionIsAddedToThePath()
		{
			mTestedViewModel.ArchiveFormat = ".zip";
	
			mTestedViewModel.DestinationPath = "C:\\Directory\\Archive";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo("C:\\Directory\\Archive.zip"));
		}
		
		[Test]
		public void WhenDestinationPatIsSetWithSomeExtension_AndArchiveFormatIsSet_PathExtensionIsNotChanged()
		{
			mTestedViewModel.ArchiveFormat = ".zip";
	
			mTestedViewModel.DestinationPath = "C:\\Directory\\Archive.txt";

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo("C:\\Directory\\Archive.txt"));
		}
		
		[Test]
		public void WhenDestinationPatIsToNothing_DoNotChangeIt()
		{
			mTestedViewModel.DestinationPath = String.Empty;

			Assert.That(mTestedViewModel.DestinationPath, Is.EqualTo(String.Empty));
		}

		[Test]
		public void AcceptedSettingsIsNullWhenUserHasCanceled()
		{
			mTestedViewModel.Cancel();

			Assert.That(mTestedViewModel.AcceptedSettings, Is.Null);
		}

		[Test]
		public void AcceptedSettingsIsCorrectAfterAcceptingWithSingleFormat()
		{
			var zipSettings = A.Fake<object>();
			A.CallTo(() => mZipSettingsViewModelMock.ToArchiveSettings()).Returns(zipSettings);

			mTestedViewModel.DestinationPath = "Path.zip";
			mTestedViewModel.ArchiveFormat   = ".zip";

			mTestedViewModel.Accept();

			Assert.That(mTestedViewModel.AcceptedSettings.DestinationPath,                         Is.EqualTo(new Path("Path.zip")));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings,                         Has.Count.EqualTo(1));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.First().ArchiveFormat,   Is.EqualTo(".zip"));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.First().ArchiveSettings, Is.EqualTo(zipSettings));
		}

		[Test]
		public void AcceptedSettingsIsCorrectAfterAcceptingWithMultipleFormats()
		{
			var tarSettings = A.Fake<object>();
			A.CallTo(() => mTarSettingsViewModelMock.ToArchiveSettings()).Returns(tarSettings);

			mTestedViewModel.DestinationPath = "Path.tar.7z";
			mTestedViewModel.ArchiveFormat   = ".tar.7z";

			mTestedViewModel.Accept();

			Assert.That(mTestedViewModel.AcceptedSettings.DestinationPath,                              Is.EqualTo(new Path("Path.tar.7z")));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings,                              Has.Count.EqualTo(2));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.ElementAt(0).ArchiveFormat,   Is.EqualTo(".tar"));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.ElementAt(0).ArchiveSettings, Is.EqualTo(tarSettings));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.ElementAt(1).ArchiveFormat,   Is.EqualTo(".7z"));
			Assert.That(mTestedViewModel.AcceptedSettings.ArchiveSettings.ElementAt(1).ArchiveSettings, Is.Null);
		}

		[Test]
		public void CancelingRaisesCloseEvent()
		{
			bool eventRaised = false;
			mTestedViewModel.ViewClosingRequested += (sender, e) => eventRaised = true;

			mTestedViewModel.Cancel();

			Assert.That(eventRaised, Is.True);
		}

		[Test]
		public void AcceptingRaisesCloseEvent()
		{
			bool eventRaised = false;
			mTestedViewModel.ViewClosingRequested += (sender, e) => eventRaised = true;

			mTestedViewModel.DestinationPath = "/path";

			mTestedViewModel.Accept();

			Assert.That(eventRaised, Is.True);
		}

		[Test]
		public void ArchiveSettingsControlsReturnsEmptyListWhenCurrentFormatDoesNotHaveSettingsViewModelFactory()
		{
			mTestedViewModel.ArchiveFormat = ".7z";

			Assert.That(mTestedViewModel.ArchiveSettingsControls, Is.Empty);
		}

		[Test]
		public void ControlsAreSetToFormatSpecificSettingsAfterFormatChanges()
		{
			var zipControls = new ISettingsControl[]
			{
				A.Fake<ISettingsControl>(),
				A.Fake<ISettingsControl>()
			};

			A.CallTo(() => mZipSettingsViewModelMock.SettingControls).Returns(zipControls);

			mTestedViewModel.ArchiveFormat = ".zip";

			Assert.That(mTestedViewModel.ArchiveSettingsControls, Is.EqualTo(zipControls));
		}

		[Test]
		public void ControlsAreSetToControlsFromBothFormatsInTheCaseOfCompositeFormat()
		{
			var tarControls = new ISettingsControl[]
			{
				A.Fake<ISettingsControl>(),
				A.Fake<ISettingsControl>()
			};

			var zipControls = new ISettingsControl[]
			{
				A.Fake<ISettingsControl>()
			};

			// For debugging purposes only
			A.CallTo(() => tarControls[0].ToString()).Returns("tarControls[0]");
			A.CallTo(() => tarControls[1].ToString()).Returns("tarControls[1]");
			A.CallTo(() => zipControls[0].ToString()).Returns("zipControls[0]");

			A.CallTo(() => mTarSettingsViewModelMock.SettingControls).Returns(tarControls);
			A.CallTo(() => mZipSettingsViewModelMock.SettingControls).Returns(zipControls);

			mTestedViewModel.ArchiveFormat = ".tar.zip";

			Assert.That(mTestedViewModel.ArchiveSettingsControls, Is.EqualTo(tarControls.Concat(zipControls)));
		}

		[Test]
		public void WhenFormatsSettingsControlsListChanges_PropertyNotificationIsForwarded()
		{
			mTestedViewModel.ArchiveFormat = ".zip";

			var propertyChangeTester = new PropertyChangedTester(mTestedViewModel);

			RaisePropertyChangedEventFor(mZipSettingsViewModelMock as INotifyPropertyChanged,
			                             PropertyName.Of(() => mZipSettingsViewModelMock.SettingControls));

			propertyChangeTester.AssertPropertyChangedRaisedFor(() => mTestedViewModel.ArchiveSettingsControls);
		}

		[Test]
		public void PropertyNotificationIsNotRaisedFormatSettingsControlsListChangesForNotCurrectlySelectedFormat()
		{
			mTestedViewModel.ArchiveFormat = ".zip";
			mTestedViewModel.ArchiveFormat = ".tar";

			var propertyChangeTester = new PropertyChangedTester(mTestedViewModel);

			RaisePropertyChangedEventFor(mZipSettingsViewModelMock as INotifyPropertyChanged,
			                             PropertyName.Of(() => mZipSettingsViewModelMock.SettingControls));

			propertyChangeTester.AssertNoPropertyChangedRaised();
		}

		private void RaisePropertyChangedEventFor(INotifyPropertyChanged propertyNotifier, string propertyName)
		{
			propertyNotifier.PropertyChanged += Raise.With(new PropertyChangedEventArgs(propertyName)).Now;			
		}
	}
}
