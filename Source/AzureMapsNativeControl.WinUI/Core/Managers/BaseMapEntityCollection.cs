using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// An abstract class used as a basis for map entity collections in most map managers.
    /// </summary>
    /// <typeparam name="T">The type of entities in this collection.</typeparam>
    /// <typeparam name="TOptions">Options type used by T</typeparam>
    public abstract class BaseMapEntityCollection<T, TOptions> : IEnumerable<T> where T : MapEntity<TOptions>
    {
        #region Internal Properties

        internal List<T> _collection = [];
        internal HashSet<string> _ids = new();
        internal Map _map;

        internal string _addMethodName;
        internal string _removeManyByIdMethodName;

        #endregion

        #region Constructor

        public BaseMapEntityCollection(Map map, string addMethodName, string removeManyByIdMethodName)
        {
            _map = map;
            _addMethodName = addMethodName;
            _removeManyByIdMethodName = removeManyByIdMethodName;
        }

        #endregion

        #region Add Methods

        /// <summary>
        /// Adds a entity object into the collection.
        /// </summary>
        /// <param name="entity">The entity object to add.</param>
        public async void Add(T entity)
        {
            await UpdateEntitiesAsync([entity]);
        }

        /// <summary>
        /// Adds a collection of entities to the collection.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public async void AddRange(IList<T> entities)
        {
            await UpdateEntitiesAsync(entities);
        }

        /// <summary>
        /// Adds a entity object into the collection.
        /// </summary>
        /// <param name="entity">The entity object to add.</param>
        public async Task AddAsync(T entity)
        {
            await UpdateEntitiesAsync([entity]);
        }

        /// <summary>
        /// Adds a collection of entities to the collection.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        public async Task AddRangeAsync(IList<T> entities)
        {
            await UpdateEntitiesAsync(entities);
        }

        #endregion

        #region Remove & Clear Methods

        /// <summary>
        /// Clears all entities from the collection.
        /// </summary>
        public async void Clear()
        {
            //Calling add entities with an empty array and set "replace" to true to clear all existing data.
            await RemoveEntitiesAsync(null, true);
        }

        /// <summary>
        /// Clears all entities from the collection.
        /// </summary>
        public async Task ClearAsync()
        {
            //Calling add entities with an empty array and set "replace" to true to clear all existing data.
            await RemoveEntitiesAsync(null, true);
        }

        /// <summary>
        /// Removes a entity from the collection.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public async void Remove(T entity)
        {
            await RemoveEntitiesAsync([entity]);
        }

        /// <summary>
        /// Removes a entity by id from the collection.
        /// </summary>
        /// <param name="entityId">The id of the entity to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public async void Remove(string? entityId)
        {
            if (!string.IsNullOrWhiteSpace(entityId) && _ids.Contains(entityId))
            {
                T? entity = _collection.FirstOrDefault(s => s.Id == entityId);

                if (entity != null)
                {
                    await RemoveEntitiesAsync([entity]);
                }
            }
        }

        /// <summary>
        /// Removes a collection of entities from the collection.
        /// </summary>
        /// <param name="entities">Collection of entities to remove.</param>
        public async void RemoveRange(IList<T> entities)
        {
            await RemoveEntitiesAsync(entities);
        }

        /// <summary>
        /// Removes a collection of entities by id's from the collection.
        /// </summary>
        /// <param name="entityIds">List of entity id's.</param>
        public async void RemoveRange(IList<string> entityIds)
        {
            if (entityIds != null)
            {
                var entities = _collection.Where(f => !string.IsNullOrWhiteSpace(f.Id) && entityIds.Contains(f.Id));
                await RemoveEntitiesAsync(entities.ToList());
            }
        }

        /// <summary>
        /// Removes a entity from the collection.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public async Task<bool> RemoveAsync(T entity)
        {
            if (entity != null)
            {
                await RemoveEntitiesAsync([entity]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a entity by id from the collection.
        /// </summary>
        /// <param name="entityId">The id of the entity to remove.</param>
        /// <returns>True if removed successfully.</returns>
        public async Task<bool> RemoveAsync(string? entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId) || !_ids.Contains(entityId))
            {
                return false;
            }

            T? entity = _collection.FirstOrDefault(s => s.Id == entityId);

            if (entity != null)
            {
                await RemoveEntitiesAsync([entity]);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a collection of entities from the collection.
        /// </summary>
        /// <param name="entities">Collection of entities to remove.</param>
        public async Task RemoveRangeAsync(IList<T> entities)
        {
            await RemoveEntitiesAsync(entities);
        }

        /// <summary>
        /// Removes a collection of entities by id's from the collection.
        /// </summary>
        /// <param name="entityIds">List of entity id's.</param>
        public async Task RemoveRangeAsync(IList<string> entityIds)
        {
            if (entityIds != null)
            {
                var entities = _collection.Where(f => !string.IsNullOrWhiteSpace(f.Id) && entityIds.Contains(f.Id));
                await RemoveEntitiesAsync(entities.ToList());
            }
        }

        #endregion

        #region Contains, Count, IndexOf Methods

        /// <summary>
        /// Specifies if a entity is contained in the collection.
        /// </summary>
        /// <param name="entities">The entity to check.</param>
        /// <returns>True is collection contains entity.</returns>
        public bool Contains(T entities)
        {
            if (entities != null)
            {
                return _collection.Contains(entities);
            }

            return false;
        }

        /// <summary>
        /// Specifies if a entity is contained in the collection.
        /// </summary>
        /// <param name="entityId">The id of the entity.</param>
        /// <returns>True is collection contains entity.</returns>
        public bool Contains(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId) || !_ids.Contains(entityId))
            {
                return false;
            }

            return _collection.Any(s => s.Id == entityId);
        }

        /// <summary>
        /// The number of entities in the collection.
        /// </summary>
        public int Count
        {
            get { return _collection.Count; }
        }

        /// <summary>
        /// Gets the index of a entity in the collection.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <returns>The index of the entity or -1 if not found.</returns>
        public int IndexOf(T entity)
        {
            if (entity != null)
            {
                return _collection.IndexOf(entity);
            }

            return -1;
        }

        /// <summary>
        /// Gets the index of a entity by id in the collection.
        /// </summary>
        /// <param name="entityId">A entity id</param>
        /// <returns>The index of a entity with the specified id or -1 if not found.</returns>
        public int IndexOf(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                return -1;
            }

            var entity = _collection.FirstOrDefault(s => s.Id == entityId);

            if (entity != null)
            {
                return _collection.IndexOf(entity);
            }

            return -1;
        }

        #endregion

        #region Replace Methods

        /// <summary>
        /// Replaces the data in the collection with an entity.
        /// </summary>
        /// <param name="entity"></param>
        public async void Replace(T entity)
        {
            await UpdateEntitiesAsync([entity], true);
        }

        /// <summary>
        /// Replaces the data in the collection with a collection of entities.
        /// </summary>
        /// <param name="entities"></param>
        public async void ReplaceRange(IList<T> entities)
        {
            await UpdateEntitiesAsync(entities, true);
        }

        /// <summary>
        /// Replaces the data in the collection with an entity.
        /// </summary>
        /// <param name="entity"></param>
        public async Task ReplaceAsync(T entity)
        {
            await UpdateEntitiesAsync([entity], true);
        }

        /// <summary>
        /// Replaces the data in the collection with a collection of entities.
        /// </summary>
        /// <param name="entities"></param>
        public async Task ReplaceRangeAsync(IList<T> entities)
        {
            await UpdateEntitiesAsync(entities, true);
        }

        #endregion

        #region Enumerator Methods

        /// <summary>
        /// Gets an enumerator for the entities in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the entities in the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the entities in the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets entity at a specific index.
        /// </summary>
        /// <param name="index">The index of the entity.</param>
        /// <returns>The entity at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                return _collection[index];
            }
        }

        /// <summary>
        /// Gets a entity by it's ID.
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <returns>The matched entity or null.</returns>
        public T? GetById(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default(T);
            }

            return _collection.FirstOrDefault(s => s.Id == id);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Internal method used to clear the collection without doing anything to the map.
        /// </summary>
        internal void SilentClear()
        {
            _collection.Clear();
        }

        /// <summary>
        /// Adds/removes entities to the collection.
        /// </summary>
        /// <param name="newEntities">The new entities to add.</param>
        internal virtual async Task UpdateEntitiesAsync(IList<T>? newEntities, bool replaceAll = false)
        {
            if (replaceAll)
            {
                await RemoveEntitiesAsync(null, replaceAll);
            }

            //Add new entities.
            if (newEntities != null && newEntities.Count > 0)
            {
#if MAUI && ANDROID
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
#endif
                    foreach (var e in newEntities)
                    {
                        //Make sure the entity is not null.
                        if (e != null)
                        {
                            //Ensure the entity is not already in this collection.
                            if (e.Map == null || e.Map != _map)
                            {
                                e.Map = null;

                                //Add the collection.
                                _collection.Add(e);

                                //Add to the map.
                                await _map.JsInterlop.InvokeJsMethodAsync(_map, _addMethodName, e.Id, e.GetOptions());

                                //Set the map property.
                                e.Map = _map;

                                if (e is IMapEventTarget el)
                                {
                                    _map.Events.ReAddEvents(el);
                                }
                            }
                        }
                    }
#if MAUI && ANDROID
                });
#endif
            }
        }

        internal async Task RemoveEntitiesAsync(IList<T>? oldEntities, bool replaceAll = false)
        {
            var removeIds = new List<string>();

            if (replaceAll)
            {
                //Dettach all entities from the map.
                foreach (var e in _collection)
                {
                    if (e != null)
                    {
                        e.Map = null;
                        removeIds.Add(e.Id);
                    }
                }

                //Clear the collection.
                _collection.Clear();
            }

            //Remove old entities.
            else if (oldEntities != null && oldEntities.Count > 0)
            {
                foreach (var e in oldEntities)
                {
                    if (e != null && Contains(e))
                    {
                        //Remove from the map.
                        if (e.Map != null)
                        {
                            e.Map = null;
                            removeIds.Add(e.Id);
                        }

                        //Remove from the collection.
                        _collection.Remove(e);
                    }
                }
            }

            if (removeIds.Count > 0)
            {

#if MAUI && ANDROID
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
#endif
                    //Remove any events attached to these entities.
                    await _map.Events.BulkRemoveEvents(removeIds);

                    //Remove entities by id.
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, _removeManyByIdMethodName, removeIds);
#if MAUI && ANDROID
                });
#endif
            }
        }

    #endregion
}
}
