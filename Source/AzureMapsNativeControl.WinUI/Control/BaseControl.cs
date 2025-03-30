using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Base constructor for all controls.
    /// </summary>
    [JsonDerivedType(typeof(BringDataIntoViewControl))]
    [JsonDerivedType(typeof(CompassControl))]
    [JsonDerivedType(typeof(OverviewMapControl))]
    [JsonDerivedType(typeof(PitchControl))]
    [JsonDerivedType(typeof(ScaleControl))]
    [JsonDerivedType(typeof(StyleControl))]
    [JsonDerivedType(typeof(TrafficControl))]
    [JsonDerivedType(typeof(TrafficLegendControl))]
    [JsonDerivedType(typeof(ZoomControl))]
    public abstract class BaseControl: IBaseControl, INotifyPropertyChanged
    {
        #region Private Properties

        private ControlPosition _position = ControlPosition.TopLeft;
        private ControlStyle _style = ControlStyle.Light;

        #endregion

        #region Constructor

        /// <summary>
        /// Base constructor for all controls.
        /// </summary>
        /// <param name="jsNamespace">The JavaScript namespace to the control class.</param>
        public BaseControl(string jsNamespace)
        {
            JsNamespace = jsNamespace;
            Id = UniqueId.Get(jsNamespace); 
        }

        /// <summary>
        /// Base constructor for all controls.
        /// </summary>
        /// <param name="jsNamespace">The JavaScript namespace to the control class.</param>
        /// <param name="moduleInfo">Module information for the control. Set this for custom controls before adding the control to the maps control manager.</param>
        public BaseControl(string jsNamespace, MapModuleInfo moduleInfo)
        {
            JsNamespace = jsNamespace;
            ModuleInfo = moduleInfo;
            Id = UniqueId.Get(jsNamespace);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Event for when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged = null;

        /// <summary>
        /// Optional module information for the control. Set this for custom controls before adding the control to the maps control manager.
        /// </summary>
        [JsonIgnore]
        public MapModuleInfo? ModuleInfo { get; private set; }

        /// <summary>
        /// The JavaScript namespace of the class.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("jsNamespace")]
        internal string JsNamespace { get; private set; }

        /// <summary>
        /// A unique ID for the source.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The map instance the control is attached to.
        /// </summary>
        [JsonIgnore]
        public Map? _map { get; set; }

        /// <summary>
        /// The position the control will be placed on the map. If not specified, the control will be located at the default position it defines.
        /// </summary>
        [JsonPropertyName("position")]
        public ControlPosition Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged("Position", "position", value);
                }
            }
        }

        /// <summary>
        /// The style of the control.
        /// Can only set before adding the control to the map, unless the control has a setOptions method that supports updating this.
        /// </summary>
        [JsonPropertyName("style")]
        public ControlStyle Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    OnPropertyChanged("Style", "style", value);
                }
            }
        }

        #endregion

        #region Internal Methods

        internal void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void OnPropertyChanged(string propertyName, object? value)
        {
            string jsPropName = Utils.ToCamelCase(propertyName);
            OnPropertyChanged(propertyName, jsPropName, value);
        }

        internal async void OnPropertyChanged(string propertyName, string jsPropName, object? value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (_map != null)
            {
                //If value is null, set all options.
                if (value == null)
                {
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "setControlOptions", this);
                }
                else
                {
                    //Only update the individual property
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "setControlOptions", new Dictionary<string, object?>()
                    {
                        { "id", Id },
                        { jsPropName, value }
                    });
                }
            }
        }

        internal async void CallCustomControlFunction(string functionName, params object?[] args)
        {
            if (_map != null)
            {
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "callGenericItemFunction", Id, Constants.ControlCache, functionName, args);
            }
        }

        internal async Task<T?> CallCustomControlFunction<T>(string functionName, params object?[] args)
        {
            if (_map != null)
            {
                return await _map.JsInterlop.InvokeJsMethodAsync<T>(_map, "callGenericItemFunction", Id, Constants.ControlCache, functionName, args);
            }

            return default(T);
        }

        #endregion
    }
}
