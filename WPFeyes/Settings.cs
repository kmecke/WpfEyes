using System;
using System.Xml.Serialization;

namespace WPFeyes
{
    [Serializable()]
    [XmlRoot("EyesSettings")]
    public class Settings
    {
        public Settings()
        { }

        public Settings(string v)
        {
            RefreshRate = 20;
            Opacity = 0.3f;
            ShowResizeGrip = true;
            ShowXYPosition = false;
            DragMove = true;
            MostTop = true;
            Color = "#FF606060";
            eyePos.x = 100;
            eyePos.y = 100;
            eyeSize.Height = 100;
            eyeSize.Width = 100;
        }

        [XmlElement("RefreshRate")]
        public float RefreshRate { get; set; }

        [XmlElement("Opacity")]
        public float Opacity { get; set; }

        [XmlElement("ShowResizeGrip")]
        public Boolean ShowResizeGrip { get; set; }

        [XmlElement("ShowXYPosition")]
        public Boolean ShowXYPosition { get; set; }

        [XmlElement("DragMove")]
        public Boolean DragMove { get; set; }

        [XmlElement("MostTop")]
        public Boolean MostTop { get; set; }

        [XmlElement("Color")]
        public string Color { get; set; }

        [XmlElement("EyePosition")]
        public EyePosition eyePos { get; set; }

        [XmlElement("EyeSize")]
        public EyeSize eyeSize { get; set; }
    }

    public class EyePosition
    {
        [XmlElement("x")]
        public double x { get; set; }

        [XmlElement("y")]
        public double y { get; set; }
    }

    public class EyeSize
    {
        [XmlElement("Height")]
        public double Height { get; set; }

        [XmlElement("Width")]
        public double Width { get; set; }
    }

}
