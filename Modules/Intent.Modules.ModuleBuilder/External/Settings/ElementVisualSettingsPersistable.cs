using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ElementVisualSettingsPersistable
    {
        private string _specializationType;
        [XmlAttribute("type")]
        public string SpecializationType
        {
            get => _specializationType ?? SpecializationTypeOld;
            set => _specializationType = value;
        }

        [XmlElement("specializationType")]
        public string SpecializationTypeOld { get; set; }

        [XmlElement("size")]
        public SizeSettings Size { get; set; }

        [XmlElement("position")]
        public PositionSettings Position { get; set; }

        [XmlArray("display")]
        [XmlArrayItem("drawPath", typeof(PathDrawSettings))]
        [XmlArrayItem("drawText", typeof(TextDrawSettings))]
        public List<object> DisplayElements { get; set; }

        [XmlIgnore]
        public List<PathDrawSettings> DisplayPaths => DisplayElements.Where(x => (x is PathDrawSettings)).Cast<PathDrawSettings>().ToList();

        [XmlIgnore]
        public List<TextDrawSettings> DisplayTexts => DisplayElements.Where(x => (x is TextDrawSettings)).Cast<TextDrawSettings>().ToList();

        [XmlElement("stereotypesVisualSettings")]
        public StereotypesVisualSettingsPersistable StereotypesVisualSettings { get; set; }

        [XmlElement("defaultSize")]
        public SizeSettings DefaultSize { get; set; }

        //[XmlElement("text")]
        //public TextSettings TextSettings { get; set; }

        //[XmlElement("showAttributes")]
        //public bool ShowAttributes { get; set; }

        //[XmlElement("showOperations")]
        //public bool ShowOperations { get; set; }

        [XmlArray("childElementVisualSettings")]
        [XmlArrayItem("childElementVisualSetting")]
        public List<ElementVisualSettingsPersistable> ChildElementVisualSettings { get; set; }
    }

    public class StereotypesVisualSettingsPersistable
    {
        [XmlElement("size")]
        public SizeSettings Size { get; set; }

        [XmlElement("position")]
        public PositionSettings Position { get; set; }

    }

    public class PathDrawSettings
    {
        [XmlElement("condition")]
        public string ConditionFunction { get; set; }

        [XmlElement("path")]
        public string PathFunction { get; set; }

        [XmlElement("lineColor")]
        public string LineColor { get; set; }

        [XmlElement("lineWidth")]
        public string LineWidth { get; set; }

        [XmlElement("lineDashArray")]
        public string LineDashArray { get; set; }

        [XmlElement("fillColor")]
        public string FillColor { get; set; }
    }

    public class TextDrawSettings
    {
        [XmlElement("condition")]
        public string ConditionFunction { get; set; }

        [XmlElement("text")]
        public string TextFunction { get; set; }

        [XmlElement("style")]
        public string StyleFunction { get; set; }

        //[XmlElement("fontColor")]
        //public string FontColor { get; set; }

        //[XmlElement("fontSize")]
        //public string FontSize { get; set; }

        //[XmlElement("fontWeight")]
        //public string FontWeight { get; set; }

        //[XmlElement("fontFamily")]
        //public string FontFamily { get; set; }

        //[XmlElement("fontStyle")]
        //public string FontStyle { get; set; }

        //[XmlElement("whiteSpace")]
        //public string WhiteSpace { get; set; }

        //[XmlElement("textAlign")]
        //public string TextAlign { get; set; }

        [XmlElement("size")]
        public SizeSettings SizeSettings { get; set; }

        [XmlElement("position")]
        public PositionSettings PositionSettings { get; set; }
    }

    //public class ChildElementVisualSettings
    //{
    //    [XmlAttribute("specializationType")]
    //    public string SpecializationType { get; set; }

    //    [XmlElement("showEnabled")]
    //    public bool ShowEnabled { get; set; }

    //    [XmlElement("showByDefault")]
    //    public bool ShowByDefault { get; set; }

    //    [XmlElement("text")]
    //    public TextDrawSettings TextSettings { get; set; }
    //}

    public class PositionSettings
    {
        [XmlAttribute("x")]
        public string X { get; set; }

        [XmlAttribute("y")]
        public string Y { get; set; }
    }

    public class SizeSettings
    {
        [XmlAttribute("width")]
        public string Width { get; set; }

        [XmlAttribute("height")]
        public string Height { get; set; }
    }
}