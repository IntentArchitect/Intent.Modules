﻿using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class AssociationVisualSettingsPersistable
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

        [XmlElement("lineType")]
        public AssociationVisualLineType LineType { get; set; }

        [XmlElement("lineColor")]
        public string LineColor { get; set; }

        [XmlElement("lineWidth")]
        public string LineWidth { get; set; }

        [XmlElement("lineDashArray")]
        public string LineDashArray { get; set; }

        [XmlElement("flowDirectionReversed")]
        public bool FlowDirectionReversed { get; set; }
        public bool ShouldSerializeFlowDirectionReversed() => FlowDirectionReversed;

        [XmlElement("sourceEnd")]
        public AssociationEndVisualSettings SourceEnd { get; set; }

        [XmlElement("targetEnd")]
        public AssociationEndVisualSettings TargetEnd { get; set; }
    }

    public class AssociationEndVisualSettings
    {
        [XmlElement("primaryLabel")]
        public string PrimaryLabel { get; set; }

        [XmlElement("secondaryLabel")]
        public string SecondaryLabel { get; set; }

        [XmlElement("pointIndicator")]
        public AssociationPointerSettings PointIndicator { get; set; }

        [XmlElement("navigableIndicator")]
        public AssociationPointerSettings NavigableIndicator { get; set; }
    }

    public class AssociationPointerSettings
    {
        [XmlElement("fillColor")]
        public string FillColor { get; set; }

        [XmlElement("lineColor")]
        public string LineColor { get; set; }

        [XmlElement("lineWidth")]
        public string LineWidth { get; set; }

        [XmlElement("lineDashArray")]
        public string LineDashArray { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }
    }
}