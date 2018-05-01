using System;

namespace WPFeyes
{
    [Serializable()]
    public class Settings
    {
        [System.Xml.Serialization.XmlElement("RefreshRate")]
        public string RefreshRate { get; set; }


    }
}
