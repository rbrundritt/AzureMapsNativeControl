using AzureMapsNativeControl.Internal;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Event args for key events.
    /// </summary>
    public class MapKeyboardEventArgs : MapEventArgs
    {
        #region Constructor

        /// <summary>
        /// Event object returned by the maps when a key event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventName">The name of the event that occurred.</param>
        public MapKeyboardEventArgs(Map map, string eventName) : base(map, eventName)
        {
        }

        /// <summary>
        /// Event object returned by the maps when a key event occurs.
        /// </summary>
        /// <param name="map">The Map instance in which the event occurred on.</param>
        /// <param name="eventData">Raw event data to populate this object.</param>
        internal MapKeyboardEventArgs(Map map, RawMapMsg eventData) : base(map, eventData)
        {
            Key = eventData.Key;
            KeyCode = eventData.KeyCode;
            AltKey = eventData.AltKey;
            CtrlKey = eventData.CtrlKey;
            ShiftKey = eventData.ShiftKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The key that was pressed. For example: "a", "A", "Enter"
        /// Set to the JavaScript KeyboardEvent.key value.
        /// https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// The code for the key that was pressed. For example: "KeyA", "Enter"
        /// Set to the JavaScript KeyboardEvent.code value.
        /// https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/code
        /// </summary>
        public string? KeyCode { get; set; }

        /// <summary>
        /// Indicates if the alt key was pressed when the event occurred.
        /// </summary>
        public bool AltKey { get; set; } = false;

        /// <summary>
        /// Indicates if the ctrl key was pressed when the event occurred.
        /// </summary>
        public bool CtrlKey { get; set; } = false;

        /// <summary>
        /// Indicates if the shift key was pressed when the event occurred.
        /// </summary>
        public bool ShiftKey { get; set; } = false;

        #endregion
    }
}
