using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ADE.Configurations.Objects
{
    [XmlRoot(ElementName = "srv")]
    public class srv
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "dbn")]
    public class dbn
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "uid")]
    public class uid
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "pwd")]
    public class pwd
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "cna")]
    public class cna
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "cpa")]
    public class cpa
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "mainConfig")]
    public class MainConfig
    {
        [XmlElement(ElementName = "srv")]
        public srv Srv { get; set; }
        [XmlElement(ElementName = "dbn")]
        public dbn Dbn { get; set; }
        [XmlElement(ElementName = "uid")]
        public uid Uid { get; set; }
        [XmlElement(ElementName = "pwd")]
        public pwd Pwd { get; set; }
        [XmlElement(ElementName = "cna")]
        public cna Cna { get; set; }
        [XmlElement(ElementName = "cpa")]
        public cpa Cpa { get; set; }
    }
}
