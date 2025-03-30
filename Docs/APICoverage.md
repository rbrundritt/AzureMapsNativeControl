# Web SDK API Capability Coverage

This library is a .NET wrapper for the Azure Maps Web SDK and many of its extension modules. This document provides detailed information on which capabilities are supported, and insights on new or alternative options.

## Azure Maps Module support

Status legend: :white_check_mark: Implemented | :hammer_and_wrench: In progress | :clipboard: Planned

| Status |  Module | Description & Notes |
| :---: | --- | --- |
| :white_check_mark: | [Animations](https://github.com/Azure-Samples/azure-maps-animations) | Adds animation capabilties. |
| :white_check_mark: | [Bring data into view control](https://github.com/Azure-Samples/azure-maps-bring-data-into-view-control) | Adds a simple button that analyizes all data added to the map, and repositions the map so it's all within the map view. |
| :white_check_mark: | [Drawing Tools](https://learn.microsoft.com/en-us/azure/azure-maps/set-drawing-options) | Drawing capabilites for the map. **Investigate possible extensions. There has been discussions in the dev community about adding more advance capabilities, Azure Maps dev team may also have plans** |
| :white_check_mark: | [Layer & Legend controls](https://github.com/Azure-Samples/azure-maps-layer-legend) | A control that provides UI controls for dynamically styling layers and displaying legends on the map. |
| :white_check_mark: | [Overview Map](https://github.com/Azure-Samples/azure-maps-overview-map) | A control that displays an overview map of the area the main map is focused on. |
| :white_check_mark: | [PMTiles](https://github.com/protomaps/PMTiles) support | Easily load PMTile data as a vector tile source. |
| :white_check_mark: | [Swipe Map](https://github.com/Azure-Samples/azure-maps-animations) | Allows swiping between two overlapping maps, ideal for comparing two overlapping data sets. |
| :white_check_mark: | [Gridded data source](https://github.com/Azure-Samples/azure-maps-gridded-data-source) | A data source that clusters data points into cells of a grid area. **Will investigate before deciding.** |

### Features & Modules under consideration

The following modules are being considered for future implementation.

|  Module | Description & Notes |
| --- | --- |
| [HTML Marker Layer](https://github.com/Azure-Samples/azure-maps-html-marker-layer) | A layer that renders point data from a data source as HTML elements on the map. **Will consider based on demand.** |
| [Image Exporter](https://github.com/Azure-Samples/azure-maps-image-exporter) | Generates screenshots of the map. **Investigate native options before deciding on this.** |
| [Spider Clusters](https://github.com/Azure-Samples/azure-maps-spider-clusters) | Adds a visualization to the map that expands clusters into a spiral spider layout.|
| [Spyglass Control](https://github.com/Azure-Samples/azure-maps-spyglass-control) | A window that displays a data set inside of a spyglass on the map. **Will consider based on demand.**  |
| [Selection Control](https://github.com/Azure-Samples/azure-maps-selection-control) | A controls for selecting data in a data source using drawing tools or by requesting a route range polygon. **Need to determine best option for integrating with REST services in this library** |

- Expose Simple Data Layer option (part of spatial io module, most of which is likely better done using native libraries)
- Custom WebGL Layers
- Hybrid geolocation control that ties into the native geolocation API rather than the browser version.
- Offline support - Requires using the NPM version of the Azure Maps Web SDK. Likely requires some modifications to the Azure Maps Web SDK to work with this library.
- If/When Upgrading to .NET 9 - For Maui, investigate moving to new built in HybridWebView control (may be lacking key features).

### Features & Modules not planned

|  Module | Description & Notes |
| --- | --- |
| Fullscreen control | There are two Fullscreen controls for Azure Maps, a [newer built in one](https://learn.microsoft.com/en-us/javascript/api/azure-maps-control/atlas.control.fullscreencontrol?view=azure-maps-typescript-latest), and an [older open source one](https://github.com/azure-samples/azure-maps-fullscreen-control/tree/main/). However, .MAUI does not support fullscreen mode, and WPF and WinUI require using native methods to put the app into fullscreen mode, this this web based control does not work. |
| [Geolocation control](https://github.com/Azure-Samples/azure-maps-geolocation-control) | Integrates a web browsers geolocation API with the map. **Likely better to use native geolocation APIs. Will add sample(s) that provide similar capabilities.** |
| [Scale Bar Control](https://github.com/Azure-Samples/azure-maps-scale-bar-control) | This capability is now built into Azure Maps and exposed through this library. |
| [Services UI](https://github.com/Azure-Samples/azure-maps-services-ui) | A set of web UI controls wrapping the Azure Maps REST services. **Most would likely prefer to do this in native UI** |
| [Spatial IO](https://learn.microsoft.com/azure/azure-maps/how-to-use-spatial-io-module) | Adds support for several spatial data formats, OGC layers, simple data layer. **Not planned as it is more efficient to use native libraries for this.** |
| [Sync Map](https://github.com/Azure-Samples/azure-maps-sync-maps) | Synchronizes the cameras of two or more maps. **Added a native implementation of this instead.**  |
| [Indoor Maps (Azure Maps Creator)](https://learn.microsoft.com/azure/azure-maps/) | Custom indoor maps via Azure Maps Creator platform. **Azure Maps Creator platform being retired. May consider alternatives in the future.** |

## Azure Maps Web SDK API coverage

The following table lists the names of the core Azure Maps Web SDK functions and features that are covered by this library.

| Web SDK Namespaces | C# namespace |
|--------------------|--------------|
| `atlas` | `AzureMapsNativeControl` |
| `atlas.control` | `AzureMapsNativeControl.Controls` |
| `atlas.data` | `Azure.Core.GeoJson` |
| `atlas.events` | `AzureMapsNativeControl.Events` |
| `atlas.layer` | `AzureMapsNativeControl.Layers` |
| `atlas.math` | `AzureMapsNativeControl.AtlasMath` |
| `atlas.source` | `AzureMapsNativeControl.Sources` |

## Root atlas namespace supported features

| Class name | Supported |
|---| --- |
| `atlas.Map` | :heavy_check_mark: |
| `atlas.Pixel` | :heavy_check_mark: |
| `atlas.Shape` | :heavy_check_mark: |

## atlas.data supported features

| Function name | Supported |
|---| --- |
| `atlas.data.BoundingBox` | :heavy_check_mark: |
| `atlas.data.Feature` | :heavy_check_mark: All `Azure.Core.GeoJson.GeoObject` objects are basically features. E.g. `Feature<Point>` = `GeoPoint` |
| `atlas.data.FeatureCollection` | :heavy_check_mark: |
| `atlas.data.Geometry` | :heavy_check_mark: |
| `atlas.data.LineString` | :heavy_check_mark: |
| `atlas.data.MercatorPoint` | :heavy_check_mark: |
| `atlas.data.MultiLineString` | :heavy_check_mark: |
| `atlas.data.MultiPoint` | :heavy_check_mark: |
| `atlas.data.MultiPolygon` | :heavy_check_mark: Uses `GeoPolygonCollection` |
| `atlas.data.Point` | :heavy_check_mark: Uses `PointGeometry` class. Renamed to reduce conflicts with similar named classes. |
| `atlas.data.Polygon` | :heavy_check_mark: |
| `atlas.data.Position` | :heavy_check_mark: |

### atlas.data.BoundingBox supported features

Most of these are exposed as non-static methods on the `BoundingBox` class in this library.

| Function name | Supported |
|---| --- |
| `atlas.data.BoundingBox.fromEdges` | :heavy_check_mark: Pass into constructor of `BoundingBox` class. |
| `atlas.data.BoundingBox.fromBoundingBox` | :heavy_check_mark: Use: `Clone` method on `BoundingBox` object. |
| `atlas.data.BoundingBox.fromData` | :heavy_check_mark: |
| `atlas.data.BoundingBox.fromLatLngs` | :x: |
| `atlas.data.BoundingBox.fromPositions` | :heavy_check_mark: Use: `AtlasMath.BoundsFromPositions` |
| `atlas.data.BoundingBox.fromDimensions` | :heavy_check_mark: Use: `AtlasMath.BoundsFromDimensions` |
| `atlas.data.BoundingBox.containsPosition` | :heavy_check_mark: Use: `Contains` method on `BoundingBox` object. |
| `atlas.data.BoundingBox.containsBoundingBox` | :heavy_check_mark: |
| `atlas.data.BoundingBox.crossesAntimeridian` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getCenter` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getEast` | :heavy_check_mark: Use: `BoundingBox` properties. |
| `atlas.data.BoundingBox.getHeight` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getNorth` | :heavy_check_mark: Use: `BoundingBox` properties. |
| `atlas.data.BoundingBox.getNorthEast` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getNorthWest` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getSouth` | :heavy_check_mark: Use: `BoundingBox` properties. |
| `atlas.data.BoundingBox.getSouthEast` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getSouthWest` | :heavy_check_mark: |
| `atlas.data.BoundingBox.getWest` | :heavy_check_mark: Use: `BoundingBox` properties. |
| `atlas.data.BoundingBox.getWidth` | :heavy_check_mark: |
| `atlas.data.BoundingBox.intersect` | :heavy_check_mark: |
| `atlas.data.BoundingBox.merge` | :heavy_check_mark: |
| `atlas.data.BoundingBox.splitOnAntimeridian` | :heavy_check_mark: |

**Additional features**

- `BufferToMinEdgeLength` - Inspects the width and height of the bounding box and buffers these if they are less than the specified edge length in degrees.

## atlas.math supported features

All math functions are exposed as static methods on the `AtlasMath` class. As an enhancement, [TileMath](https://learn.microsoft.com/en-us/azure/azure-maps/zoom-levels-and-tile-grid?tabs=csharp) has also been added to this library.

| Class name | Supported |
|---| --- |
| `atlas.math.AffineTransform` | :heavy_check_mark: |

| Function name | Supported |
|---| --- |
| `atlas.math.boundingBoxToPolygon` | :heavy_check_mark: Exposed as `ToPolygon` method on `BoundingBox` class. |
| `atlas.math.convertAcceleration` | :heavy_check_mark: |
| `atlas.math.convertDistance` | :heavy_check_mark: |
| `atlas.math.convertSpeed` | :heavy_check_mark: |
| `atlas.math.convertTimespan` | :x: Use TimeSpan class. |
| `atlas.math.getAcceleration` | :heavy_check_mark: |
| `atlas.math.getAccelerationFromSpeeds` | :heavy_check_mark: |
| `atlas.math.getAccelerationFromFeatures` | :heavy_check_mark: |
| `atlas.math.getArea` | :heavy_check_mark: |
| `atlas.math.getConvexHull` | :heavy_check_mark: |
| `atlas.math.getClosestPointOnGeometry` | :heavy_check_mark: |
| `atlas.math.getDistanceTo` | :heavy_check_mark: |
| `atlas.math.getCardinalSpline` | :heavy_check_mark: |
| `atlas.math.getEarthRadius` | :heavy_check_mark: |
| `atlas.math.getGeodesicPath` | :heavy_check_mark: |
| `atlas.math.getGeodesicPaths` | :x: Pass the result of `GetGeodesicPath` with `crossAntimerdian=false` into `GetPathSplitByAntimeridian`. |
| `atlas.math.getHeading` | :heavy_check_mark: |
| `atlas.math.getLengthOfPath` | :heavy_check_mark: |
| `atlas.math.getPathDenormalizedAtAntimerian` | :heavy_check_mark: |
| `atlas.math.getPathSplitByAntimeridian` | :heavy_check_mark: |
| `atlas.math.getPixelHeading` | :heavy_check_mark: |
| `atlas.math.getPointWithHeadingAlongPath` | :heavy_check_mark: |
| `atlas.math.getPointsWithHeadingsAlongPath` | :heavy_check_mark: |
| `atlas.math.getPositionAlongPath` | :heavy_check_mark: |
| `atlas.math.getPositionsAlongPath` | :heavy_check_mark: |
| `atlas.math.getPosition` | :x: Use GeoPoint.Coordinates |
| `atlas.math.getPositions` | :heavy_check_mark: |
| `atlas.math.getRegularPolygonPath` | :heavy_check_mark: |
| `atlas.math.getRegularPolygonPaths` | :x: Pass the result of `GenerateRegularPolygon` with `crossAntimerdian=false` into `GetPathSplitByAntimeridian`. |
| `atlas.math.getSpeed` | :heavy_check_mark: |
| `atlas.math.getSpeedFromFeatures` | :heavy_check_mark: |
| `atlas.math.getTimespan` | :x: Use TimeSpan class. |
| `atlas.math.getTravelDistance` | :heavy_check_mark: |
| `atlas.math.interpolate` | :heavy_check_mark: |
| `atlas.math.mercatorPixelsToPositions` | :heavy_check_mark: |
| `atlas.math.mercatorPositionsToPixels` | :heavy_check_mark: |
| `atlas.math.normalizeLatitude` | :heavy_check_mark: |
| `atlas.math.normalizeLongitude` | :heavy_check_mark: |
| `atlas.math.parseTimestamp` | :x: Use `FromJsonDateTime` and `ToJsonDateTime` methods. |
| `atlas.math.rotatePositions` | :heavy_check_mark: |
| `atlas.math.simplify` | :heavy_check_mark: |

**Additional features**

- Point in Polygon and Point in Circle calculations.
- Helper methods for converting to/from JSON date numbers to .NET DateTime.

## atlas.animations supported features

The animation module is dynamically loaded on demand by the map when using any `MapAnimation` or the `AnimatedTileLayer` class.

| Function name | Supported |
|---| --- |
| `atlas.animations.drop` | :heavy_check_mark: |
| `atlas.animations.dropMarkers` | :heavy_check_mark: |
| `atlas.animations.setCoordinates` | :heavy_check_mark: |
| `atlas.animations.moveAlongPath` | :heavy_check_mark: |
| `atlas.animations.moveAlongRoute` | :heavy_check_mark: |
| `atlas.animations.extractRoutePoints` | :heavy_check_mark: |
| `atlas.animations.morph` | :heavy_check_mark: |
| `atlas.animations.delay` | :x: |
| `atlas.animations.interval` | :x: |
| `atlas.animations.setInterval` | :x: |
| `atlas.animations.clearInterval` | :x: |
| `atlas.animations.setTimeout` | :x: |
| `atlas.animations.clearTimeout` | :x: |
| `atlas.animations.getEasingFn` | :x: |
| `atlas.animations.getEasingNames` | :x: |
| `atlas.animations.flowingDashedLine` | :heavy_check_mark: |
| `atlas.animations.fadeShapes` | :x: No samples exist, may not be adopted by users. Consider exposing in the future if needed. |

**Additional features** 

- `AnimatedTileLayer` class is supported.

## GriddedDataSource support

The [gridded data source module](https://github.com/Azure-Samples/azure-maps-gridded-data-source) is available in this project. In order to use it you must first load the GriddedDataSourceModule like so:

```csharp
await MyMap.LoadModuleAsync(AzureMapsModules.GriddedDataSourceModule);
```

## Interface objects

| Interface name | Supported |
|---| --- |
| `CameraOptions` | :heavy_check_mark: |
| `CameraBoundsOptions` | :heavy_check_mark: |
| `AccelerationUnits` | :heavy_check_mark: |
| `AreaUnits` | :heavy_check_mark: |
| `DistanceUnits` | :heavy_check_mark: |
| `SpeedUnits` | :heavy_check_mark: |
| `TimeUnits` | :x: |

## Azure.Core.GeoJson support

The `Azure.Core.GeoJson` namespace contains classes that represent GeoJSON objects. Support for these objects is provided in this library as the [Azure Maps REST clients](https://learn.microsoft.com/en-us/azure/azure-maps/rest-sdk-developer-guide) do (or will) leverage this library. By supporting these objects, you will be able to easily use the [Azure Maps REST clients](https://learn.microsoft.com/en-us/azure/azure-maps/rest-sdk-developer-guide) with this library.

This library has its own GeoJSON that are observable objects, and have enhanced features and JSON serialization capabilities.

You can convert a `Azure.Core.GeoJson` object by simply passing into the constructor of the corresponding object in this library. For example, to convert a `Azure.Core.GeoJson.Point` object to a `PointGeometry` object, pass the `Point` object into the constructor of the `PointGeometry` object.

All GeoJson object from this library object have a `ToGeoObject` that will convert the object to a `Azure.Core.GeoJson` object. For example, to convert a `PointGeometry` object to a `Azure.Core.GeoJson.Point` object, use the `ToGeoObject` method on the `PointGeometry` object.

