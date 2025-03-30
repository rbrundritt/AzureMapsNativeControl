using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// An observable dictionary.
    /// </summary>
    /// <typeparam name="TKey">A non-Nullable key value.</typeparam>
    /// <typeparam name="TValue?"></typeparam>
    [Serializable]
    public class ObservableDictionary<TKey, TValue> : ObservableRangeCollection<ObservableKeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue?>, IDeepCloneable<ObservableDictionary<TKey, TValue>> where TKey : notnull
    {
        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue?>>? collection = null)
           : base()
        {
            if (collection != null)
            {
                var list = collection.Select(pair => new ObservableKeyValuePair<TKey, TValue?>(pair.Key, pair.Value)).ToList();
                AddRangeCore(list);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public ObservableDictionary(IEnumerable<ObservableKeyValuePair<TKey, TValue?>> collection)
           : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey, TValue?}"/> class.
        /// </summary>
        /// <param name="collection"></param>
        public ObservableDictionary(ObservableDictionary<TKey, TValue?> collection)
           : base(collection)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Specifies if the dictionary is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the keys of the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return (from i in ThisAsCollection() select i.Key).ToList(); }
        }

        /// <summary>
        /// Gets the values of the dictionary.
        /// </summary>
        public ICollection<TValue?> Values
        {
            get { return (from i in ThisAsCollection() select i.Value).ToList(); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a key value pair to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(TKey key, TValue? value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("The dictionary already contains the key: " + key);
            }
            base.Add(new ObservableKeyValuePair<TKey, TValue>() { Key = key, Value = value });
        }

        /// <summary>
        /// Adds a key value pair to the dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(KeyValuePair<TKey, TValue?> item)
        {
            if (ContainsKey(item.Key))
            {
                throw new ArgumentException("The dictionary already contains the key: " + item.Key);
            }
            base.Add(new ObservableKeyValuePair<TKey, TValue>() { Key = item.Key, Value = item.Value });
        }

        /// <summary>
        /// Adds a range of key value pairs to the dictionary.
        /// </summary>
        /// <param name="items"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue?>> items)
        {
            var list = new List<ObservableKeyValuePair<TKey, TValue?>>();
            foreach (var pair in items)
            {
                if (ContainsKey(pair.Key))
                {
                    throw new ArgumentException("The dictionary already contains the key: " + pair.Key);
                }
                list.Add(new ObservableKeyValuePair<TKey, TValue?>(pair.Key, pair.Value));
            }
            base.AddRange(list);
        }

        /// <summary>
        /// Tries to create a deep clone of the dictionary.
        /// </summary>
        /// <returns></returns>
        public new ObservableDictionary<TKey, TValue> DeepClone()
        {
            var list = new List<ObservableKeyValuePair<TKey, TValue?>>();

            foreach (var i in ThisAsCollection())
            {
                list.Add(i.DeepClone());
            }

            return new ObservableDictionary<TKey, TValue>(list);
        }

        /// <summary>
        /// Checks to see if the dictionary contains a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return ThisAsCollection().Any((i) => i.Key.Equals(key));
        }

        /// <summary>
        /// Checks to see if the dictionary contains a value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue?> item)
        {
            var r = GetKvpByTheKey(item.Key);
            if (Equals(r, default(ObservableKeyValuePair<TKey, TValue?>)))
            {
                return false;
            }
            return Equals(r.Value, item.Value);
        }

        /// <summary>
        /// Copies 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        public new int Count => ThisAsCollection().Count;

        /// <summary>
        /// Copies the key value pairs to an array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyTo(KeyValuePair<TKey, TValue?>[] array, int arrayIndex)
        {
            var count = this.Count;

            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (count > array.Length - arrayIndex)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var i in ThisAsCollection())
            {
                if (i.Key == null)
                    throw new InvalidOperationException("Key or Value cannot be null.");

                array[arrayIndex++] = new KeyValuePair<TKey, TValue?>(i.Key, i.Value);
            }
        }

        /// <summary>
        /// Gets an enumerator of key value pairs.
        /// </summary>
        /// <returns></returns>
        public new IEnumerator<KeyValuePair<TKey, TValue?>> GetEnumerator()
        {
            return (from i in ThisAsCollection() select new KeyValuePair<TKey, TValue?>(i.Key, i.Value)).ToList().GetEnumerator();
        }

        /// <summary>
        /// Removes an item from the dictionary that has the matching key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            var remove = ThisAsCollection().Where(pair => Equals(key, pair.Key)).ToList();
            foreach (var pair in remove)
            {
                ThisAsCollection().Remove(pair);
            }
            return remove.Count > 0;
        }

        /// <summary>
        /// Removes a key value pair from the dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue?> item)
        {
            var r = GetKvpByTheKey(item.Key);
            if (Equals(r, default(ObservableKeyValuePair<TKey, TValue?>)))
            {
                return false;
            }
            if (!Equals(r.Value, item.Value))
            {
                return false;
            }
            return ThisAsCollection().Remove(r);
        }

        /// <summary>
        /// Remove a range of key value pairs. 
        /// </summary>
        /// <param name="items"></param>
        public void RemoveRange(IEnumerable<KeyValuePair<TKey, TValue?>> items)
        {
            var list = new List<ObservableKeyValuePair<TKey, TValue?>>();

            foreach (var pair in items)
            {
                if (pair.Key != null && ContainsKey(pair.Key))
                {
                    var r = GetKvpByTheKey(pair.Key);
                    if (!Equals(r, default(ObservableKeyValuePair<TKey, TValue?>)) && Equals(r.Value, pair.Value))
                    {
                        list.Add(r);
                    }
                }
            }

            base.RemoveRange(list);
        }

        /// <summary>
        /// Removes a range of keys.
        /// </summary>
        /// <param name="keys"></param>
        public void RemoveRange(IEnumerable<TKey> keys)
        {
            var list = new List<ObservableKeyValuePair<TKey, TValue?>>();

            foreach (var key in keys)
            {
                if (ContainsKey(key))
                {
                    var r = GetKvpByTheKey(key);
                    if (!Equals(r, default(ObservableKeyValuePair<TKey, TValue?>)))
                    {
                        list.Add(r);
                    }
                }
            }

            base.RemoveRange(list);
        }

        /// <summary>
        /// Replaces a key value pair with a new key value pair.
        /// </summary>
        /// <param name="items"></param>
        public void ReplaceRange(IEnumerable<KeyValuePair<TKey, TValue?>> items)
        {
            Clear();
            AddRange(items);
        }

        /// <summary>
        /// Tries to get the value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue? value)
        {
            foreach (var i in ThisAsCollection())
            {
                if (i.Key != null && i.Key.Equals(key))
                {
                    value = i.Value;
                    return true;
                }
            }
            value = default(TValue?);
            return false;
        }

        /// <summary>
        /// Tries to get the value of a key and cast it to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue<T>(TKey key, out T? value)
        {
            value = default;
            var r = GetKvpByTheKey(key);
            if (r == null || Equals(r, default(ObservableKeyValuePair<TKey, TValue?>)) || r.Value == null)
            {
                return false;
            }
            if (r.Value is T casTValue)
            {
                value = casTValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the value of a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TValue? this[TKey key]
        {
            get
            {
                TValue? result;
                if (!TryGetValue(key, out result))
                {
                    throw new ArgumentException("Key not found");
                }
                return result;
            }
            set
            {
                if (ContainsKey(key))
                {
                    GetKvpByTheKey(key).Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        #endregion

        #region Private Methods

        internal ObservableKeyValuePair<TKey, TValue?> GetKvpByTheKey(TKey key)
        {
            return ThisAsCollection().FirstOrDefault(i => i.Key.Equals(key));
        }

        private bool Equals<T>(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }

        private ObservableCollection<ObservableKeyValuePair<TKey, TValue?>> ThisAsCollection()
        {
            return this;
        }

        #endregion
    }
}