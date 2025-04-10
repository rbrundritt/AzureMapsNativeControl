using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// A control that uses the browser's geolocation API to locate the user on the map.
    /// </summary>
    public sealed class GeolocationControl : BaseControl
    {
        #region Private Properties

        private bool _calculateMissingValues = false;
        private string _markerColor = "DodgerBlue";
        private int _maxZoom = 15;

        private GeolocationPositionOptions _positionOptions = new GeolocationPositionOptions
        {
            Timeout = 10000,
            EnableHighAccuracy = true,
            MaximumAge = double.PositiveInfinity
        };
        
        private bool _showUserLocation = true;
        private bool _trackUserLocation = false;
        private bool _updateMapCamera = true;

        private int _compassEventThrottleDelay = 100;
        private bool _enableCompass = true;
        private bool _syncMapCompassHeading = false;

        #endregion

        #region Contructor

        /// <summary>
        /// A control that uses the browser's geolocation API to locate the user on the map.
        /// </summary>
        public GeolocationControl() : base("atlas.control.GeolocationControl", AzureMapsModules.GeolocationControlModule) {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Specifies that if the `speed` or `heading` values are missing in the geolocation position, it will calculate these values 
        /// based on the last known position. Default: `false`
        /// </summary>
        [JsonPropertyName("calculateMissingValues")]
        public bool CalculateMissingValues
        {
            get
            {
                return _calculateMissingValues;
            }
            set
            {
                _calculateMissingValues = value;
                OnPropertyChanged("CalculateMissingValues", value);
            }
        }

        /// <summary>
        /// The delay in milliseconds between compass events. The compass heading value can change very rapidly with the slightest movement of a device which can negatively 
        /// impact applications where heavy computations or UI changes occur due to the event. This options throttles how frequently the event will fire.Only values greater or equal to `100` are accepted.
        /// The marker direction updates independantly of this option.Default: `100`
        /// </summary>
        [JsonPropertyName("compassEventThrottleDelay")]
        public int CompassEventThrottleDelay
        {
            get
            {
                return _compassEventThrottleDelay;
            }
            set
            {
                _compassEventThrottleDelay = value;
                OnPropertyChanged("CompassEventThrottleDelay", value);
            }
        }

        /// <summary>
        /// Specifies if the compass should be enabled, if available. Based on the device orientation. Default: `true`
        /// </summary>
        [JsonPropertyName("enableCompass")]
        public bool EnableCompass
        {
            get
            {
                return _enableCompass;
            }
            set
            {
                _enableCompass = value;
                OnPropertyChanged("EnableCompass", value);
            }
        }

        /// <summary>
        /// Specifies if the map should rotate to sync it's heading with the compass. Based on the device orientation. Default: `false`
        /// </summary>
        [JsonPropertyName("syncMapCompassHeading")]
        public bool SyncMapCompassHeading
        {
            get
            {
                return _syncMapCompassHeading;
            }
            set
            {
                _syncMapCompassHeading = value;
                OnPropertyChanged("SyncMapCompassHeading", value);
            }
        }

        /// <summary>
        /// Specifies that if the `speed` or `heading` values are missing in the geolocation position, it will calculate these values 
        /// based on the last known position. Default: `false`
        /// </summary>
        [JsonPropertyName("markerColor")]
        public string MarkerColor
        {
            get
            {
                return _markerColor;
            }
            set
            {
                _markerColor = value;
                OnPropertyChanged("MarkerColor", value);
            }
        }

        /// <summary>
        /// The maximum zoom level the map can be zoomed out. 
        /// If zoomed out more than this when location updates, the map will zoom into this level.
        /// If zoomed in more than this level, the map will maintain its current zoom level.
        /// Default: `15`
        /// </summary>
        [JsonPropertyName("maxZoom")]
        public int MaxZoom
        {
            get
            {
                return _maxZoom;
            }
            set
            {
                _maxZoom = value;
                OnPropertyChanged("CalculateMissingValues", value);
            }
        }

        /// <summary>
        /// A Geolocation API PositionOptions object. Default: `{ enableHighAccuracy : true , maximumAge: Infinity, timeout : 10000 }`
        /// </summary>
        [JsonPropertyName("positionOptions")]
        public GeolocationPositionOptions PositionOptions
        {
            get
            {
                return _positionOptions;
            }
            set
            {
                _positionOptions = value;
                OnPropertyChanged("PositionOptions", value);
            }
        }

        /// <summary>
        /// Shows the users location on the map using a marker. Default: `true` 
        /// </summary>
        [JsonPropertyName("showUserLocation")]
        public bool ShowUserLocation
        {
            get
            {
                return _showUserLocation;
            }
            set
            {
                _showUserLocation = value;
                OnPropertyChanged("ShowUserLocation", value);
            }
        }

        /// <summary>
        /// If `true` the geolocation control becomes a toggle button and when active the map will receive updates to the user's location as it changes. Default: `false`
        /// </summary>
        [JsonPropertyName("trackUserLocation")]
        public bool TrackUserLocation
        {
            get
            {
                return _trackUserLocation;
            }
            set
            {
                _trackUserLocation = value;
                OnPropertyChanged("TrackUserLocation", value);
            }
        }

        /// <summary>
        /// Specifies if the map camera should update as the position moves. When set to `true`, the map camera will update to the new position, unless the 
        /// user has interacted with the map. Default: `true`
        /// </summary>
        [JsonPropertyName("updateMapCamera")]
        public bool UpdateMapCamera
        {
            get
            {
                return _updateMapCamera;
            }
            set
            {
                _updateMapCamera = value;
                OnPropertyChanged("UpdateMapCamera", value);
            }
        }

        #endregion
    }
}
