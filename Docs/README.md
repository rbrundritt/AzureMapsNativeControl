# Azure Maps Native Control Docs

The Azure Maps Native Control is a .NET library that wraps the Azure Maps Web SDK. Nearly all of the Azure Maps Web SDK functionality and capabilities are exposed through a similar API interface. 

See the [API Coverage](APICoverage.md) document in depth details of the API coverage and differences between the Azure Maps Web SDK and the Azure Maps Native Control.

## Getting Started

1. Before loading the map instance, you need to setup the global map configuration. In the configuration you can add your Azure Maps authenication information have the option to set some low level options, such as disabling telemetry. Note that Microsoft Entra ID can also be used for authentication.

    ### .NET Maui Apps

    In .NET Maui this is handled via dependancy injection in the `MauiProgram.cs` file, within the `CreateMauiApp` method, add the following code:

    ### WinUI Apps

    In WinUI apps the configuration can be set anytime before the map instance is initialized. A good place to put this is in the `OnLaunched` method of the `App.xaml.cs` file. 

    ```csharp
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        AzureMapsConfiguration.Configure(new AzureMapsConfiguration
        {
            // Set the subscription key
            SubscriptionKey = "<Your Azure Maps Key>"
        });

        m_window = new MainWindow();
        m_window.Activate();
    }
    ```

    ### WPF Apps

    In WinUI apps the configuration can be set anytime before the map instance is initialized. A good place to put this is in the `OnStartup` method of the `App.xaml.cs` file (You may need to add this method to this file). 

    ```csharp
    protected override void OnStartup(StartupEventArgs e)
    {
        AzureMapsConfiguration.Configure(new AzureMapsConfiguration
        {
            // Set the subscription key
            SubscriptionKey = "<Your Azure Maps Key>"
        });

        base.OnStartup(e);
    }
    ```

2. In your XAML file, add references to the Azure Maps namespaces:

    ### .NET Maui & WPF Apps

    ```xml
    xmlns:atlas="clr-namespace:AzureMapsNativeControl;assembly=AzureMapsNativeControl"             

    <!-- Add this reference if you want to add controls to the map in XAML -->
    xmlns:atlasControl="clr-namespace:AzureMapsNativeControl.Control;assembly=AzureMapsNativeControl"
    ```

    ### WinUI Apps

    ```xml
    xmlns:atlas="using:AzureMapsNativeControl"

    <!-- Add this reference if you want to add controls to the map in XAML -->
    xmlns:atlasControl="using:AzureMapsNativeControl.Control"
    ```

3. In your XAML file, add the map control to your layout:

    ```xml
    <atlas:Map x:Name="MyMap"/>
    ```

    Here is a more advanced example that sets a bunch of settings for when the map loads, adds an event handler for when the map is ready to access, and adds controls in XAML.  Note that the map `Setting` can only be set in XAML and are only used when initially loading the map. Many of these settings can be updated using other `Map` methods after the map is loaded; `SetCamera`, `SetCameraAsync`, `SetStyle`, `SetTraffic`, and `SetUserInteraction`. 

    ```xml
    <atlas:Map x:Name="MyMap" OnReady="MyMap_OnReady">
        <!-- Pass in some settings for the map to use when loading. -->
        <atlas:Map.Settings>
            <atlas:MapLoadOptions 
                Style="Night" 
                Center="-110,45"
                Zoom="5" 
                View="Auto" 
                Language="en-US" 
                ShowFeedbackLink="False" 
                ShowLogo="False"/>
        </atlas:Map.Settings>

        <!-- Optionally add some controls to the map. -->
        <atlas:Map.Controls>
            <atlasControl:StyleControl Position="TopRight" />
            <atlasControl:ZoomControl Position="TopRight" />
            <atlasControl:CompassControl Position="TopRight" />
            <atlasControl:PitchControl Position="TopRight" />
            <atlasControl:TrafficControl Position="TopRight" />
            <atlasControl:ScaleControl Position="BottomRight" />
            <atlasControl:TrafficLegendControl Position="BottomLeft" />
        </atlas:Map.Controls>
    </atlas:Map>
    ```

> [!TIP]
> If running a project in Debug mode, you can press F12 when the map is in focus to open the browser developer tools.

## Web Root

Since this library hosts the Azure Maps Web SDK inside of a web view control, there is a default folder called `map_resources` that is used as a web root. Files in this folder can be access from the web view using relative paths from within the web view. You can add this folder to your project at the following locations.

| Platform | Web Root Path | Description |
| -------- | ------------- | ----------- |
| .NET Maui | `Resources/Raw/map_resources` | Set `Build Action` of files to `MauiAsset`. |
| WPF | `map_resources` | Add this to the project root directory. Set `Build Action` of files to `Content`, and set `Copy to Output Directory` to `Copy if newer`. |
| WinUI | `Assets/map_resources` | Add a `map_resources` folder in the `Assets` folder of the project. |

You can use subfolders and paths to organize your files.

As an example, assume there is a file called `mapdata.json` in the `map_resources` folder. You can import this data into `DataSource` with the following code without having the specify the root directory of the project:

```csharp
var dataSource = new DataSource();
await dataSource.ImportDataFromUrlAsync("mapdata.json");
```

Similarly, images can be added to the maps imager sprite in the same way. For example, if there is a file called `myimage.png` in the `map_resources` folder, you can add it to the map sprite with the following code:

```csharp
MyMap.ImageSprite.AddImageFromUrl("myImageId", "myimage.png");
```

## Added Features

In addition to the core capabilities of the Azure Maps Web SDK, several modules with additional capabilities have been added and are documented in the  [API Coverage](APICoverage.md) document. In additional to these, here are a few other capabilities that have been added to the Azure Maps Native Control:

- Map background style option - Easily set the maps background style using a CSS style string with the `BackgroundStyle` property of the `MapLoadOptions` and `setStyle`.
- Map screenshot capability via `CaptureScreenshotAsync` method.
- File drag and drop support 

## Geolocation support

```xml
//<uses-permission android:name="android.permission.ACCESS_GPS" />
//<uses-permission android:name="android.permission.ACCESS_ASSISTED_GPS" />
<//uses-permission android:name="android.permission.ACCESS_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

https://learn.microsoft.com/en-us/answers/questions/1460336/maui-webview-access-geolocation
https://stackoverflow.com/questions/5329662/android-webview-geolocation

## Drag and Drop support

The Azure Maps Native Control supports dragging and dropping files on the map that are supported by the `DataSource` class (currently just GeoJSON). This functionallity works in all versions of this library with the exception of Android and iOS where no such capability exists in the underlying platform.

To leverage this you first have to set the AllowFilesDrop property of the `MapLoadOptions` to `true`. This will enable the drag and drop functionality for the map control. 

```xml
<azm:Map x:Name="map" AllowFilesDrop="True" />
```
```

Then you can add a `` event to the map control to handle the dropped files. The event will provide you with a list of the dropped files. You can then import the file into a data source.

```csharp
MyMap.OnFilesDropped += async (s, e) =>
{
    if(e is MapFilesDroppedEventArgs evt && evt.Files != null && evt.Files.Count > 0)
    {
        //Loop through the files and do something with them. In this example we will import them into a data source.
        foreach (var f in evt.Files)
        {
            //Check if the file is a supported file type.
            if (DataSource.IsSupportedFileType(f.Name, f.MimeType))
            {
                await dataSource.ImportDataFromStreamAsync(f.Stream);
            }
        }
    }
};
```

## Serialization Tips

### GeoJSON serialization

There are multiple ways to serialize and deserialize GeoJSON objects using this library. 

**Serialize GeoJSON Objects:**

The `FeatureCollection`, `Feature`, `Geometry` classes, `BoundingBox`, `Position`, and `PositionCollection` classes all have a `ToString` method that will serialize the object to a GeoJSON string. 
The `ToString` method can also take in an integer value to limit the number of decimal places in the output string (6 decimal places is approximately equal to 0.11 meters of accuracy). This is useful when you want to reduce the size of the output string/file.

The standard `JsonSerializer.Serialize` method can also be used to convert a GeoJSON object into a `string`. If any of these GeoJSON classes are properties of parent class and you serialize that parent class, the JsonSerializer will automatically serialize the GeoJSON objects using the appropriate JsonConverters that are built into this library.

```csharp
//Create some GeoJSON objects.
var geometry = new PointGeometry(new Position(0, 0));
var feature = new Feature(geometry);
var featureCollection = new FeatureCollection(new Feature[] { feature });

//Use the ToString method.
var pointGeometryString = point.ToString();
var featureString = feature.ToString();
var featureCollectionString = featureCollection.ToString();

//Use the ToString method, but limit the number of decimal places.
var pointGeometryString = point.ToString(6);
var featureString = feature.ToString(6);
var featureCollectionString = featureCollection.ToString(6);

//Use the standard JsonSerializer.
var pointGeometryString = JsonSerializer.Serialize(point);
var featureString = JsonSerializer.Serialize(feature);
var featureCollectionString = JsonSerializer.Serialize(featureCollection);
```

**Deserialize GeoJSON Objects:**

The `FeatureCollection`, `Feature`, `Geometry` classes, `BoundingBox`, `Position`, and `PositionCollection` all have `Parse` and `TryParse` methods that will deserialize a GeoJSON string. 

The standard `JsonSerializer.Deserialize` method can also be used to convert a GeoJSON string into a GeoJSON object. If any of these GeoJSON classes are properties of parent class and you deserialize that parent class, the JsonSerializer will automatically deserialize the GeoJSON objects using the appropriate JsonConverters that are built into this library.

**NOTE:** The `FeatureCollection` deserialization supports any GeoJSON object type as input and will return a FeatureCollection containing the parsed GeoJSON objects. When in aren't sure what type of GeoJSON data your input is, desearilize as a `FeatureCollection`. Similarly, the `Feature` deserialization also supports any GeoJSON Geometry object type as input and will return a Feature containing the parsed Geometry object.

```csharp
//Use the Parse method.
var geometry = Geometry.Parse(pointGeometryString);
var feature = Feature.Parse(featureString);
var featureCollection = FeatureCollection.Parse(featureCollectionString);

//Use the parse method with a stream.
using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(pointGeometryString)))
{
	var featureCollection = FeatureCollection.Parse(stream);
}

//Use the standard JsonSerializer.
var geometry = JsonSerializer.Deserialize<Geometry>(pointGeometryString);
var feature = JsonSerializer.Deserialize<Feature>(featureString);
var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(featureCollectionString);
```

### GeometryCollection serialization

The Azure Maps Web SDK does not natively support GeoJSON GeometryCollection objects, however, this library includes some support for serializing and deserializing these objects.

To read a GeometryCollection from a GeoJSON object you can either deserialize it as a `FeatureCollection` or use the `FeatureCollectionConverter.Read` method. This method will return a `FeatureCollection` object. 

For Example:

```csharp
var featureCollectionObject = JsonSerializer.Deserialize<FeatureCollection>(geometryCollectionString);

var featureCollectionObject = FeatureCollectionConverter.Read(geometryCollectionString);
```

To write a GeometryCollection to a GeoJSON object you can either serialize it as a `FeatureCollection` or use the `FeatureCollectionConverter.Write` method. This method will return a `string` object.
