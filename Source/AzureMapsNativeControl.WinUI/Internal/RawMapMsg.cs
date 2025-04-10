using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Drawing;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Internal
{
    /// <summary>
    /// Internal class for passing data between .NET and JS versions of the map.
    /// </summary>
    internal class RawMapMsg
    {
        /// <summary>
        /// The type name of the event.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the map that the event occured on.
        /// </summary>
        [JsonPropertyName("mapId")]
        public string MapId { get; set; } = string.Empty;

        /// <summary>
        /// The Error object that triggered the event.
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        /// <summary>
        /// Error information from the Geolocation API.
        /// </summary>
        [JsonPropertyName("geolocationError")]
        public GeolocationPositionError? GeolocationError { get; set; }

        /// <summary>
        /// The compass heading. Set when the compass heading changes or when there is a last known compass heading when there is a geolocation success.
        /// </summary>
        [JsonPropertyName("compassHeading")]
        public double? CompassHeading { get; set; }

        /// <summary>
        /// The map camera options when the event occurred.
        /// </summary>
        [JsonPropertyName("camera")]
        public CameraOptions? Camera { get; set; }

        #region Data Properties

        /// <summary>
        /// The id's of shapes that were in the event. These are features that were added to a DataSource. 
        /// This is a simple lookup table that we use in combination with the Features property to determine which shapes we should try and retrieve the .NET versions of.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("shapeIds")]
        internal IList<string>? ShapeIds { get; set; } 

        /// <summary>
        /// Raw GeoJSON features in the event.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("features")]
        [JsonConverter(typeof(FeatureEnumerableConverter))]
        internal IList<Feature>? Features { get; set; }

        #endregion

        #region Keyboard Event Properties

        /// <summary>
        /// The key that was pressed. For example: "a", "A", "Enter"
        /// Set to the JavaScript KeyboardEvent.key value.
        /// https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// The code for the key that was pressed. For example: "KeyA", "Enter"
        /// Set to the JavaScript KeyboardEvent.code value.
        /// https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/code
        /// </summary>
        [JsonPropertyName("keyCode")]
        public string? KeyCode { get; set; }

        /// <summary>
        /// Indicates if the alt key was pressed when the event occurred.
        /// </summary>
        [JsonPropertyName("altKey")]
        public bool AltKey { get; set; } = false;

        /// <summary>
        /// Indicates if the ctrl key was pressed when the event occurred.
        /// </summary>
        [JsonPropertyName("ctrlKey")]
        public bool CtrlKey { get; set; } = false;

        /// <summary>
        /// Indicates if the shift key was pressed when the event occurred.
        /// </summary>
        [JsonPropertyName("shiftKey")]
        public bool ShiftKey { get; set; } = false;

        #endregion

        #region Touch Event Properties

        /// <summary>
        /// The array of pixel coordinates of all touch points on the map.
        /// </summary>
        [JsonPropertyName("pixels")]
        public IList<Pixel>? Pixels { get; set; }

        /// <summary>
        /// The geographical location of all touch points on the map.
        /// </summary>
        [JsonPropertyName("positions")]
        [JsonConverter(typeof(PositionEnumerableConverter))]
        public IList<Position>? Positions { get; set; }

        #endregion

        #region Mouse Events Properties

        /// <summary>
        /// The id of the layer the event is attached to.
        /// </summary>
        [JsonPropertyName("layerId")]
        public string? LayerId { get; set; }

        /// <summary>
        /// The pixel coordinates of the mouse pointer, relative to the map.
        /// </summary>
        [JsonPropertyName("pixel")]
        public Pixel? Pixel { get; set; }

        /// <summary>
        /// The geographic location on the map of the mouse pointer.
        /// </summary>
        [JsonPropertyName("position")]
        public Position? Position { get; set; }

        #endregion

        #region TargetedEvent Properties

        /// <summary>
        /// The id of the marker thar was targetted.
        /// </summary>
        [JsonPropertyName("markerId")]
        public string? MarkerId { get; set; }

        /// <summary>
        /// The id of the popup thar was targetted.
        /// </summary>
        [JsonPropertyName("popupId")]
        public string? PopupId { get; set; }

        #endregion

        #region Style Change Properties

        /// <summary>
        /// The name of the style that was loaded.
        /// </summary>
        [JsonPropertyName("style")]
        public MapStyle? Style { get; set; }

        /// <summary>
        /// The result of the async operation.
        /// </summary>
        [JsonPropertyName("_result")]
        public JsonElement? Result { get; set; }

        #endregion

        #region Drawing Manager Properties

        /// <summary>
        /// The drawing mode the manager is in.
        /// </summary>
        [JsonPropertyName("drawingMode")]
        public DrawingMode? DrawingMode { get; set; }

        /// <summary>
        /// The id of the drawing manager that the event occurred on.
        /// </summary>
        [JsonPropertyName("drawingManagerId")]
        public string? DrawingManagerId { get; set; }

        #endregion

        #region Animation Properties

        /// <summary>
        /// The id of the animation that the event occurred on.
        /// </summary>
        [JsonPropertyName("animationId")]
        public string? AnimationId { get; set; }
        
        /// <summary>
        /// Progress of the animation where 0 is the start and 1 is the end.
        /// </summary>
        [JsonPropertyName("progress")]
        public double Progress { get; set; }

        /// <summary>
        /// The progress of the animation after being passed through an easing function.
        /// </summary>
        [JsonPropertyName("easingProgress")]
        public double EasingProgress { get; set; }

        /// <summary>
        /// The focal heading of an animation frame. Returned by path animations.
        /// </summary>
        [JsonPropertyName("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// The index of the frame if using the frame based animation timer.
        /// </summary>
        [JsonPropertyName("frameIdx")]
        public int FrameIdx { get; set; }

        /// <summary>
        /// The number of frames in the animation. 
        /// </summary>
        [JsonPropertyName("numFrames")]
        public int NumFrames { get; set; }

        /// <summary>
        ///  Average speed between points in meters per second.
        /// </summary>
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        /// <summary>
        /// Estimated JSON timestamp in the animation based on the timestamp information provided for each point. 
        /// </summary>
        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }

        #endregion

        /// <summary>
        /// The id of the control that the event occurred on.
        /// </summary>
        [JsonPropertyName("controlId")]
        public string? ControlId { get; set; }

        /// <summary>
        /// The array of dropped file information.
        /// </summary>
        [JsonPropertyName("files")]
        public IList<RawMapDroppedFileInfo>? Files { get; set; }

        /// <summary>
        /// The id of an async task.
        /// </summary>
        [JsonPropertyName("taskId")]
        public string? TaskId { get; set; }

        #region Public Methods
        
        /// <summary>
        /// Gets the Features from the map using the shape IDs.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public IList<Feature> GetFeatures(Map map)
        {
            //If there are shape ids, update the features to the .NET versions.
            if (ShapeIds != null && Features != null)
            {
                //Loop through features, if their Id is in the list of shape ids, get their .NET versions.
                for(int i = 0; i < Features.Count; i++)
                {
                    var feature = Features[i];

                    if (!string.IsNullOrWhiteSpace(feature.Id) && ShapeIds.Contains(feature.Id))
                    {
                        var f = map.Sources.GetDataSourceFeature(feature);

                        if (f != null)
                        {
                            //Update the feature reference.
                            //Note that the feature in the message may have new coordinates and properties.
                            var orig = Features[i];

                            if(!orig.Geometry.Equals(f.Geometry))
                            {
                               orig.Geometry = f.Geometry;
                            }

                            //Update the properties if they are different, but only if there are new properties, don't remove anything.
                            if(orig.Properties != null)
                            {
                                if (f.Properties != null)
                                {
                                    foreach (var key in f.Properties.Keys)
                                    {
                                        //Update existing properties.
                                        if (orig.Properties.ContainsKey(key))
                                        {
                                            orig.Properties[key] = f.Properties[key];
                                        } 
                                        //Add new properties. Skip the internal Azure Maps shape ID property.
                                        else if(!key.Equals(Constants.AzureMapsShapeID))
                                        {
                                            orig.Properties.Add(key, f.Properties[key]);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                orig.Properties = f.Properties;
                            }
                        }
                    }
                }
            } 
            
            return (Features != null) ? Features: new List<Feature>();
        }

        #endregion
    }
}
