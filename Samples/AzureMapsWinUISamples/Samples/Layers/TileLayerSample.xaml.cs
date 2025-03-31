using Microsoft.UI.Xaml.Controls;
using AzureMapsNativeControl;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Layer;
using AzureMapsNativeControl.Source;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace AzureMapsWinUISamples.Samples
{
    public sealed partial class TileLayerSample : Page
    {
        /*********************************************************************************************************
        * This sample shows the different ways to tile layers.
        * 
        * This sample is based on these Azure Maps Web SDK samples: 
        * 
        * https://samples.azuremaps.com/?sample=tile-layer-options
        * https://samples.azuremaps.com/?sample=tile-layer-using-x%2C-y%2C-and-z
        * https://samples.azuremaps.com/?sample=wms-tile-layer
        * https://samples.azuremaps.com/?sample=wmts-tile-layer
        *********************************************************************************************************/

        #region Private Properties

        private TileLayer tileLayer;

        /// <summary>
        /// A dictionary of tile sources for the sample. 
        /// The TileSourceInfo class is a helper class in this sample app that contains a tile source and center/zoom to set the map to.
        /// </summary>
        private Dictionary<string, TileSourceInfo> tileSourceInfo = new Dictionary<string, TileSourceInfo>();

        #endregion

        #region Constructor

        public TileLayerSample()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Event handler for when the map is ready to be accessed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyMap_OnReady(object sender, MapEventArgs e)
        {
            //For this sample, since we may refresh the browser when testing/debugging, clear the cached tile sources.
            tileSourceInfo.Clear();

            //Preload the tile sources for this sample.
            await PreloadTileSources();

            //For this sample, set the initial tile source from the picker.
            TileSourcePicker.SelectedIndex = 0;
        }

        /// <summary>
        /// Preloads the tile sources for the sample.
        /// </summary>
        /// <returns></returns>
        private async Task PreloadTileSources()
        {
            /****************************************************************
             * Load XYZ_tile_layer source.
             * 
             * Based on: https://samples.azuremaps.com/?sample=tile-layer-using-x%2C-y%2C-and-z
             * Tile source from: https://openseamap.org/index.php
             *****************************************************************/

            //When creating a tile source you could set all the properties individually or use named arguments to make it easier to read.
            //The following example sets each property individually, while most other samples will use named arguments as it keeps things easier.

            tileSourceInfo.Add("XYZ_tile_layer", new TileSourceInfo(
                new TileSource(
                    "https://tiles.openseamap.org/seamark/{z}/{x}/{y}.png", //Formatted tile URL.
                    null,   //If a proxy should be used to access the tiles (helps with non-CORs enabled endpoints).
                    false,   //If the tile source servers vector tiles (used by data layers) rather than raster tiles (used by TileLayer).
                    256,    //Tile size
                    null,   //Subdomains
                    null,   //Bounds - defaults to full world bounds.
                    7,      //Min source zoom
                    17      //Max source zoom
                ),
                new Position(-122.426181, 47.608070), 11));

            /****************************************************************
             * Load XYZ_with_subdomains source.
             * 
             * Many tile servers use subdomains as a way to distribute the load across multiple servers for better performance.
             * Tile source from: https://wiki.openstreetmap.org/wiki/OpenRailwayMap
             *****************************************************************/

            tileSourceInfo.Add("XYZ_with_subdomains", new TileSourceInfo(
                new TileSource(
                    //If a tile server supports subdomains, you can put a {subdomain} placeholder in the tile URL.
                    tileUrl: "https://{subdomain}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png",

                    //You can then list the different subdomains in the subdomains property.
                    subdomains: ["a", "b", "c"],

                    tileSize: 256,
                    minSourceZoom: 7,
                    maxSourceZoom: 17
                ),
                new Position(-0.127500, 51.507220), 11));

            /****************************************************************
             * Load WMTS_tile_layer source.
             * 
             * Based on: https://samples.azuremaps.com/?sample=wmts-tile-layer
             * Tile source from: https://basemap.nationalmap.gov/arcgis/rest/services/USGSImageryOnly/MapServer
             * 
             * Original WMTS URL format: https://basemap.nationalmap.gov/arcgis/rest/services/USGSImageryOnly/MapServer/WMTS/tile/1.0.0/USGSImageryOnly/default/{TileMatrixSet}/{TileMatrix}/{TileRow}/{TileCol}
             * Note the settings; TileMatrixSet=EPSG:3857 or GoogleMapsCompatible, TileMatrix={z}, TileRow={y}, TileCol={x}
             *****************************************************************/

            tileSourceInfo.Add("WMTS_tile_layer", new TileSourceInfo(
                new TileSource(
                    tileUrl: "https://basemap.nationalmap.gov/arcgis/rest/services/USGSImageryOnly/MapServer/WMTS/tile/1.0.0/USGSImageryOnly/default/GoogleMapsCompatible/{z}/{y}/{x}",
                    tileSize: 256,
                    minSourceZoom: 0,
                    maxSourceZoom: 18
                ),
                new Position(-99.47, 40.75), 4));

            /****************************************************************
             * Load WMS_tile_layer source.
             * 
             * Based on: https://samples.azuremaps.com/?sample=wms-tile-layer
             * Tile source from: https://carto.nationalmap.gov/arcgis/rest/services/govunits/MapServer
             * 
             * Note the bounding box placeholder in the tileUrl and the CRS set to EPSG:3857
             *****************************************************************/

            tileSourceInfo.Add("WMS_tile_layer", new TileSourceInfo(
                new TileSource(
                    tileUrl: "https://carto.nationalmap.gov/arcgis/services/govunits/MapServer/WMSServer?FORMAT=image%2Fpng&HEIGHT=1024&LAYERS=1%2C2%2C3%2C4%2C5%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%2C14%2C15%2C16%2C17%2C18%2C19%2C20%2C21&REQUEST=GetMap&STYLES=default%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault%2Cdefault&TILED=true&TRANSPARENT=true&WIDTH=1024&VERSION=1.3.0&SERVICE=WMS&CRS=EPSG%3A3857&BBOX={bbox-epsg-3857}",

                    tileSize: 1024
                ),
                new Position(-122.426181, 47.60807), 7));

            /****************************************************************
             * Load Azure_Maps_Weather_Tiles source.
             * 
             * Based on: https://samples.azuremaps.com/?sample=show-weather-overlays-on-a-map
             * 
             * Note the bounding box placeholder in the tileUrl and the CRS set to EPSG:3857
             *****************************************************************/

            //By using "{azMapsDomain}" in the URL domain, the map will automatically point to the same domain as the map control and use the same authentication.
            string azureMapsWeatherTileUrl = "https://{azMapsDomain}/map/tile?api-version=2022-08-01&tilesetId={layerName}&zoom={z}&x={x}&y={y}";

            string tileUrl = azureMapsWeatherTileUrl.Replace("{layerName}", "microsoft.weather.infrared.main");
            //string tileUrl = azureMapsWeatherTileUrl.Replace("{layerName}", "microsoft.weather.radar.main"); //Alternative: radar data

            tileSourceInfo.Add("Azure_Maps_Weather_Tiles", new TileSourceInfo(
               new TileSource(tileUrl, null, false, 256, null, BoundingBox.Global(), 0, 18),
               new Position(-99.47, 40.75), 4));

            /****************************************************************
             * Load local_MBTile_file source.
             * 
             * MBTile files are SQLite databases that contain map tiles (raster or vector).
             * This makes moving and storing map tiles easier.
             *****************************************************************/

            //Try loading the MBTile file from the app package.
            //This file will be copied to the app data directory the first time this is run.
            var mbTileSource = await MBTileSource.TryLoadAsync("map_resources/data/rasterTiles/countries-raster.mbtiles");

            if (mbTileSource != null)
            {
                //Try and get the center information from the MBTile file metadata. The altitude information in the position is a zoom level.
                var viewInfo = mbTileSource.ToTileJson().Center;

                if (viewInfo == null)
                {
                    viewInfo = new Position(0, 0, 2);
                }

                tileSourceInfo.Add("local_MBTile_file", new TileSourceInfo(
                    mbTileSource,
                    viewInfo, (int)(viewInfo.Altitude ?? 2)));
            }
            else
            {
                Debug.WriteLine("Failed to load MBTile file.");
            }

            /****************************************************************
             * Load local_tile_zip_file source.
             * 
             * Loads map tiles from a zip file. In this case tiles are stored 
             * in the root directory and use a quadkey nameing convention.
             *****************************************************************/

            //You can load the file yourself using FileSystem.OpenAppPackageFileAsync then creating a ZipArchive.
            //The ZipFileTileSource class also has a TryGetZipArchive method that can also be used to load a zip file,
            //and supports both file paths and URL paths. It tries searching both in the app Raw folder and the app data directory.

            //Try and load the zip file. 
            var zipFile = await ZipFileTileSource.TryLoadAsync("map_resources/data/rasterTiles/katrina.zip");

            if (zipFile != null)
            {
                tileSourceInfo.Add("local_tile_zip_file", new TileSourceInfo(
                    new ZipFileTileSource(zipFile,
                        formattedFilePath: "{quadkey}.png",
                        tileSize: 256,
                        minSourceZoom: 1,
                        maxSourceZoom: 9,
                        bounds: new BoundingBox(-101.065, 14.01, -80.538, 35.176)
                    ),
                    new Position(-90, 25), 4));
            }
            else
            {
                Debug.WriteLine("Failed to load zip file.");
            }

            /****************************************************************
             * Load local_tile_zip_file_folders source.
             * 
             * Tile source from: https://github.com/paulo-roger/toril-leaflet/tree/main
             * 
             * This example shows how to load tiles from a zip file where the tiles 
             * organized into a folder structure: "{z}/{x}/{y}.png"
             *****************************************************************/

            //Try and load the zip file.
            var zipFile2 = await ZipFileTileSource.TryLoadAsync("map_resources/data/rasterTiles/toril-leaflet.zip");

            if (zipFile2 != null)
            {
                tileSourceInfo.Add("local_tile_zip_file_folders", new TileSourceInfo(
                    new ZipFileTileSource(zipFile2,
                        formattedFilePath: "{z}/{x}/{y}.png",
                        tileSize: 256,
                        minSourceZoom: 1,
                        maxSourceZoom: 4,
                        bounds: new BoundingBox(-180, 30, 156, 85)
                    ),
                    new Position(0, 65), 2));
            }
            else
            {
                Debug.WriteLine("Failed to load zip file.");
            }

            /****************************************************************
             * Load proxy_tile_requests source.
             *
             * Tile source from: https://github.com/hugobudd/SF_Model_Tiles
             * 
             * The Azure Maps Web SDK requires hosted assets to be on a CORs enabled endpoint. 
             * If you want to access assets that are hosted on non-CORs enabled endpoints, set the UseProxy option to true.
             *****************************************************************/

            tileSourceInfo.Add("proxy_tile_requests", new TileSourceInfo(
                new TileSource(
                    tileUrl: "https://raw.githubusercontent.com/rbrundritt/SampleMapData/main/RasterTiles/SF_Model_Tiles/{z}/{x}/{y}.png",

                    //The Azure Maps Web SDK requires hosted assets to be on a CORs enabled endpoint. 
                    //If you want to access assets that are hosted on non-CORs enabled endpoints, set the UseProxy option to true.
                    useProxy: true,

                    tileSize: 256,
                    minSourceZoom: 7,
                    maxSourceZoom: 15,
                    bounds: new BoundingBox(-122.5393, 37.68797, -122.32267, 37.82813)
                ),
                new Position(-122.457156, 37.774885), 11));
        }

        /// <summary>
        /// Event handler for when the tile source picker changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TileSourcePicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tileSourceInfo.Count == 0)
            {
                return;
            }

            var tileSourceName = Helpers.GetSelectedPickerString(sender);

            if (tileSourceInfo.ContainsKey(tileSourceName))
            {
                var sourceInfo = tileSourceInfo[tileSourceName];

                //If there isn't a tile layer already, create it and add to the map.
                if (tileLayer == null)
                {
                    tileLayer = new TileLayer(sourceInfo.Source, new MediaLayerOptions
                    {
                        FadeDuration = 0
                    });
                    MyMap.Layers.Add(tileLayer);
                }
                else
                {
                    //Update the source of the tile layer.
                    tileLayer.SetSource(sourceInfo.Source);
                }

                //Update the camera for the sample.
                MyMap.SetCamera(new CameraOptions
                {
                    Center = sourceInfo.Center,
                    Zoom = sourceInfo.Zoom
                });
            }
        }

        /// <summary>
        /// Event handler for when the opacity slider changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpacitySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (tileLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var opacity = (double)Math.Round(slider.Value, 2);

            //Set the opacity of the tile layer.
            tileLayer.SetOptions(new MediaLayerOptions
            {
                Opacity = opacity
            });

            OpacityLabel.Text = $"Opacity: {opacity}";
        }

        /// <summary>
        /// Event handler for when the hue rotation slider changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueRotationSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (tileLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var hueRotation = (double)Math.Round(slider.Value);

            //Set the hue rotation of the tile layer.
            tileLayer.SetOptions(new MediaLayerOptions
            {
                HueRotation = hueRotation
            });

            HueRotationLabel.Text = $"Hue Rotation: {hueRotation}";
        }

        /// <summary>
        /// Event handler for when the contrast slider changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContrastSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (tileLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var contrast = (double)Math.Round(slider.Value, 2);

            //Set the contrast of the tile layer.
            tileLayer.SetOptions(new MediaLayerOptions
            {
                Contrast = contrast
            });

            ContrastLabel.Text = $"Contrast: {contrast}";
        }

        /// <summary>
        /// Event handler for when the brightness slider changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaturationSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (tileLayer == null)
            {
                return;
            }

            var slider = (Slider)sender;

            var saturation = (double)Math.Round(slider.Value, 2);

            //Set the saturation of the tile layer.
            tileLayer.SetOptions(new MediaLayerOptions
            {
                Saturation = saturation
            });

            SaturationLabel.Text = $"Saturation: {saturation}";
        }

        /// <summary>
        /// Event handler for when the before layer picker changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeLayerPicker_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tileLayer == null)
            {
                return;
            }

            var beforeLayerId = Helpers.GetSelectedPickerString(sender);

            if (beforeLayerId.Equals("undefined"))
            {
                beforeLayerId = string.Empty;
            }

            //Move the tile layer before the selected layer within the map.
            //The two main built-in layer IDs are "labels" and "roads", but you can also pass in the ID of any of your layers as well.
            MyMap.Layers.Move(tileLayer, beforeLayerId);
        }

        #endregion
    }

    /// <summary>
    /// A helper class for this sample that contains the tile source and center/zoom to set the map to.
    /// </summary>
    public class TileSourceInfo
    {
        public TileSourceInfo(TileSource source, Position center, int zoom)
        {
            Source = source;
            Center = center;
            Zoom = zoom;
        }

        public TileSource Source { get; set; }

        public Position Center { get; set; }

        public int Zoom { get; set; }
    }
}
