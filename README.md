# FileArchiver
<p align="center">
<img src="https://raw.githubusercontent.com/wiki/Strachu/FileArchiver/Screenshots/Main%20window.png"/><br/>
<a href="https://github.com/Strachu/FileArchiver/wiki/Screenshots">More screenshots</a>
</p>
**FileArchiver** is an application for creating, manipulating and extracting of file archives such as ZIP and 7Z files.
The main distinct feature of FileArchiver is that it is optimized for batched manipulation of archives.
While other applications do operations on an archive immediately, FileArchiver does all operations in memory and applies them
after a user chooses the save operation. In this way, during multiple operations the archive is rewritten only once for all
operations instead of a rewrite issued for every operation as is done in other zip utilities.

# Features
- In memory archive operation batching
- Easy to use and intuitive interface
- Support for creation, modification and extraction of archive files saved in formats: **.7z**, **.zip**, **.tar**, **.gz** and any of their combinations such as **.tar.gz**
- Simple creation of multiple nested archives such as .tar.gz or .tar.7z
- Pluggable architecture allowing for new functionality such as new formats to be added by implementing appropriate interfaces 
and putting the resulting assembly in the Plugins directory
- Multiple languages support (currently only English and Polish)
- Integration with a context menu of Windows's File Explorer allowing for fast file packing and archive extraction

# Requirements
To run the application you need:
- Windows 7 or later (other operating systems are not supported at the moment)
- [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

Additionally, if you want to compile the application you need:
- Microsoft Visual Studio 2013 or later

# Libraries
The application uses the following libraries:
- Windows Forms for the creation of graphical user interface
- [SharpCompress 0.11](https://github.com/adamhathcock/sharpcompress) to enable support for .zip, .tar and .gz archives.
- A modified version of [7-zip 9.20](http://www.7-zip.org/) command line available at https://github.com/Strachu/7-zip to add support for .7z archives.
- A C# port of [Liferay Nativity](https://github.com/liferay/liferay-nativity) available at https://github.com/Strachu/liferay-nativity for integration with a context menu of File Explorer
- [MEF](https://msdn.microsoft.com/en-us/library/dd460648%28v=vs.110%29.aspx) as a DI container and to enable the support for plugins
- [Code contracts](https://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970) for preconditions checking
- [NUnit 2.6.4](http://www.nunit.org/) as a unit test framework
- [FakeItEasy 1.25.2](https://github.com/FakeItEasy/FakeItEasy) for the creation of stubs and mocks in unit tests
- [System.IO.Abstractions](https://github.com/tathamoddie/System.IO.Abstractions) to help in mocking of the file system during unit tests

# Tools
During the creation of the application the following tools were used:
- Microsoft Visual Studio 2013 with the following extensions:
  - [File Nesting](https://visualstudiogallery.msdn.microsoft.com/3ebde8fb-26d8-4374-a0eb-1e4e2665070c)
  - [License Header Manager](https://visualstudiogallery.msdn.microsoft.com/5647a099-77c9-4a49-91c3-94001828e99e)
  - [ResXManager](https://visualstudiogallery.msdn.microsoft.com/3b64e04c-e8de-4b97-8358-06c73a97cc68)
  - [Code Alignment](https://visualstudiogallery.msdn.microsoft.com/7179e851-a263-44b7-a177-1d31e33c84fd/)
  - [NUnit Test Adapter](https://visualstudiogallery.msdn.microsoft.com/6ab922d0-21c0-4f06-ab5f-4ecd1fe7175d)
  - [Visual Studio Spell Checker](https://visualstudiogallery.msdn.microsoft.com/a23de100-31a1-405c-b4b7-d6be40c3dfff)
  - [VisualSVN] (https://visualstudiogallery.msdn.microsoft.com/DBD60715-FE57-44B5-ABEA-F18618068C1E) (before the migration to GitHub)
  - [Sancastle Help File Builder](https://github.com/EWSoftware/SHFB)
- [Git](https://git-scm.com/)
- [Git Extensions](https://github.com/gitextensions/gitextensions)
- [TortoiseGit](https://code.google.com/p/tortoisegit/)
- [Subversion](https://subversion.apache.org/) (before the migration to GitHub)
- [TortoiseSVN](http://tortoisesvn.net/) (before the migration to GitHub)
- [SlimTune profiler](https://code.google.com/p/slimtune/)

# License
FileArchiver is a free software distributed under the GNU GPL 3 or later license.

See LICENSE.txt and LICENSE_THIRD_PARTY.txt for more information.
