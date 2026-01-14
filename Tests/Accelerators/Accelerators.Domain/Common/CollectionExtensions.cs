using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.CollectionExtensions", Version = "1.0")]

namespace Accelerators.Domain.Common
{
    /// <summary>
    /// Provides extension methods for collection objects.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Compares two collections and returns a result indicating the differences between them.
        /// </summary>
        /// <typeparam name="TChanged">The type of elements in the changed collection.</typeparam>
        /// <typeparam name="TOriginal">The type of elements in the base collection.</typeparam>
        /// <param name="baseCollection">The base collection to compare.</param>
        /// <param name="changedCollection">The changed collection to compare against the base collection.</param>
        /// <param name="equalityCheck">A predicate to determine if two elements are equal.</param>
        /// <returns>A <see cref="ComparisonResult{TChanged, TOriginal}"/> object that describes the differences between the two collections.</returns>
        public static ComparisonResult<TChanged, TOriginal> CompareCollections<TChanged, TOriginal>(
            this ICollection<TOriginal> baseCollection,
            IEnumerable<TChanged> changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck)
        {
            changedCollection ??= new List<TChanged>();

            var toRemove = baseCollection.Where(baseElement => changedCollection.All(changedElement => !equalityCheck(baseElement, changedElement))).ToList();
            var toAdd = changedCollection.Where(changedElement => baseCollection.All(baseElement => !equalityCheck(baseElement, changedElement))).ToList();

            var possibleEdits = new List<Match<TChanged, TOriginal>>();

            foreach (var changedElement in changedCollection)
            {
                var match = baseCollection.FirstOrDefault(baseElement => equalityCheck(baseElement, changedElement));

                if (match is not null)
                {
                    possibleEdits.Add(new Match<TChanged, TOriginal>(changedElement, match));
                }
            }
            return new ComparisonResult<TChanged, TOriginal>(toAdd, toRemove, possibleEdits);
        }

        /// <summary>
        /// Represents the result of comparing two collections.
        /// </summary>
        /// <typeparam name="TChanged">The type of elements that have changed.</typeparam>
        /// <typeparam name="TOriginal">The type of original elements.</typeparam>
        public class ComparisonResult<TChanged, TOriginal>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ComparisonResult{TChanged, TOriginal}"/> class.
            /// </summary>
            /// <param name="toAdd">A collection of elements to be added.</param>
            /// <param name="toRemove">A collection of elements to be removed.</param>
            /// <param name="possibleEdits">A collection of matched elements that might have edits.</param>
            public ComparisonResult(ICollection<TChanged> toAdd,
                ICollection<TOriginal> toRemove,
                ICollection<Match<TChanged, TOriginal>> possibleEdits)
            {
                ToAdd = toAdd;
                ToRemove = toRemove;
                PossibleEdits = possibleEdits;
            }

            /// <summary>
            /// Gets the collection of elements to be added.
            /// </summary>
            public ICollection<TChanged> ToAdd { get; }
            /// <summary>
            /// Gets the collection of elements to be removed.
            /// </summary>
            public ICollection<TOriginal> ToRemove { get; }
            /// <summary>
            /// Gets the collection of matched elements that might have edits.
            /// </summary>
            public ICollection<Match<TChanged, TOriginal>> PossibleEdits { get; }

            /// <summary>
            /// Determines whether there are any changes between the two collections.
            /// </summary>
            /// <returns><see langword="true" /> if there are changes; otherwise, <see langword="false" />.</returns>
            public bool HasChanges()
            {
                return ToAdd.Count > 0 || ToRemove.Count > 0 || PossibleEdits.Count > 0;
            }
        }
        /// <summary>
        /// Represents a matched pair of changed and original elements.
        /// </summary>
        /// <typeparam name="TChanged">The type of the changed element.</typeparam>
        /// <typeparam name="TOriginal">The type of the original element.</typeparam>
        public class Match<TChanged, TOriginal>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Match{TChanged, TOriginal}"/> class.
            /// </summary>
            /// <param name="changed">The changed element.</param>
            /// <param name="original">The original element.</param>
            public Match(TChanged changed, TOriginal original)
            {
                Changed = changed;
                Original = original;
            }

            /// <summary>
            /// Gets the changed element.
            /// </summary>
            public TChanged Changed { get; private set; }
            /// <summary>
            /// Gets the original element.
            /// </summary>
            public TOriginal Original { get; private set; }
        }
    }
}