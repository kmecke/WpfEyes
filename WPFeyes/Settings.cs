using System;
using System.Xml.Serialization;

namespace WPFeyes
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("EyesSettings")]
    public class Settings
    {
        private string v;

        public Settings()
        { }

        public Settings(string v)
        {
            RefreshRate = 20;
            Opacity = 30;
            ShowResizeGrip = true;
            ShowXYPosition = false;
            DragMove = true;
            MostTop = true;
            Color = "#FF808080";
            eyePos.x = 100;
            eyePos.y = 100;
            eyeSize.Height = 100;
            eyeSize.Width = 100;
        }

        [System.Xml.Serialization.XmlElement("RefreshRate")]
        public int RefreshRate { get; set; }

        [System.Xml.Serialization.XmlElement("Opacity")]
        public int Opacity { get; set; }

        [System.Xml.Serialization.XmlElement("ShowResizeGrip")]
        public Boolean ShowResizeGrip { get; set; }

        [System.Xml.Serialization.XmlElement("ShowXYPosition")]
        public Boolean ShowXYPosition { get; set; }

        [System.Xml.Serialization.XmlElement("DragMove")]
        public Boolean DragMove { get; set; }

        [System.Xml.Serialization.XmlElement("MostTop")]
        public Boolean MostTop { get; set; }

        [System.Xml.Serialization.XmlElement("Color")]
        public string Color { get; set; }

        [System.Xml.Serialization.XmlElement("eyePos")]
        public EyePosition eyePos { get; set; }

        [System.Xml.Serialization.XmlElement("eyeSize")]
        public EyeSize eyeSize { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("EyePosition")]
    public class EyePosition
    {
        [System.Xml.Serialization.XmlElement("x")]
        public float x { get; set; }

        [System.Xml.Serialization.XmlElement("y")]
        public float y { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("EyeSize")]
    public class EyeSize
    {
        [System.Xml.Serialization.XmlElement("Height")]
        public float Height { get; set; }

        [System.Xml.Serialization.XmlElement("Width")]
        public float Width { get; set; }
    }

}
