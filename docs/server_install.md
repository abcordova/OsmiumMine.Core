
# OsmiumMine Core Server Installation Documentation

Version 0.1.0

Copyright &copy; 2016-2017 Nihal Talur (0xFireball), IridiumIon Software. All Rights Reserved.

Except as otherwise noted, the content of this page is licensed under the Creative Commons Attribution 3.0 License, and code samples are licensed under the Apache 2.0 License.

## Using SSDB

The default SSDB will not work because its `EXISTS` command does not include hash sets.
Instead, you should use my [lightly modified SSDB fork](https://github.com/0xFireball/ssdb),
which fixes the issue server-side.