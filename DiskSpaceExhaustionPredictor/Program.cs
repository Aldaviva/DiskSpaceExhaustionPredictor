using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Accord.Statistics.Models.Regression.Linear;

namespace DiskSpaceExhaustionPredictor {

    internal static class Program {

        private static readonly string HISTORY_FILE_PATH =
            Environment.ExpandEnvironmentVariables(@"%APPDATA%\JAM Software\TreeSize Professional\scanhistory.xml");

        private static int Main(string[] args) {
            string driveLetter;
            if (args.Length < 1) {
                Console.WriteLine($"Usage: {Process.GetCurrentProcess().ProcessName} C:\n" +
                                  "where C: is the letter of the drive you want to analyze.\n" +
                                  "Requires a history of TreeScan Professional scans for this drive.");
                return 1;
            } else {
                driveLetter = Path.GetFullPath(args[0]);
            }

            var deserializer = new XmlSerializer(typeof(ScanHistory));
            ScanHistory scanHistory;
            try {
                using (var fileStream = new FileStream(HISTORY_FILE_PATH, FileMode.Open)) {
                    scanHistory = (ScanHistory) deserializer.Deserialize(fileStream);
                }
            } catch (FileNotFoundException) {
                Console.WriteLine("Error: TreeSize Professional scan history file was not found while trying to load historical " +
                                  "records of disk space usage.\n" +
                                  "Ensure TreeSize Professional 5.2 or similar is installed, " +
                                  "and than you have run at least two scans at different times.");
                return 2;
            }

            IEnumerable<(double dateOA, long allocatedBytes)> matchingRows = (from scan in scanHistory.scans
                    orderby scan.dateOA
                    where Path.GetFullPath(scan.path) == driveLetter
                    select (scan.dateOA, allocatedBytes: long.Parse(scan.sizeData.allocatedBytes)))
                .ToList();

            switch (matchingRows.Count()) {
                case 0:
                    Console.WriteLine(
                        $"Error: Historical scans were loaded successfully, but drive {driveLetter} has never been scanned.\n" +
                        $"There were {scanHistory.scans.Count - matchingRows.Count():N0} other, unrelated scan(s) in the TreeSize history.\n" +
                        "In order to analyze the disk space usage over time, you must scan the disk at least twice.");
                    return 3;
                case 1:
                    Console.WriteLine(
                        $"Error: Historical scans were loaded successfully, but drive {driveLetter} has only been scanned once.\n" +
                        $"There were {scanHistory.scans.Count - matchingRows.Count():N0} other, unrelated scan(s) in the TreeSize history.\n" +
                        "In order to analyze the disk space usage over time, you must scan the disk at least twice.");
                    return 3;
                default:
                    break;
            }

            Console.WriteLine($"Analyzing disk usage over time, based on {matchingRows.Count()} scans, the most recent of which was " +
                              $"on {DateTime.FromOADate(matchingRows.Last().dateOA):D}.");

            double[] dateSequence = matchingRows.Select(x => x.dateOA).ToArray();
            double[] allocatedByteSequence = matchingRows.Select(x => (double) x.allocatedBytes).ToArray();

            SimpleLinearRegression simpleLinearRegression = SimpleLinearRegression.FromData(dateSequence, allocatedByteSequence);
            double m = simpleLinearRegression.Slope;
            double b = simpleLinearRegression.Intercept;

            if (m < 0) {
                Console.WriteLine($"Disk space usage on {driveLetter} appears to be decreasing over time, " +
                                  "and will therefore never run out.");
                return 0;
            }

            long diskCapacity = new DriveInfo(driveLetter).TotalSize;

            double capacityExhaustionDateExcel = (diskCapacity - b) / m;
            DateTime capacityExhaustionDate = DateTime.FromOADate(capacityExhaustionDateExcel);
            double daysRemaining = capacityExhaustionDate.Subtract(DateTime.Now).TotalDays;
            Console.WriteLine($"Disk space on {driveLetter} will be exhausted in about {daysRemaining:N0} days, " +
                              $"around {capacityExhaustionDate:D}.");

            return 0;
        }

    }

}