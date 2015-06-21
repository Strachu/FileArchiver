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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using FileArchiver.Archive.SevenZip.Settings;
using FileArchiver.Core.Archive;
using FileArchiver.Core.Utils.File;

using Path = FileArchiver.Core.ValueTypes.Path;

namespace FileArchiver.Archive.SevenZip.SevenZipCommunication
{
	/// <summary>
	/// A wrapper class encapsulating the invocation of the 7-zip application.
	/// </summary>
	/// <remarks>
	/// A special version of 7-zip available at https://github.com/Strachu/7-zip is required for the class
	/// to work properly. The modifications are required to allow meaningful progress reporting.
	/// </remarks>
	internal class SevenZip
	{
		public bool IsSevenZipArchive(Path archivePath)
		{
			Contract.Requires(archivePath != null);

			using(var process = SevenZipProcess.Execute(String.Format("l \"{0}\" do_not_list_files", archivePath)))
			{
				var fileListReader = new FileListingReader(process.StandardOutput);
				try
				{
					var archiveProperties = fileListReader.ReadArchiveProperties();

					return archiveProperties["Type"] == "7z";
				}
				catch(EndOfStreamException)
				{
					return false;
				}
			}
		}

		public ArchiveInfo ReadArchiveInfo(Path archivePath, CancellationToken cancelToken)
		{
			Contract.Requires(archivePath != null);

			using(var process = SevenZipProcess.Execute(String.Format("l -slt -t7z \"{0}\"", archivePath)))
			{
				var fileListReader = new FileListingReader(process.StandardOutput);
				
				var archiveInfo      = fileListReader.ReadArchiveProperties();
				var archiveEntries   = fileListReader.ReadEntries().ToList();

				var isSolid          = (archiveInfo["Solid"] == "+");
				var compressionLevel = CompressionLevelParser.ParseFromEntryList(archiveEntries);
				var files            = ParseEntries(archiveEntries, cancelToken);

				return new ArchiveInfo(files, compressionLevel, isSolid);
			}
		}

		private IEnumerable<FileEntry> ParseEntries(IEnumerable<IDictionary<string, string>> entries,
		                                            CancellationToken cancelToken)
		{
			var hierarchyBuilder = new FileEntryHierarchyBuilder();

			foreach(var entryProperties in entries)
			{
				cancelToken.ThrowIfCancellationRequested();

				var destinationPath = new Path(entryProperties["Path"]).ParentDirectory;
				var file            = EntryParser.ParseEntry(entryProperties);

				hierarchyBuilder.AddFile(destinationPath, file);
			}

			return hierarchyBuilder.Build().Select(AddSevenZipEntryDataIfDoesNotExist);
		}

		/// <summary>
		/// Adds an empty seven zip data to the entry if it does not exist.
		/// </summary>
		/// <remarks>
		/// This is needed when there is no information about directories in the archive.
		/// </remarks>
		private FileEntry AddSevenZipEntryDataIfDoesNotExist(FileEntry original)
		{
			var entryData = new SevenZipEntryData(original.Path, solidBlockIndex: null);

			return original.BuildCopy()
			               .WithArchiveData(original.ArchiveData ?? entryData)
			               .WithFiles(original.Files.Select(AddSevenZipEntryDataIfDoesNotExist).ToArray())
			               .Build();
		}

		public void ExtractFromArchive(Path archivePath,
		                               IReadOnlyCollection<FileDestinationPair> files,
		                               CancellationToken cancelToken,
		                               IProgress<long> progress)
		{
			Contract.Requires(archivePath != null);
			Contract.Requires(files != null);
			Contract.Requires(Contract.ForAll(files, file => file != null));

			progress               = progress ?? new Progress<long>();
			var extractionProgress = new CompositeFileProgress(progress.Report);

			using(var process = StartExtraction(archivePath, files.Select(x => x.File).ToList()))
			{
				// Sample output:
				//
				//7-Zip (A) 9.20  Copyright (c) 1999-2010 Igor Pavlov  2010-11-18
				//
				//Processing archive: Test.7z
				//
				//Skipping    Directory\1.txt
				//Extracting  1.txt
				//Extracting  Directory\Test.7z
				//
				//Everything is Ok
				//
				//Files: 3
				//Size:       245
				//Compressed: 481
				//

				SkipHeader(process.StandardError);

				while(true)
				{
					cancelToken.ThrowIfCancellationRequested();

					var line = process.StandardError.ReadLine();
					if(String.IsNullOrEmpty(line))
						break;

					var operation = ExtractionOperationLine.Parse(line);

					var entry = files.SingleOrDefault(x => x.File.GetArchiveEntryData().IdInArchive == operation.FileId);
					if(entry == null)
						continue;

					if(operation.Action == Action.Skipping)
					{
						extractionProgress.GetProgressForNextFile().Report(entry.File.Size);
						continue;
					}

					var partialStream = new PartialReadStream(process.StandardOutput.BaseStream, entry.File.Size);

					partialStream.CopyToNewFileWithProgress(entry.DestinationPath, cancelToken,
					                                        extractionProgress.GetProgressForNextFile());
				}
			}
		}

		private SevenZipProcess StartExtraction(Path archivePath, IReadOnlyCollection<FileEntry> files)
		{
			// There is a limit to a maximum length of command line invocation. To reduce the length of invocation
			// if all files of a directory are to be extracted, then only the directory is passed as command argument.
			
			var collapsedFileList     = DirectoryCollapser.Collapse(files).ToList();
			var originalFileNames     = collapsedFileList.Select(x => x.GetArchiveEntryData().IdInArchive);
			var filePathsCommandLine  = FilePathsToCommandParameters(originalFileNames);

			var commandLineParameters = String.Format("x -t7z -so \"{0}\" {1}", archivePath, filePathsCommandLine);

			return SevenZipProcess.Execute(commandLineParameters);
		}

		private void SkipHeader(TextReader reader)
		{
			// Sample header:
			//
			// 7-Zip (A) 9.20  Copyright (c) 1999-2010 Igor Pavlov  2010-11-18
			//
			// Processing archive: Test.7z
			//

			for(int i = 0; i < 5; ++i)
			{
				reader.ReadLine();
			}
		}

		public void AddFilesToArchive(Path archivePath,
		                              IEnumerable<Path> files,
		                              CompressionLevel compressionLevel,
		                              bool useSolidCompression,
		                              CancellationToken cancelToken,
		                              IProgress<double> progress)
		{
			var solidModeSwitch        = String.Format("-ms={0}", (useSolidCompression) ? "on" : "off");
			var compressionLevelSwitch = String.Format("-mx{0}",  (int)compressionLevel);
			var commandLineParameters  = String.Format("a -t7z {0} {1} \"{2}\"", compressionLevelSwitch, solidModeSwitch,
			                                                                     archivePath);

			UpdateArchive(commandLineParameters, files.Select(x => x.ToString()), cancelToken, progress);
		}

		public void RemoveFilesFromArchive(Path archivePath,
		                                   Path newArchivePath,
		                                   IEnumerable<string> files,
		                                   CancellationToken cancelToken,
		                                   IProgress<double> progress)
		{
			var commandLineParameters = String.Format("u \"{0}\" -u- -up1q0r0x0y0z0w0!\"{1}\"", archivePath, newArchivePath);

			UpdateArchive(commandLineParameters, files, cancelToken, progress);
		}

		private void UpdateArchive(string commandParameters,
		                           IEnumerable<string> files,
		                           CancellationToken cancelToken,
		                           IProgress<double> progress)
		{
			var commandLineParameters = String.Format("{0} {1}", commandParameters, FilePathsToCommandParameters(files));

			using(var process = SevenZipProcess.Execute(commandLineParameters))
			{
				while(true)
				{
					cancelToken.ThrowIfCancellationRequested();

					var percentageDone = ReadPercent(process.StandardOutput);
					if(percentageDone == null)
						break;

					progress.Report(percentageDone.Value * 0.01);
				}
			}
		}

		private string FilePathsToCommandParameters(IEnumerable<string> files)
		{
			var argumentListBuilder = new StringBuilder();
			foreach(var entry in files)
			{
				argumentListBuilder.Append("\"");
				argumentListBuilder.Append(entry);
				argumentListBuilder.Append("\" ");
			}
			argumentListBuilder.Remove(argumentListBuilder.Length - 1, 1);

			return argumentListBuilder.ToString();
		}

		/// <summary>
		/// Reads the next percentage from the stream skipping everything else.
		/// </summary>
		/// <param name="reader">
		/// The reader.
		/// </param>
		/// <returns>
		/// Percentage or null if end of stream has been encountered.
		/// </returns>
		private int? ReadPercent(TextReader reader)
		{
			// Sample output:
			//
			//7-Zip (A) 9.20  Copyright (c) 1999-2010 Igor Pavlov  2010-11-18
			//Scanning
			//
			//Creating archive test3.7z
			//
			//Compressing  TestDirectory\WindowsFormsApplication1\Properties\AssemblyInfo.cs          0%      
			//Compressing  TestDirectory\WindowsFormsApplication1\Form1.cs          0%      
			//Compressing  TestDirectory\WindowsFormsApplication1\Form1.Designer.cs          0%      
			//Compressing  TestDirectory\7za.exe          0%    0%      
			//Compressing  TestDirectory\DXSDK June 2010.exe          0%    0%     0%    0%    0%    0%    0%    0%    0%    0%    0%    0%    1%    1%    1%    1%    1%    1%    2%    2%    2%    2%    2%    2%    2%    2%    2%    2%
			//Compressing  TestDirectory\WindowsFormsApplication1\bin\Debug\WindowsFormsApplication1.exe         99%      
			//Compressing  TestDirectory\WindowsFormsApplication1\bin\Debug\WindowsFormsApplication1.vshost.exe         99%      
			//Compressing  TestDirectory\WindowsFormsApplication1\obj\Debug\TempPE\Properties.Resources.Designer.cs.dll         99%   99%   99%   99%   99%      
			//
			//Everything is Ok

			var lastCharactersRead = new char[3];
			while(true)
			{
				int characterCode = reader.Read();
				if(characterCode == -1)
					return null;

				var character = (char)characterCode;
				if(character == '%')
				{
					if(Char.IsDigit(lastCharactersRead[0]))
					{
						return 100;
					}

					if(Char.IsDigit(lastCharactersRead[1]))
					{
						var percentAsString = new string(lastCharactersRead.Skip(1).ToArray());

						return Int32.Parse(percentAsString);
					}

					var percentAsString2 = new string(lastCharactersRead.Skip(2).ToArray());

					return Int32.Parse(percentAsString2);
				}

				lastCharactersRead[0] = lastCharactersRead[1];
				lastCharactersRead[1] = lastCharactersRead[2];
				lastCharactersRead[2] = character;
			}
		}

	}
}
