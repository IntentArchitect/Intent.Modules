using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.DriftReporting.Api;

namespace Intent.Modules.Common.DriftReporting.Reporting
{
    /// <summary>Everything the run's reporters reported, by Area, in the order the Areas were first asked for.</summary>
    internal sealed class DriftCollector
    {
        private readonly List<DriftArea> _areas = new();

        public IReadOnlyList<DriftArea> Areas => _areas;

        public IEnumerable<DriftFinding> Findings => _areas.SelectMany(x => x.Findings);

        public DriftArea Area(string name, IEnumerable<DriftCategory> checks)
        {
            name = string.IsNullOrWhiteSpace(name) ? "General" : name.Trim();
            var area = _areas.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));
            if (area == null)
            {
                _areas.Add(area = new DriftArea(name));
            }

            area.Declare(checks ?? Enumerable.Empty<DriftCategory>());
            return area;
        }
    }

    internal sealed class DriftArea : IDriftArea
    {
        private readonly HashSet<DriftCategory> _declared = new();
        private readonly List<DriftFinding> _findings = new();

        public DriftArea(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public IReadOnlyList<DriftFinding> Findings => _findings;
        public int Errors => _findings.Count(x => x.Severity == DriftSeverity.Error);
        public int Warnings => _findings.Count(x => x.Severity == DriftSeverity.Warning);

        /// <summary>
        /// The categories the report shows for the Area, in their fixed order: those declared, and any other that has
        /// findings.
        /// </summary>
        public IReadOnlyList<DriftCategory> Categories =>
            Enum.GetValues<DriftCategory>().Where(x => _declared.Contains(x) || _findings.Any(f => f.Category == x)).ToList();

        public void Declare(IEnumerable<DriftCategory> checks) => _declared.UnionWith(checks);

        public void Log(DriftFinding finding)
        {
            if (finding != null)
            {
                _findings.Add(finding);
            }
        }
    }

    internal sealed class DriftContext : IDriftContext
    {
        private readonly DriftCollector _collector;

        public DriftContext(IApplication application, DriftCollector collector)
        {
            Application = application;
            _collector = collector;
        }

        public IApplication Application { get; }

        public IDriftArea Area(string name, params DriftCategory[] checks) => _collector.Area(name, checks);
    }
}
