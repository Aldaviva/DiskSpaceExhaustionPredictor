
# DiskSpaceExhaustionPredictor
Based on historical TreeSize scans, predict when a disk will run out of free space.

## Problem
I want to know the deadline by which I need to buy a new hard drive for my NAS RAID.

## Solution
Look at TreeSize scan history and assume linear disk usage growth, which fits my usage quite accurately. This uses an [ordinary least-squares estimation](https://en.wikipedia.org/wiki/Ordinary_least_squares) for the linear regression.

## Requirements
- [Microsoft .NET Framework 4.7 or later](https://dotnet.microsoft.com/download)
- [JAM Software TreeSize Professional](https://www.jam-software.com/treesize/) (tested with 5.2)

## Prerequisites
1. Run TreeSize and scan the given drive.
1. Do this at least twice. The prediction accuracy will increase as the number of scans and duration of time covered by the scans increases.
1. You can look at a graph of the history of the drive's allocated bytes in the History tab.

## Usage
This is a console mode program. Run it from a command line like the Command Prompt or PowerShell.
```
>DiskSpaceExhaustionPredictor.exe D:
Analyzing disk usage over time, based on 31 scans, the most recent of which was on Tuesday, October 22, 2019.
Disk space on D:\ will be exhausted in about 148 days, around Wednesday, March 18, 2020.
```

## Development

### Requirements
- [Microsoft Visual Studio Community 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16) or better

### Building
1. Open the solution file in Visual Studio.
1. Build the solution with `Build → Build solution`.
1. If you don't care about debugging the program and don't want a folder full of DLLs to worry about, use the solution configuration toolbar dropdown menu to switch from Debug to Release.
1. The compiled program is `DiskSpaceExhaustionPredictor\bin\Release\DiskSpaceExhaustionPredictor.exe`, relative to the solution directory (or `Debug` for debug builds).
