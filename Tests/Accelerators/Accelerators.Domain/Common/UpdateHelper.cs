using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.UpdateHelper", Version = "1.0")]

namespace Accelerators.Domain.Common
{
    /// <summary>
    /// Provides utility methods for updating collections.
    /// </summary>
    public static class UpdateHelper
    {
        /// <summary>
        /// Performs mutations to synchronize the baseCollection to end up the same as the changedCollection.
        /// </summary>
        /// <typeparam name="TChanged">The type of items in the changed collection.</typeparam>
        /// <typeparam name="TOriginal">The type of items in the base collection.</typeparam>
        /// <param name="baseCollection">The base collection to be updated.</param>
        /// <param name="changedCollection">The collection containing the changes.</param>
        /// <param name="equalityCheck">A predicate that determines if an item from the base collection matches an item from the changed collection.</param>
        /// <param name="assignmentAction">A delegate that defines how to update an item from the base collection using an item from the changed collection.</param>
        /// <returns>The updated base collection.</returns>
        /// <remarks>
        /// If the changed collection is <see langword="null" />, an empty list of type <typeparamref name="TOriginal"/> will be returned.
        /// If the base collection is <see langword="null" />, a new list of type <typeparamref name="TOriginal"/> will be created and used.
        /// </remarks>
        public static ICollection<TOriginal> CreateOrUpdateCollection<TChanged, TOriginal>(
            ICollection<TOriginal> baseCollection,
            IEnumerable<TChanged>? changedCollection,
            Func<TOriginal, TChanged, bool> equalityCheck,
            Func<TOriginal?, TChanged, TOriginal> assignmentAction)
        {
            if (changedCollection == null)
            {
                return new List<TOriginal>();
            }

            baseCollection ??= new List<TOriginal>()!;

            var result = baseCollection.CompareCollections(changedCollection, equalityCheck);
            foreach (var elementToAdd in result.ToAdd)
            {
                var newEntity = assignmentAction(default, elementToAdd);
                baseCollection.Add(newEntity);
            }

            foreach (var elementToRemove in result.ToRemove)
            {
                baseCollection.Remove(elementToRemove);
            }

            foreach (var elementToEdit in result.PossibleEdits)
            {
                assignmentAction(elementToEdit.Original, elementToEdit.Changed);
            }

            return baseCollection;
        }
    }
}