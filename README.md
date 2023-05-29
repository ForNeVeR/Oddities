Oddities [![Status Zero][status-zero]][andivionian-status-classifier]
========
This repository groups several .NET libraries supporting old and obscure data formats.

Project History
---------------
This project started as part of the game reimplementation [O21][o21]. The original game is an old Windows game (from 3.1 era), and so it was necessary to implement several old Windows data formats to load the game data properly.

Currently, the following data formats are supported:
- [NE][wikipedia.ne] (aka "New Executable"), 16-bit `.exe` Windows binary.
- [DIB][microsoft.dib] (device-independent binary), encountered as part of the BMP format and in the NE resource table.
- [Windows Help File format][docs.winhelp] (aka WinHelp aka `.HLP`) and accompanying formats (often stored in a `.HLP` file):
  - [MRB (multi-resolution bitmap)][file-info.mrb],
  - [SHG (Segmented Hyper-Graphic)][file-info.shg].

If you encounter a case not handled by the library, don't hesitate to [open an issue][issues]! 

Read the corresponding sections below for each part of the library suite.

Documentation
-------------
- [Contributor Guide][docs.contributing]
- [License (MIT)][docs.license]
- [Code of Conduct (adapted from the Contributor Covenant)][docs.code-of-conduct]

Acknowledgments
---------------
- For the documentation on WinHelp, we'd like to thank:
  - Pete Davis and Mike Wallace, the authors of [Windows Undocumented File Formats][book.windows-undocumented-file-formats],
  - Manfred Winterhoff, the author of [the documentation][docs.winhelp],
  - Paul Wise and other contributors of [helpdeco][].

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-zero-
[book.windows-undocumented-file-formats]: https://a.co/d/dq5fCoj
[docs.code-of-conduct]: CODE_OF_CONDUCT.md
[docs.contributing]: CONTRIBUTING.md
[docs.license]: LICENSE.md
[docs.winhelp]: http://www.oocities.org/mwinterhoff/helpfile.htm
[file-info.mrb]: https://fileinfo.com/extension/mrb
[file-info.shg]: https://fileinfo.com/extension/shg
[helpdeco]: https://github.com/pmachapman/helpdeco
[issues]: https://github.com/ForNeVeR/Oddities/issues 
[microsoft.dib]: https://learn.microsoft.com/en-us/windows/win32/gdi/device-independent-bitmaps
[o21]: https://github.com/ForNeVeR/O21
[status-zero]: https://img.shields.io/badge/status-zero-lightgrey.svg
[wikipedia.ne]: https://en.wikipedia.org/wiki/New_Executable
