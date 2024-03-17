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

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("specializationType")]
        public string SpecializationTypeOld { get; set; }

        [XmlElement("size")]
        public SizeSettings Size { get; set; }

        [XmlElement("position")]
        public PositionSettings Position { get; set; }

        [XmlElement("anchorPointsFunction")]
        public string AnchorPointsFunction { get; set; }

        [XmlArray("display")]
        [XmlArrayItem("drawPath", typeof(PathDrawSettings))]
        [XmlArrayItem("drawText", typeof(TextDrawSettings))]
        [XmlArrayItem("drawSvgResource", typeof(SvgResourceDrawSettings))]
        public List<object> DisplayElements { get; set; }

        [XmlIgnore]
        public List<PathDrawSettings> DisplayPaths => DisplayElements.OfType<PathDrawSettings>().ToList();

        [XmlIgnore]
        public List<TextDrawSettings> DisplayTexts => DisplayElements.OfType<TextDrawSettings>().ToList();

        [XmlIgnore]
        public List<SvgResourceDrawSettings> DisplaySvgResources => DisplayElements.OfType<SvgResourceDrawSettings>().ToList();

        [XmlElement("stereotypesVisualSettings")]
        public StereotypesVisualSettingsPersistable StereotypesVisualSettings { get; set; }

        [XmlElement("defaultSize")]
        public SizeSettings DefaultSize { get; set; }

        [XmlElement("autoResizeEnabledByDefault")]
        public bool AutoResizeEnabledByDefault { get; set; } = true;

        [XmlArray("childElementVisualSettings")]
        [XmlArrayItem("childElementVisualSetting")]
        public List<ElementVisualSettingsPersistable> ChildElementVisualSettings { get; set; } = new List<ElementVisualSettingsPersistable>();

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

        [XmlElement("size")]
        public SizeSettings SizeSettings { get; set; }

        [XmlElement("position")]
        public PositionSettings PositionSettings { get; set; }
    }

    public class SvgResourceDrawSettings
    {
        [XmlElement("condition")]
        public string ConditionFunction { get; set; }

        [XmlElement("path")]
        public string ResourcePath { get; set; } // a relative path

        [XmlElement("size")]
        public SizeSettings SizeSettings { get; set; }

        [XmlElement("position")]
        public PositionSettings PositionSettings { get; set; }

        [XmlIgnore]
        public string AbsolutePath { get; set; }
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