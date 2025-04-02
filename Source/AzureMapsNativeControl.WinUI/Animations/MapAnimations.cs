using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

#if MAUI
using Microsoft.Maui.ApplicationModel;
#else

#endif

namespace AzureMapsNativeControl.Animations
{
    /// <summary>
    /// Map animations.
    /// </summary>
    public static class MapAnimations
    {
        #region Group Animations

        /// <summary>
        /// Creates a group animation. This is used to create a group animation from a list of animations.
        /// </summary>
        /// <param name="animations">The animations to group.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<GroupAnimation?> GroupAnimationsAsync(IList<IPlayableAnimation?> animations, GroupAnimationOptions? options = null)
        {
            Map? map = null;

            //Get the IDs of the animations.
            var animationIds = new List<string>();
            foreach (var animation in animations)
            {
                if (animation != null && animation is IPlayableAnimation)
                {
                    animationIds.Add(animation.Id);

                    if (map == null)
                    {
                        if (animation is PlayableAnimation)
                        {
                            map = ((PlayableAnimation)animation).Map;
                        }
                        else if (animation is GroupAnimation)
                        {
                            map = ((GroupAnimation)animation).Map;
                        }
                        else if(animation is MapEntity<LayerOptions>)
                        {
                            map = ((MapEntity<LayerOptions>)animation).Map;
                        }
                    }
                }
            }

            //Make sure we have a map and at least one animation.
            if (map != null && animationIds.Count > 0)
            {
                await LoadAnimationModule(map);

                var groupAnimation = new GroupAnimation(map);

                //async groupAnimation(animationIds, options)
                await map.JsInterlop.InvokeJsMethodAsync(map, "groupAnimation", groupAnimation.Id, animationIds, options);

                return groupAnimation;
            }

            return null;
        }

        #endregion

        #region SetCoordinates Animation

        /// <summary>
        /// Sets the coordinates of a Point feature and animates it to the new position.
        /// </summary>
        /// <param name="feature">Point feature to animate to a new position.</param>
        /// <param name="newPosition">The new position to animate to.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> SetCoordinates(Feature feature, Position newPosition, Map map, string sourceId, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            await LoadAnimationModule(map);

            if (feature.SourceId == sourceId || string.IsNullOrEmpty(feature.SourceId))
            {
                //Check to see if the line is a Point.
                if (feature.Geometry.Type != GeoJsonType.Point)
                {
                    throw new ArgumentException("Feature must be a Point.");
                }

                var source = ValidateFeatureInDataSource(feature, map, sourceId);
                if (source != null)
                { 
                    var animation = new PlayableAnimation(map);
                    //async setCoordinatesAnimation(animationId, feature, newPosition, sourceId, options, animateMap = false) 
                    await map.JsInterlop.InvokeJsMethodAsync(map, "setCoordinatesAnimation", animation.Id, feature.Id, newPosition, sourceId, options, animateMap);

                    //When this animation is complete, set the coordinates of the feature in the .NET version of the data source to the new position.
                    map.Events.Add("oncomplete", animation, (s, e) =>
                    {
                        (feature.Geometry as PointGeometry).Coordinates = newPosition;
                    });               

                    return animation;
                }
            }
            else
            {
                throw new ArgumentException("Feature must be in the same source as the one passed in.");
            }

            return null;
        }

        /// <summary>
        /// Sets the coordinates of a Point feature and animates it to the new position.
        /// </summary>
        /// <param name="feature">Point feature to animate to a new position.</param>
        /// <param name="newPosition">The new position to animate to.</param>
        /// <param name="source">The data source the feature is or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> SetCoordinates(Feature feature, Position newPosition, DataSourceLite source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source != null && feature != null)
            {
                if (source.Map == null)
                {
                    throw new ArgumentNullException("Source must already be added to a map.");
                }

                return await SetCoordinates(feature, newPosition, source.Map, source.Id, options, animateMap);
            }

            return null;
        }

        /// <summary>
        /// Sets the coordinates of a Point feature and animates it to the new position.
        /// </summary>
        /// <param name="feature">Point feature to animate to a new position.</param>
        /// <param name="newPosition">The new position to animate to.</param>
        /// <param name="source">The data source the feature is or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> SetCoordinates(Feature feature, Position newPosition, DataSource source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source != null && feature != null)
            {
                if (source.Map == null)
                {
                    throw new ArgumentNullException("Source must already be added to a map.");
                }

                return await SetCoordinates(feature, newPosition, source.Map, source.Id, options, animateMap);
            }

            return null;
        }

        /// <summary>
        /// Sets the coordinates of a HtmlMarker and animates it to the new position.
        /// </summary>
        /// <param name="marker">The marker to animate the new position of.</param>
        /// <param name="newPosition">The new position to animate to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> SetCoordinates(HtmlMarker marker, Position newPosition, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (marker != null)
            {
                if (marker.Map == null)
                {
                    throw new ArgumentNullException("Marker must already be added to a map.");
                }

                await LoadAnimationModule(marker.Map);

                var animation = new PlayableAnimation(marker.Map);
                //async setCoordinatesAnimation(animationId, featureId, newPosition, sourceId, options)
                await marker.Map.JsInterlop.InvokeJsMethodAsync(marker.Map, "setCoordinatesAnimation", animation.Id, marker.Id, newPosition, null, options, animateMap);

                //When this animation is complete, set the coordinates of the feature in the .NET version of the data source to the new position.
                marker.Map.Events.Add("oncomplete", animation, (s, e) =>
                {
                    marker.SetOptions(new HtmlMarkerOptions
                    {
                        Position = newPosition
                    });

                    //Dispose the animation as this type of animation can only be done once.
                    animation.Dispose();
                });

                return animation;
            }

            return null;
        }

        #endregion

        #region Drop Animation

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="point">A point geometries to drop in.</param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>A playable animation or null if no point geometries in the data.</returns>
        public static async Task<PlayableAnimation?> DropAsync(PointGeometry point, DataSourceLite source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync([point], source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="feature">A point feature to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropAsync(Feature feature, DataSourceLite source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync([feature], source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="point">A point geometries to drop in.</param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>A playable animation or null if no point geometries in the data.</returns>
        public static async Task<PlayableAnimation?> DropAsync(PointGeometry point, DataSource source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync([point], source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="feature">A point feature to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropAsync(Feature feature, DataSource source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync([feature], source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="points">A one or more point geometries or shapes to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>A playable animation or null if no point geometries in the data.</returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<PointGeometry> points, DataSourceLite source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync(points, source.Map, source.Id, height, options);
        }

        /// <summary>
        ///  Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="features">A one or more point geometries or shapes to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<Feature> features, DataSourceLite source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync(features, source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="points">A one or more point geometries or shapes to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>A playable animation or null if no point geometries in the data.</returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<PointGeometry> points, DataSource source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync(points, source.Map, source.Id, height, options);
        }

        /// <summary>
        ///  Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="features">A one or more point geometries or shapes to drop in. </param>
        /// <param name="source">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<Feature> features, DataSource source, int? height = null, PlayableAnimationOptions? options = null)
        {
            return await DropAsync(features, source.Map, source.Id, height, options);
        }

        /// <summary>
        /// Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="points">A one or more point geometries or shapes to drop in. </param>
        /// <param name="sourceId">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns>A playable animation or null if no point geometries in the data.</returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<PointGeometry> points, Map? map, string sourceId, int? height = null, PlayableAnimationOptions? options = null)
        {
            if (map != null && points != null && points.Count > 0)
            {
                await LoadAnimationModule(map);
                var animation = new PlayableAnimation(map);

                //async dropAnimation(animationId, pointFeatures, sourceId, height, options) 
                await map.JsInterlop.InvokeJsMethodAsync(map, "dropAnimation", animation.Id, points, sourceId, height, options);

                return animation;
            }

            return null;
        }

        /// <summary>
        ///  Adds an offset array property to point shapes and animates it's y value to simulate dropping. 
        /// Use with a symbol layer with the icon/text offset property set to['get', 'offset'] and the opacity set to['get', 'opacity'].
        /// </summary>
        /// <param name="features">A one or more point geometries or shapes to drop in. </param>
        /// <param name="sourceId">The data source to drop the point shapes into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropAsync(IList<Feature> features, Map? map, string sourceId, int? height = 200, PlayableAnimationOptions? options = null)
        {
            if (map != null && features != null && features.Count > 0)
            {
                await LoadAnimationModule(map);

                //Extract features that are points. Non-point features are ignored.
                var points = features.Where(f => f.Geometry.Type == GeoJsonType.Point).ToList();

                if (points != null && points.Count > 0)
                {
                    var animation = new PlayableAnimation(map);

                    //async dropAnimation(animationId, pointFeatures, sourceId, height, options) 
                    await map.JsInterlop.InvokeJsMethodAsync(map, "dropAnimation", animation.Id, points, sourceId, height, options);
                    return animation;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds an offset to a HtmlMarker to animate it's y value to simulate dropping. Animation modifies `pixelOffset` value of HtmlMarkers. 
        /// </summary>
        /// <param name="marker">The marker to drop.</param>
        /// <param name="map">The map to drop the markers into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropMarkersAsync(HtmlMarker marker, Map map, int? height = 200, PlayableAnimationOptions? options = null)
        {
            return await DropMarkersAsync([marker], map, height, options);
        }

        /// <summary>
        /// Adds an offset to HtmlMarkers to animate it's y value to simulate dropping. Animation modifies `pixelOffset` value of HtmlMarkers. 
        /// </summary>
        /// <param name="markers">The markers to drop.</param>
        /// <param name="map">The map to drop the markers into.</param>
        /// <param name="height">The height at which to drop the shape from. Default: 200 pixels</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> DropMarkersAsync(IList<HtmlMarker> markers, Map map, int? height = null, PlayableAnimationOptions? options = null)
        {
            if (map != null && markers != null && markers.Count > 0)
            {
                await LoadAnimationModule(map);

                var markerIds = new List<string>();

                //Make sure all markers are not visible and get their IDs.
                foreach(var m in markers)
                {
                    m.SetOptions(new HtmlMarkerOptions
                    {
                        Visible = false
                    });

                    markerIds.Add(m.Id);
                }

                //Make sure all markers have been loaded into the map.
                await map.Markers.AddRangeAsync(markers);

                var animation = new PlayableAnimation(map);

                //async dropMarkersAnimation(animationId, markerIds, height, options) 
                await map.JsInterlop.InvokeJsMethodAsync(map, "dropMarkersAnimation", animation.Id, markerIds, height, options);
                return animation;
            }

            return null;
        }

        #endregion

        #region Snakeline Animation

        /// <summary>
        /// Animates a line along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="line">The line feature to animate.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the line feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along with the line.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> Snakeline(Feature line, Map map, string sourceId, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            await LoadAnimationModule(map);

            // Check to see if the line is in the same source as the one passed in, or not added to a source yet.
            if (line.SourceId == sourceId || string.IsNullOrEmpty(line.SourceId))
            {
                //Check to see if the line is a LineString.
                if (line.Geometry.Type != GeoJsonType.LineString)
                {
                    throw new ArgumentException("Line must be a LineString.");
                }

                var source = ValidateFeatureInDataSource(line, map, sourceId);
                if (source != null)
                {
                    var animation = new PlayableAnimation(map);
                    //async snakelineAnimation(animationId, line, sourceId, options, animateMap = false) 
                    await map.JsInterlop.InvokeJsMethodAsync(map, "snakelineAnimation", animation.Id, line.Id, sourceId, options, animateMap);
                    return animation;
                }
            } 
            else
            {
                throw new ArgumentException("Line must be in the same source as the one passed in.");
            }

            return null;
        }

        /// <summary>
        /// Animates a line along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="line">The line feature to animate.</param>
        /// <param name="source">The data source the line feature is or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along with the line.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> Snakeline(Feature line, DataSourceLite source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source != null && line != null)
            {
                if (source.Map == null)
                {
                    throw new ArgumentNullException("Source must already be added to a map.");
                }

                return await Snakeline(line, source.Map, source.Id, options, animateMap);
            }

            return null;
        }

        /// <summary>
        /// Animates a line along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="line">The line feature to animate.</param>
        /// <param name="source">The data source the line feature is or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along with the line.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PlayableAnimation?> Snakeline(Feature line, DataSource source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source != null && line != null)
            {
                if (source.Map == null)
                {
                    throw new ArgumentNullException("Source must already be added to a map.");
                }

                return await Snakeline(line, source.Map, source.Id, options, animateMap);
            }

            return null;
        }

        #endregion

        #region Move Along Path Animation

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(IEnumerable<Position> path, Feature point, Map map, string sourceId, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            await LoadAnimationModule(map);

            //Ensure the path is not null or empty, and has atleast 2 points.
            if (path == null || path.Count() < 2)
            {
                throw new ArgumentException("Path must have at least 2 points.");
            }

            // Check to see if the point is in the same source as the one passed in, or not added to a source yet.
            if (point.SourceId == sourceId || string.IsNullOrEmpty(point.SourceId))
            {
                //Check to see if the line is a Point.
                if (point.Geometry.Type != GeoJsonType.Point)
                {
                    throw new ArgumentException("Point must be a Point Geometry Feature.");
                }

                var source = ValidateFeatureInDataSource(point, map, sourceId);
                if (source != null)
                {
                    var animation = new PlayableAnimation(map);
                    //async moveAlongPathAnimation(animationId, path, point, sourceId, options, animateMap = false)
                    await map.JsInterlop.InvokeJsMethodAsync(map, "moveAlongPathAnimation", animation.Id, path, point.Id, sourceId, options, animateMap);
                    
                    return animation;
                }
            }
            else
            {
                throw new ArgumentException("Point must be in the same source as the one passed in.");
            }

            return null;
        }

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(LineString path, Feature point, Map map, string sourceId, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            return await MoveAlongPath(path.Coordinates, point, map, sourceId, options, animateMap);
        }

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(IEnumerable<Position> path, Feature point, DataSource source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            return await MoveAlongPath(path, point, source.Map, source.Id, options, animateMap);
        }

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(IEnumerable<Position> path, Feature point, DataSourceLite source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            return await MoveAlongPath(path, point, source.Map, source.Id, options, animateMap);
        }

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(LineString path, Feature point, DataSource source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            return await MoveAlongPath(path.Coordinates, point, source, options, animateMap);        
        }

        /// <summary>
        /// Moves a point along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(LineString path, Feature point, DataSourceLite source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            return await MoveAlongPath(path.Coordinates, point, source, options, animateMap);
        }

        /// <summary>
        /// Moves a marker along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="marker">The marker to animate.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(IEnumerable<Position> path, HtmlMarker marker, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            //Ensure the path is not null or empty, and has atleast 2 points.
            if (path == null || path.Count() < 2)
            {
                throw new ArgumentException("Path must have at least 2 points.");
            }

            if (marker != null)
            {
                if (marker.Map == null)
                {
                    throw new ArgumentNullException("Marker must already be added to a map.");
                }

                await LoadAnimationModule(marker.Map);

                var animation = new PlayableAnimation(marker.Map);
                //async moveAlongPathAnimation(animationId, path, point, sourceId, options, animateMap = false)
                await marker.Map.JsInterlop.InvokeJsMethodAsync(marker.Map, "moveAlongPathAnimation", animation.Id, path, marker.Id, null, options, animateMap);

                return animation;
            }

            return null;
        }

        /// <summary>
        /// Moves a marker along a path. Can also animate the maps camera along with the line as well.
        /// </summary>
        /// <param name="path">The path to move the point along.</param>
        /// <param name="marker">The marker to animate.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongPath(LineString path, HtmlMarker marker, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            //Ensure the path is not null or empty, and has atleast 2 points.
            if (path == null || path.Coordinates.Count() < 2)
            {
                throw new ArgumentException("Path must have at least 2 points.");
            }

            return await MoveAlongPath(path.Coordinates, marker, options, animateMap);
        }

        #endregion

        #region Move along route

        /// <summary>
        ///  Animates a map and/or a Point shape along a route path. 
        ///  The movement will vary based on timestamps within the point feature properties. 
        ///  All points must have a `timestamp` property that is a `Date.getTime()` value. Use the `extractRoutePoints` function to preprocess data.
        /// </summary>
        /// <param name="route">The route points to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongRoute(IList<Feature> route, Feature point, Map map, string sourceId, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            await LoadAnimationModule(map);

            //Ensure the path is not null or empty, and has atleast 2 points.
            if (route == null || route.Count() < 2)
            {
                throw new ArgumentException("Route must have at least 2 points.");
            }

            // Check to see if the point is in the same source as the one passed in, or not added to a source yet.
            if (point.SourceId == sourceId || string.IsNullOrEmpty(point.SourceId))
            {
                //Check to see if the line is a Point.
                if (point.Geometry.Type != GeoJsonType.Point)
                {
                    throw new ArgumentException("Point must be a Point Geometry Feature.");
                }

                var source = ValidateFeatureInDataSource(point, map, sourceId);
                if (source != null)
                {
                    var animation = new PlayableAnimation(map);
                    //async moveAlongPathAnimation(animationId, path, point, sourceId, options, animateMap = false)
                    await map.JsInterlop.InvokeJsMethodAsync(map, "moveAlongRouteAnimation", animation.Id, route, point.Id, sourceId, options, animateMap);

                    return animation;
                }
            }
            else
            {
                throw new ArgumentException("Point must be in the same source as the one passed in.");
            }

            return null;
        }

        /// <summary>
        ///  Animates a map and/or a Point shape along a route path. 
        ///  The movement will vary based on timestamps within the point feature properties. 
        ///  All points must have a `timestamp` property that is a `Date.getTime()` value. Use the `extractRoutePoints` function to preprocess data.
        /// </summary>
        /// <param name="route">The route points to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongRoute(IList<Feature> route, Feature point, DataSource source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (point == null)
            {
                throw new ArgumentNullException("Point must be a feature.");
            }

            return await MoveAlongRoute(route, point, source.Map, source.Id, options, animateMap);
        }

        /// <summary>
        ///  Animates a map and/or a Point shape along a route path. 
        ///  The movement will vary based on timestamps within the point feature properties. 
        ///  All points must have a `timestamp` property that is a `Date.getTime()` value. Use the `extractRoutePoints` function to preprocess data.
        /// </summary>
        /// <param name="route">The route points to move the point along.</param>
        /// <param name="point">The point to animate.</param>
        /// <param name="source">The data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongRoute(IList<Feature> route, Feature point, DataSourceLite source, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            if (point == null)
            {
                throw new ArgumentNullException("Point must be a feature.");
            }

            return await MoveAlongRoute(route, point, source.Map, source.Id, options, animateMap);
        }

        /// <summary>
        ///  Animates a map and/or a Point shape along a route path. 
        ///  The movement will vary based on timestamps within the point feature properties. 
        ///  All points must have a `timestamp` property that is a `Date.getTime()` value. Use the `extractRoutePoints` function to preprocess data.
        /// </summary>
        /// <param name="route">The route points to move the point along.</param>
        /// <param name="marker">The marker to animate.</param>
        /// <param name="options">Options for the animation.</param>
        /// <param name="animateMap">Specifies if the map should be animated along path.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> MoveAlongRoute(IList<Feature> route, HtmlMarker marker, MapPathAnimationOptions? options = null, bool animateMap = false)
        {
            //Ensure the path is not null or empty, and has atleast 2 points.
            if (route == null || route.Count() < 2)
            {
                throw new ArgumentException("Route must have at least 2 points.");
            }

            if (marker != null)
            {
                if (marker.Map == null)
                {
                    throw new ArgumentNullException("Marker must already be added to a map.");
                }

                await LoadAnimationModule(marker.Map);

                var animation = new PlayableAnimation(marker.Map);
                //async moveAlongPathAnimation(animationId, path, point, sourceId, options, animateMap = false)
                await marker.Map.JsInterlop.InvokeJsMethodAsync(marker.Map, "moveAlongRouteAnimation", animation.Id, route, marker.Id, null, options, animateMap);

                return animation;
            }

            return null;
        }

        /// <summary>
        /// Extracts points from a shapes or features that form a time based route and sorts them by time. 
        /// Timestamps must parsable by the `atlas.math.parseTimestamp` function.
        /// All extracted points will have a `_timestamp` property that contains the Date.getTime() value.
        /// Features must be a Point, MultiPoint, or LineString and must contain properties that include timestamp information. 
        /// If a timestamp property name is not specified, `_timestamp` will be used.
        /// If a feature collection is passed in, the first shape with a matching timestamp property will dictate what is extracted.
        /// If the first shape is a Point, all points in the colleciton with the timestamp property will be extracted.
        /// If the first shape is a LineString or MultiLineString;
        /// - If it contains a timestamp property with an array of values the same length as coordinates in the feature, new Point features will be created from a combination of the coordinates and timestamp values.
        /// - If the feature has a `waypoints` property that contains an array of Point features with the timestamp property and the same number of coordinates, then these p will be extracted.
        /// </summary>
        /// <param name="fc">Collection of features to extract route points from.</param>
        /// <param name="timestampProperty">The property with timestamp info.</param>
        /// <returns></returns>
        public static IList<Feature> ExtractRoutePoints(FeatureCollection fc, string? timestampProperty = "_timestamp")
        {
            return ExtractRoutePoints(fc.Features, timestampProperty);
        }

        /// <summary>
        /// Extracts points from a shapes or features that form a time based route and sorts them by time. 
        /// Timestamps must parsable by the `atlas.math.parseTimestamp` function.
        /// All extracted points will have a `_timestamp` property that contains the Date.getTime() value.
        /// Features must be a Point, MultiPoint, or LineString and must contain properties that include timestamp information. 
        /// If a timestamp property name is not specified, `_timestamp` will be used.
        /// If a feature collection is passed in, the first shape with a matching timestamp property will dictate what is extracted.
        /// If the first shape is a Point, all points in the colleciton with the timestamp property will be extracted.
        /// If the first shape is a LineString or MultiLineString;
        /// - If it contains a timestamp property with an array of values the same length as coordinates in the feature, new Point features will be created from a combination of the coordinates and timestamp values.
        /// - If the feature has a `waypoints` property that contains an array of Point features with the timestamp property and the same number of coordinates, then these p will be extracted.
        /// </summary>
        /// <param name="features">Collection of features to extract route points from.</param>
        /// <param name="timestampProperty">The property with timestamp info.</param>
        /// <returns></returns>
        public static IList<Feature> ExtractRoutePoints(IEnumerable<Feature> features, string? timestampProperty = "_timestamp")
        {
            if (string.IsNullOrEmpty(timestampProperty))
            {
                timestampProperty = "_timestamp";
            }

            var points = new List<Feature>();

            if(features != null && features.Count() != 0)
            {
                var firstFeature = features.ElementAt(0);

                if(firstFeature.Geometry.Type == GeoJsonType.Point)
                {
                    //Extract only point features.
                    foreach (var f in features)
                    {
                        if (f.Geometry.Type == GeoJsonType.Point)
                        {
                            if (f.Properties.ContainsKey(timestampProperty))
                            {
                                var t = AtlasMath.FromJsonDateTime(f.Properties[timestampProperty]);
                                if (t != null)
                                {
                                    f.Properties.Add("_timestamp", AtlasMath.ToJsonDateTime(t.Value));
                                    points.Add(f);
                                }
                            }
                        }
                    }
                } 
                else if (firstFeature.Geometry.Type == GeoJsonType.LineString || firstFeature.Geometry.Type == GeoJsonType.MultiPoint)
                {
                    //Extract points from this single feature.
                    return ExtractRoutePoints(firstFeature, timestampProperty);
                }
            }

            if (points.Count > 0)
            {
                //Sort the points by timestamp.
                points = points.OrderBy(p => p.Properties.ContainsKey("_timestamp") ? p.Properties["_timestamp"] : null).ToList();
            }

            return points;
        }

        /// <summary>
        /// Extracts points from a shapes or features that form a time based route and sorts them by time. 
        /// Timestamps must parsable by the `atlas.math.parseTimestamp` function.
        /// All extracted points will have a `_timestamp` property that contains the Date.getTime() value.
        /// Features must be a Point, MultiPoint, or LineString and must contain properties that include timestamp information. 
        /// If a timestamp property name is not specified, `_timestamp` will be used.
        /// If a feature collection is passed in, the first shape with a matching timestamp property will dictate what is extracted.
        /// If the first shape is a Point, all points in the colleciton with the timestamp property will be extracted.
        /// If the first shape is a LineString or MultiLineString;
        /// - If it contains a timestamp property with an array of values the same length as coordinates in the feature, new Point features will be created from a combination of the coordinates and timestamp values.
        /// - If the feature has a `waypoints` property that contains an array of Point features with the timestamp property and the same number of coordinates, then these p will be extracted.
        /// </summary>
        /// <param name="feature">Feature to extract route points from.</param>
        /// <param name="timestampProperty">The property with timestamp info.</param>
        /// <returns></returns>
        public static IList<Feature> ExtractRoutePoints(Feature feature, string? timestampProperty = null)
        {
            if (string.IsNullOrEmpty(timestampProperty))
            {
                timestampProperty = "_timestamp";
            }

            var points = new List<Feature>();

            if (feature.Properties != null)
            {
                if (feature.Geometry.Type == GeoJsonType.Point)
                {
                    if (feature.Properties.ContainsKey(timestampProperty))
                    {
                        var t = AtlasMath.FromJsonDateTime(feature.Properties[timestampProperty]);
                        if (t != null)
                        {
                            feature.Properties.Add("_timestamp", AtlasMath.ToJsonDateTime(t.Value));
                            points.Add(feature);
                        }
                    }
                }

                if (feature.Geometry.Type == GeoJsonType.LineString || feature.Geometry.Type == GeoJsonType.MultiPoint)
                {
                    var coords = (feature.Geometry.Type == GeoJsonType.LineString) ?
                        (feature.Geometry as LineString).Coordinates : (feature.Geometry as MultiPoint).Coordinates;

                    if (coords.Count > 0)
                    {
                        if (feature.Properties.ContainsKey(timestampProperty))
                        {
                            //Check to see if the timestamp property is an array of values.
                            var ts = feature.Properties[timestampProperty] as IEnumerable<object>;

                            if(ts != null && ts.Count() == coords.Count)
                            {
                                for (int i = 0; i < coords.Count; i++)
                                {
                                    var t = AtlasMath.FromJsonDateTime(ts.ElementAt(i));
                                    if (t != null)
                                    {
                                        var point = new Feature(new PointGeometry(coords[i]), feature.Properties);
                                        point.Properties.Add("_timestamp", AtlasMath.ToJsonDateTime(t.Value));
                                        points.Add(point);
                                    }
                                }
                            }
                        }
                        //Check to see if the feature has waypoints in its properties which may contain the route point info.
                        else if (feature.Properties.ContainsKey("waypoints"))
                        {
                            var waypoints = feature.Properties["waypoints"] as IEnumerable<Feature>;
                            if (waypoints != null && waypoints.Count() == coords.Count)
                            {
                                foreach (var w in waypoints)
                                {
                                    if (w.Geometry.Type == GeoJsonType.Point && w.Properties != null && w.Properties.ContainsKey(timestampProperty))
                                    {
                                        var t = AtlasMath.FromJsonDateTime(w.Properties[timestampProperty]);
                                        if (t != null)
                                        {
                                            w.Properties.Add("_timestamp", AtlasMath.ToJsonDateTime(t.Value));
                                            points.Add(w);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(points.Count > 0)
            {
                //Sort the points by timestamp.
                points = points.OrderBy(p => p.Properties.ContainsKey("_timestamp") ? p.Properties["_timestamp"] : null).ToList();
            }

            return points;
        }

        #endregion

        #region Morph Animations

        /// <summary>
        /// Animates the morphing of a shape from one geometry type or set of coordinates to another.
        /// </summary>
        /// <param name="feature">The feature to animate. Must be added to the source already.</param>
        /// <param name="newGeometry">The new geometry to turn the shape into.</param>
        /// <param name="map">The map instance being used.</param>
        /// <param name="sourceId">The id of the data source the feature is in or should be added to.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        public static async Task<PlayableAnimation?> Morph(Feature feature, Geometry newGeometry, Map map, string sourceId, PlayableAnimationOptions? options = null)
        {
            await LoadAnimationModule(map);

            // Check to see if the feature is in the same source as the one passed in, or not added to a source yet.
            if (feature.SourceId == sourceId || string.IsNullOrEmpty(feature.SourceId))
            {
                var source = ValidateFeatureInDataSource(feature, map, sourceId);

                if (source != null)
                {
                    var animation = new PlayableAnimation(map);
                    //async morphAnimation(animationId, featureId, sourceId, newGeometry, options) 
                    await map.JsInterlop.InvokeJsMethodAsync(map, "morphAnimation", animation.Id, feature.Id, sourceId, newGeometry, options);

                    return animation;
                }
            }
            else
            {
                throw new ArgumentException("Feature must be in the same source as the one passed in.");
            }

            return null;
        }

        /// <summary>
        /// Animates the morphing of a shape from one geometry type or set of coordinates to another.
        /// </summary>
        /// <param name="feature">The feature to animate. Must be added to the source already.</param>
        /// <param name="newGeometry">The new geometry to turn the shape into.</param>
        /// <param name="source">The source the feature is in.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PlayableAnimation?> Morph(Feature feature, Geometry newGeometry, DataSource source, PlayableAnimationOptions? options = null)
        {
            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            var animation = await Morph(feature, newGeometry, source.Map, source.Id, options);

            //Update the feature in the data source, but don't trigger an update in the map.
            feature.BoundingBox = newGeometry.BoundingBox;
            feature.Geometry = newGeometry;

            return animation;
        }

        /// <summary>
        /// Animates the morphing of a shape from one geometry type or set of coordinates to another.
        /// </summary>
        /// <param name="feature">The feature to animate. Must be added to the source already.</param>
        /// <param name="newGeometry">The new geometry to turn the shape into.</param>
        /// <param name="source">The source the feature is in.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PlayableAnimation?> Morph(Feature feature, Geometry newGeometry, DataSourceLite source, PlayableAnimationOptions? options = null)
        {
            if (source.Map == null)
            {
                throw new ArgumentNullException("Source must already be added to a map.");
            }

            return await Morph(feature, newGeometry, source.Map, source.Id, options);
        }

        #endregion

        #region Flowing dashed lines

        /// <summary>
        /// Animates the dash-array of a line layer to make it appear to flow.
        /// The lineCap option of the layer must not be 'round'. If it is, it will be changed to 'butt'.
        /// </summary>
        /// <param name="layer">The layer to animate.</param>
        /// <param name="options">Options for the animation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PlayableAnimation?> FlowingDashedLine(LineLayer layer, FlowingDashLineOptions? options = null)
        {
            if(layer == null)
            {
                throw new ArgumentNullException("Layer must not be null.");
            }

            if (layer.Map == null)
            {
                throw new ArgumentNullException("Layer must already be added to a map.");
            }
            await LoadAnimationModule(layer.Map);

            var animation = new PlayableAnimation(layer.Map);

            //async flowingDashedLineAnimation(animationId, layerId, options)
            await layer.Map.JsInterlop.InvokeJsMethodAsync(layer.Map, "flowingDashedLineAnimation", animation.Id, layer.Id, options);
            return animation;
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Verify if a feature is in a data source, and if it is not, add it if it isn't in another one.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="map"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        private static BaseSource? ValidateFeatureInDataSource(Feature feature, Map map, string sourceId)
        {
            BaseSource? source = null;
            var s = map.Sources.GetById(sourceId);
            if (s != null)
            {
                if (s is DataSource)
                {
                    var ds = (DataSource)s;
                    if (string.IsNullOrEmpty(feature.SourceId))
                    {
                        // If the line is not in the source, add it to the source.
                        ds.Add(feature);
                    }
                    source = ds;
                }
                else if (s is DataSourceLite)
                {
                    var ds = (DataSourceLite)s;
                    if (string.IsNullOrEmpty(feature.SourceId))
                    {
                        // If the line is not in the source, add it to the source.
                        ds.Add(feature);
                    }
                    source = ds;
                }
            }

            return source;
        }

        internal static async Task LoadAnimationModule(Map map)
        {
            await map.JsInterlop.LoadModule(AzureMapsModules.AnimationModule);
        }

        #endregion
    }
}
