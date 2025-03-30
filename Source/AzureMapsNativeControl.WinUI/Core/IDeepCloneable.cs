namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// Interface for deep cloning objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepCloneable<T>
    {
        /// <summary>
        /// A generic clone method that creates a deep clone of the object.
        /// </summary>
        /// <returns></returns>
        public T DeepClone();
    }
}
