using GraduationTracker.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GraduationTracker.Repositories
{
    /// <summary>
    /// Base in-memory repository that provides the common lookup behaviour for any
    /// <see cref="IEntity"/>. Concrete repositories only need to supply their seed data.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public abstract class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IReadOnlyList<T> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{T}"/> class
        /// with the entities returned by <see cref="Seed"/>.
        /// </summary>
        protected InMemoryRepository()
        {
            _items = Seed().ToList();
        }

        /// <summary>
        /// Supplies the entities the repository is initialised with.
        /// </summary>
        /// <returns>The seed entities.</returns>
        protected abstract IEnumerable<T> Seed();

        /// <inheritdoc/>
        public IEnumerable<T> GetAll() => _items;

        /// <inheritdoc/>
        public T GetById(int id) => _items.FirstOrDefault(item => item.Id == id);
    }
}
