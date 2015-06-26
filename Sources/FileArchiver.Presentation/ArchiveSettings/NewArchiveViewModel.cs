#region Copyright
/*
 * Copyright (C) 2015 Patryk Strach
 * 
 * This file is part of FileArchiver.
 * 
 * FileArchiver is free software: you can redistribute it and/or modify it under the terms of
 * the GNU Lesser General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * FileArchiver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with FileArchiver.
 * If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;

using FileArchiver.Core;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Utils;
using FileArchiver.Core.ValueTypes;
using FileArchiver.Presentation.ArchiveSettings.Framework;
using FileArchiver.Presentation.Utils;

namespace FileArchiver.Presentation.ArchiveSettings
{
	/// <summary>
	/// A view model for a view allowing a user to choose new archive format and its format specific settings.
	/// </summary>
	internal sealed class NewArchiveViewModel : NotifyPropertyChangedHelper
	{
		private readonly Dictionary<string, List<IArchiveSettingsViewModel>> mArchiveSettingsViewModels = new Dictionary<string, List<IArchiveSettingsViewModel>>();

		private string mSelectedFormat;

		/// <summary>
		/// Initializes a new instance of the <see cref="NewArchiveViewModel"/> class.
		/// </summary>
		/// <param name="supportedFormats">
		/// The collection of supported formats.
		/// </param>
		/// <param name="settingsFactories">
		/// The collection of factories for view models describing format specific settings.
		/// </param>
		/// <param name="allowSingleFileArchives">
		/// Whether to show archive types not supporting multiple files as available.
		/// </param>
		public NewArchiveViewModel(IReadOnlyCollection<ArchiveFormatInfo> supportedFormats,
		                           IReadOnlyCollection<IArchiveSettingsViewModelFactory> settingsFactories,
		                           bool allowSingleFileArchives)
		{
			Contract.Requires(supportedFormats != null);
			Contract.Requires(Contract.ForAll(supportedFormats, format => format != null));
			Contract.Requires(settingsFactories != null);
			Contract.Requires(Contract.ForAll(settingsFactories, factory => factory != null));

			// TODO move to a different class?
			ConstructFormatToSettingsViewModelsMapping(supportedFormats, settingsFactories, allowSingleFileArchives);

			AvailableArchiveFormats = mArchiveSettingsViewModels.Keys;
			ArchiveFormat           = AvailableArchiveFormats.First();
		}

		private void ConstructFormatToSettingsViewModelsMapping(IReadOnlyCollection<ArchiveFormatInfo> supportedFormats,
		                                                        IReadOnlyCollection<IArchiveSettingsViewModelFactory>
			                                                        settingsFactories,
		                                                        bool includeSingleFileArchives)
		{
			var supportedFormatsForFiles = GetFormatsSuitableForStoringFiles(supportedFormats, !includeSingleFileArchives);

			foreach(var format in supportedFormatsForFiles)
			{
				mArchiveSettingsViewModels[format.Extension] = new List<IArchiveSettingsViewModel>
				{
					GetSettingsViewModel(format.Extension, settingsFactories)
				};

				if(!format.SupportsCompression)
				{
					AllowEmbeddingIntoArchivesSupportingCompression(format.Extension, supportedFormats, settingsFactories);
				}
			}
		}

		private IEnumerable<ArchiveFormatInfo> GetFormatsSuitableForStoringFiles(IEnumerable<ArchiveFormatInfo> supportedFormats,
			                                                                      bool mustSupportMultipleFiles)
		{
			if(!mustSupportMultipleFiles)
				return supportedFormats;

			return supportedFormats.Where(format => format.SupportsMultipleFiles);
		}

		private IArchiveSettingsViewModel GetSettingsViewModel(string formatExtension, IEnumerable<IArchiveSettingsViewModelFactory> viewModelFactories)
		{
			var archiveSettingsFactory = viewModelFactories.FirstOrDefault(factory => factory.ArchiveExtension == formatExtension);
			if(archiveSettingsFactory == null)
				return null;

			return archiveSettingsFactory.CreateSettingsViewModel();
		}

		private void AllowEmbeddingIntoArchivesSupportingCompression(string innerFormatExtension,
		                                                             IEnumerable<ArchiveFormatInfo> supportedFormats,
		                                                             IReadOnlyCollection<IArchiveSettingsViewModelFactory>
			                                                             settingsFactories)
		{
			var formatsSupportingCompression = supportedFormats.Where(format => format.SupportsCompression);

			foreach(var outerFormat in formatsSupportingCompression)
			{
				var compositeFormat = innerFormatExtension + outerFormat.Extension;

				mArchiveSettingsViewModels[compositeFormat] = new List<IArchiveSettingsViewModel>
				{
					mArchiveSettingsViewModels[innerFormatExtension].Single(),
					GetSettingsViewModel(outerFormat.Extension, settingsFactories)
				};
			}
		}

		private string _mDestinationPath = String.Empty;
		public string DestinationPath
		{
			get { return _mDestinationPath; }
			set
			{
				bool hasExtension      = !String.IsNullOrEmpty(System.IO.Path.GetExtension(value));
				var  pathWithExtension = hasExtension ? value : (value + ArchiveFormat);

				base.SetFieldWithNotification(ref _mDestinationPath, value != String.Empty ? pathWithExtension : value);
			}
		}

		/// <summary>
		/// Currently selected archive format.
		/// </summary>
		public string ArchiveFormat
		{
			get
			{
				return mSelectedFormat;
			}

			set
			{
				if(mSelectedFormat != null)
				{
					StopListeningForControlsChanges(mArchiveSettingsViewModels[mSelectedFormat]);
				}

				SetFieldWithNotification(ref mSelectedFormat, value);

				ListenForControlsChanges(mArchiveSettingsViewModels[mSelectedFormat]);

				UpdateDestinationFileExtension();

				OnPropertyChanged(PropertyName.Of(() => ArchiveSettingsControls));
			}
		}

		private void StopListeningForControlsChanges(IEnumerable<IArchiveSettingsViewModel> viewModels)
		{
			foreach(var propertyNotifier in viewModels.OfType<INotifyPropertyChanged>())
			{
				propertyNotifier.PropertyChanged -= ControlsPropertyChanged;
			}
		}

		/// <summary>
		/// Starts listening for view models' notification about changes of list with format specific settings controls.
		/// </summary>
		/// <remarks>
		/// The listening is needed so that view can be notified when it should recreate the controls.
		/// </remarks>
		private void ListenForControlsChanges(IEnumerable<IArchiveSettingsViewModel> viewModels)
		{
			foreach(var propertyNotifier in viewModels.OfType<INotifyPropertyChanged>())
			{
				propertyNotifier.PropertyChanged += ControlsPropertyChanged;
			}
		}

		private void ControlsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == PropertyName.Of<IArchiveSettingsViewModel>(x => x.SettingControls))
			{
				OnPropertyChanged(PropertyName.Of(() => ArchiveSettingsControls));
			}
		}

		/// <summary>
		/// Changes the destination path extension so that it matches currently selected archive format extension.
		/// </summary>
		private void UpdateDestinationFileExtension()
		{
			if(String.IsNullOrWhiteSpace(DestinationPath))
				return;

			int dotIndex = DestinationPath.IndexOf('.');

			var destinationPathWithoutExtension = (dotIndex != -1) ? DestinationPath.Substring(0, dotIndex) : DestinationPath;

			DestinationPath = destinationPathWithoutExtension + ArchiveFormat;
		}

		public IEnumerable<string> AvailableArchiveFormats
		{
			get;
			private set;
		}

		/// <summary>
		/// Returns the list of format specific settings controls for currently selected format.
		/// </summary>
		public IEnumerable<ISettingsControl> ArchiveSettingsControls
		{
			get
			{
				var settingsViewModels = mArchiveSettingsViewModels[mSelectedFormat];
				if(settingsViewModels == null)
					return null;

				return settingsViewModels.Where(VM => VM != null).SelectMany(VM => VM.SettingControls);
			}
		}

		/// <summary>
		/// Returns the settings accepted by the user or null if the user did not accept any settings.
		/// </summary>
		public NewArchiveSettings AcceptedSettings
		{
			get;
			private set;
		}

		public void Accept()
		{
			AcceptedSettings = new NewArchiveSettings(new Path(DestinationPath), ChosenFormatsDescription);

			ViewClosingRequested.SafeRaise(this, EventArgs.Empty);
		}

		private IEnumerable<ArchiveFormatWithSettings> ChosenFormatsDescription
		{
			get
			{
				var currentFormats          = ArchiveFormat.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				var currentFormatViewModels = mArchiveSettingsViewModels[ArchiveFormat];

				for(int i = 0; i < currentFormatViewModels.Count; ++i)
				{
					var formatSettings = currentFormatViewModels[i] != null ? currentFormatViewModels[i].ToArchiveSettings() : null;

					yield return new ArchiveFormatWithSettings('.' + currentFormats[i], formatSettings);
				}
			}
		}

		public void Cancel()
		{
			AcceptedSettings = null;

			ViewClosingRequested.SafeRaise(this, EventArgs.Empty);
		}

		public event EventHandler ViewClosingRequested;
	}
}
