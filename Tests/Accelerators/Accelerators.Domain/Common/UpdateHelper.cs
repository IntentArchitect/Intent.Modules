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

        /// <summary>
        /// Synchronizes a collection by adding and removing items to match the changed collection.
        /// This is useful for many-to-many relationships where entities already exist and requires
        /// that the `Add` and `Remove` Collection methods be called respectively 
        /// (as opposed to overwriting the collection directly).
        /// </summary>
        /// <typeparam name="TEntity">The type of items in the base and changed collection.</typeparam>
        /// <param name="baseCollection">The base collection to be synchronized.</param>
        /// <param name="changedCollection">The collection to synchronize to.</param>
        /// <param name="equalityCheck">A predicate that determines if an item from the base collection matches an item from the changed collection.</param>
        /// <returns>The synchronized base collection.</returns>
        /// <remarks>
        /// This method does not create new entities or update existing ones. It only adds and removes items.
        /// Typically used for many-to-many relationships where both sides already exist.
        /// If the changed collection is <see langword="null" />, the base collection will be cleared.
        /// </remarks>
        public static ICollection<TEntity> SynchronizeCollection<TEntity>(
            ICollection<TEntity> baseCollection,
            IEnumerable<TEntity>? changedCollection,
            Func<TEntity, TEntity, bool> equalityCheck)
        {
            if (changedCollection == null)
            {
                foreach (var entity in baseCollection)
                {
                    baseCollection.Remove(entity);
                }
                return baseCollection;
            }

            var result = baseCollection.CompareCollections(changedCollection, equalityCheck);

            foreach (var elementToRemove in result.ToRemove)
            {
                baseCollection.Remove(elementToRemove);
            }

            foreach (var elementToAdd in result.ToAdd)
            {
                baseCollection.Add(elementToAdd);
            }

            return baseCollection;
        }
    }
}