using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class ClassVisualSettings
    {
        [XmlElement("specializationType")]
        public string SpecializationType { get; set; }

        [XmlArray("display")]
        [XmlArrayItem("draw")]
        public List<ClassDisplaySettings> DisplayPaths { get; set; }

        [XmlElement("defaultSize")]
        public SizeSettings DefaultSize { get; set; }

        [XmlElement("text")]
        public TextSettings TextSettings { get; set; }

        [XmlElement("showAttributes")]
        public bool ShowAttributes { get; set; }

        [XmlElement("showOperations")]
        public bool ShowOperations { get; set; }

        [XmlArray("childElementVisualSettings")]
        [XmlArrayItem("childElementVisualSetting")]
        public List<ChildElementVisualSettings> ChildElementVisualSettings { get; set; }
    }

    public class ClassDisplaySettings
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

    public class ChildElementVisualSettings
    {
        [XmlAttribute("specializationType")]
        public string SpecializationType { get; set; }

        [XmlElement("showEnabled")]
        public bool ShowEnabled { get; set; }

        [XmlElement("showByDefault")]
        public bool ShowByDefault { get; set; }

        [XmlElement("text")]
        public TextSettings TextSettings { get; set; }
    }

    public class TextSettings
    {
        [XmlElement("fontColor")]
        public string FontColor { get; set; }

        [XmlElement("fontSize")]
        public string FontSize { get; set; }

        [XmlElement("fontWeight")]
        public string FontWeight { get; set; }

        [XmlElement("fontFamily")]
        public string FontFamily { get; set; }

        [XmlElement("fontStyle")]
        public string FontStyle { get; set; }

        [XmlElement("whiteSpace")]
        public string WhiteSpace { get; set; }

        [XmlElement("textAlign")]
        public string TextAlign { get; set; }

        [XmlElement("size")]
        public SizeSettings SizeSettings { get; set; }

        [XmlElement("position")]
        public PositionSettings PositionSettings { get; set; }
    }

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