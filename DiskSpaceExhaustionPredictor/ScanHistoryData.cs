using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiskSpaceExhaustionPredictor {

    [XmlRoot(ElementName = "SizeData")]
    public class SizeData {

        [XmlAttribute(AttributeName = "Size")]
        public string sizeBytes { get; set; }

        [XmlAttribute(AttributeName = "Allocated")]
        public string allocatedBytes { get; set; }

        [XmlAttribute(AttributeName = "Files")]
        public string filesCount { get; set; }

    }

    [XmlRoot(ElementName = "Root")]
    public class Scan {

        [XmlElement(ElementName = "Path")]
        public string path { get; set; }

        //redundant with child Path element
//        [XmlAttribute(AttributeName = "path")]
//        public string path { get; set; }

        [XmlElement(ElementName = "Date")]
        public string dateOA { get; set; }

        [XmlElement(ElementName = "Version")]
        public string treeSizeVersion { get; set; }

        [XmlElement(ElementName = "SizeData")]
        public SizeData sizeData { get; set; }

        [XmlElement(ElementName = "Freespace")]
        public string freeSpaceBytes { get; set; }

    }

    [XmlRoot(ElementName = "SCANS")]
    public class ScanHistory {

        [XmlElement(ElementName = "Root")]
        public List<Scan> scans { get; set; }

    }

}