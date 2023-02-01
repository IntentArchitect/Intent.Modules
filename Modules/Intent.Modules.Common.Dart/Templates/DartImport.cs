using System;

namespace Intent.Modules.Common.Dart.Templates
{
    /// <summary>
    /// Defines an import for a Dart file.
    /// </summary>
    public class DartImport : IEquatable<DartImport>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DartImport"/>.
        /// </summary>
        public DartImport(string type, string location)
        {
            Type = type;
            Location = location;
        }

        /// <summary>
        /// The type to import.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The location of the type to import.
        /// </summary>
        public string Location { get; }

        /// <inheritdoc />
        public bool Equals(DartImport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Location == other.Location;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DartImport)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Location);
        }

        /// <summary>
        /// Checks equality between this instance of <see cref="DartImport"/> and another.
        /// </summary>
        public static bool operator ==(DartImport left, DartImport right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Checks equality between this instance of <see cref="DartImport"/> and another.
        /// </summary>
        public static bool operator !=(DartImport left, DartImport right)
        {
            return !Equals(left, right);
        }
    }
}