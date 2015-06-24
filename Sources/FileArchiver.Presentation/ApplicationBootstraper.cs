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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

using FileArchiver.Core;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Loaders;
using FileArchiver.Core.Services;
using FileArchiver.Presentation.ArchiveSettings;
using FileArchiver.Presentation.CommandLine;
using FileArchiver.Presentation.Commands.CommandSystem;
using FileArchiver.Presentation.FileListView;
using FileArchiver.Presentation.OtherViews;
using FileArchiver.Presentation.Settings;
using FileArchiver.Presentation.Shell;

namespace FileArchiver.Presentation
{
	/// <summary>
	/// A class responsible for resolving the dependencies (including all plugins) of the application with IoC container.
	/// </summary>
	public static class ApplicationBootstraper
	{
		private const string PLUGIN_DIRECTORY = "Plugins";

		/// <summary>
		/// Initializes the IoC container with specified catalogs.
		/// </summary>
		/// <param name="dependencyCatalogs">
		/// The catalogs with dependencies to pass to MEF IoC container.
		/// </param>
		/// <remarks>
		/// The catalogs have specified priority based on their order in specified list.
		/// The higher the index of the catalog in the list, the higher its priority.
		/// If you want multiple catalogs to have the same priority, wrap them into AggregateCatalog.
		///
		/// The catalog is meaningful only when there is a conflict caused by multiple exports fitting into a single import.
		/// In this situation, the export from a catalog with higher catalog will be used and the rest will be ignored.
		/// This allows overriding of application's default implementations of services with user specified plugins.
		/// </remarks>
		/// <returns>
		/// Initialized IoC container.
		/// </returns>
		public static CompositionContainer InitializeContainer(IReadOnlyCollection<ComposablePartCatalog> dependencyCatalogs)
		{
			Contract.Requires(dependencyCatalogs != null);
			Contract.Requires(Contract.ForAll(dependencyCatalogs, catalog => catalog != null));

			FixTypeConvertersFromPlugins();

			//	Catalog prioritization based on http://stackoverflow.com/a/11203426
			var exportProviders = dependencyCatalogs.Select(catalog => new CatalogExportProvider(catalog)).ToArray();

			var container = new CompositionContainer(exportProviders.Reverse().ToArray());

			foreach(var provider in exportProviders)
			{
				provider.SourceProvider = container;
			}

			DependencyInjectionContainer.Initialize(container);

			return container;
		}

		/// <summary>
		/// Creates a catalog representing all plugins added to the application.
		/// </summary>
		public static ComposablePartCatalog CreatePluginsCatalog()
		{
			return new DirectoryCatalog(PLUGIN_DIRECTORY, ConfigureExportsForPlugins());
		}

		/// <summary>
		/// Creates a catalog with every type implementing an public interface in the application.
		/// </summary>
		public static ComposablePartCatalog CreateApplicationCatalog()
		{
			return new AggregateCatalog
			(
				CreateCoreAssemblyCatalog(),
				CreatePresentationAssemblyCatalog(),
				new AssemblyCatalog(Assembly.GetAssembly(typeof(IFileSystem)), ConfigureExportsForEveryTypeInApplication())
			);
		}

		private static ComposablePartCatalog CreateCoreAssemblyCatalog()
		{
			return new AssemblyCatalog(Assembly.GetAssembly(typeof(IArchive)), ConfigureExportsForEveryTypeInApplication());
		}

		private static ComposablePartCatalog CreatePresentationAssemblyCatalog()
		{
			return new AssemblyCatalog(Assembly.GetAssembly(typeof(IMainView)), ConfigureExportsForEveryTypeInApplication());
		}

		private static RegistrationBuilder ConfigureExportsForPlugins()
		{
			return ConfigureExportsForEveryTypeInApplication();
		}

		private static RegistrationBuilder ConfigureExportsForEveryTypeInApplication()
		{
			var builder = new RegistrationBuilder();

			builder.ForTypesImplementingInterfaceEndingWith("Factory").ExportInterfaces().SetCreationPolicy(CreationPolicy.Shared);

			ConfigureExportsForCore(builder);
			ConfigureExportsForPresentation(builder);

			builder.ExportAllSingletons();

			return builder;
		}

		private static void ConfigureExportsForCore(RegistrationBuilder builder)
		{
			builder.ForTypesImplementingInterfaceEndingWith("Service").ExportInterfaces().SetCreationPolicy(CreationPolicy.Shared);

			builder.ForTypesDerivedFrom<IArchiveFormatLoader>().Export<IArchiveFormatLoader>();
			builder.ForTypesDerivedFrom<IFileNameGenerator>().Export<IFileNameGenerator>();

			builder.ForType<FileSystem>().Export<IFileSystem>();
			builder.ForType<TempFileProvider>().Export<TempFileProvider>();
			builder.ForType<FileOpeningService>().Export<FileOpeningService>();
		
			builder.ForType<ArchiveLoadingService>().SelectConstructor(ctors => ctors[0], (param, import) =>
			{
				if(param.Name == "archiveLoaders")
				{
					import.AsMany();
				}
			});
		}

		private static void ConfigureExportsForPresentation(RegistrationBuilder builder)
		{
			builder.ForTypesImplementingInterfaceEndingWith("View").ExportInterfaces().SetCreationPolicy(CreationPolicy.Shared);
			builder.ForTypesImplementingInterfaceEndingWith("ViewModel").ExportInterfaces().SetCreationPolicy(CreationPolicy.Shared);
			
			builder.ForTypesEndingWith("Presenter").Export().SetCreationPolicy(CreationPolicy.Shared);
			builder.ForTypesEndingWith("ViewModel").Export().SetCreationPolicy(CreationPolicy.Shared);
			builder.ForTypesEndingWith("Panel").Export().SetCreationPolicy(CreationPolicy.Shared);

			builder.ForTypesDerivedFrom<INewArchiveSettingsScreen>().Export<INewArchiveSettingsScreen>();
			builder.ForTypesDerivedFrom<IDialogLauncher>().Export<IDialogLauncher>();
			builder.ForTypesDerivedFrom<ICommand>().Export();
			builder.ForTypesDerivedFrom<ICommandLineHandler>().Export<ICommandLineHandler>();

			if(Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				builder.ForType<WindowsFileIconProvider>().ExportInterfaces();
			}
			else
			{
				builder.ForType<GenericFileIconProvider>().ExportInterfaces();
			}
		}

		private static PartBuilder ForTypesImplementingInterfaceEndingWith(this RegistrationBuilder builder, string nameSuffix)
		{
			return builder.ForTypesMatching(type =>
			{
				return !type.IsNestedPrivate && type.GetInterfaces().Any(iface => iface.Name.EndsWith(nameSuffix));
			});
		}

		private static PartBuilder ForTypesEndingWith(this RegistrationBuilder builder, string nameSuffix)
		{
			return builder.ForTypesMatching(type => !type.IsNestedPrivate && type.Name.EndsWith(nameSuffix));
		}

		private static void ExportAllSingletons(this RegistrationBuilder builder)
		{
			Predicate<PropertyInfo> isSingletonAccessorPredicate = property => property.Name == "Instance";
			Predicate<Type>         isSingletonPredicate         = type =>
			{
				return type.GetProperties().Any(property => isSingletonAccessorPredicate(property));
			};

			builder.ForTypesMatching(isSingletonPredicate)
			       .ExportProperties(isSingletonAccessorPredicate)
			       .SetCreationPolicy(CreationPolicy.Shared);
		}

		private static void FixTypeConvertersFromPlugins()
		{
			// Workaround to fix TypeConverters from plugins not working.
			// Based on http://stackoverflow.com/questions/3612909/why-is-this-typeconverter-not-working
			AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
			{
				var domain = (AppDomain)sender;

				return domain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == e.Name);
			};
		}

		/// <summary>
		/// Sets the current locale based on user settings.
		/// </summary>
		public static void ApplyLanguageFromSettings()
		{
			if(ApplicationSettings.Instance.Language == "OSLang")
				return;

			var chosenCulture = new CultureInfo(ApplicationSettings.Instance.Language);

			CultureInfo.DefaultThreadCurrentCulture   = chosenCulture;
			CultureInfo.DefaultThreadCurrentUICulture = chosenCulture;
		}
	}
}
