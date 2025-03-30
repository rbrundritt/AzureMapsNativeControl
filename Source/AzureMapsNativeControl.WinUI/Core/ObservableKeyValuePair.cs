using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// An observable key value pair.
    /// </summary>
    /// <typeparam name="TKey">A Unique key value.</typeparam>
    /// <typeparam name="TValue?"></typeparam>
    [Serializable]
    public class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged, IDeepCloneable<ObservableKeyValuePair<TKey, TValue>> where TKey : notnull
    {
        #region Private Properties

        private TKey? key;
        private TValue? value;

        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Constructors

        public ObservableKeyValuePair()
        {
        }

        public ObservableKeyValuePair(TKey? key, TValue? value)
        {
            this.key = key;
            this.value = value;
        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// The key of the key value pair.
        /// </summary>
        public TKey? Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
            }
        }

        /// <summary>
        /// The value of the key value pair.
        /// </summary>
        public TValue? Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of the key value pair.
        /// </summary>
        /// <returns></returns>
        public ObservableKeyValuePair<TKey, TValue> DeepClone()
        {
            TValue? value;

            if (Value is IDeepCloneable<TValue> deepCloneable)
            {
                value = deepCloneable.DeepClone();
            }
            else if (Value is ICloneable cloneable)
            {
                value = (TValue)cloneable.Clone();
            }
            else
            {
                value = Value;
            }

            return new ObservableKeyValuePair<TKey, TValue>()
            {
                Key = Key,
                Value = value
            };
        }

        #endregion

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
