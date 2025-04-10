using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Internal;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

#if MAUI
#elif WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#elif WPF
using System.Windows;
using System.Windows.Controls;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// A UI view that allows swiping between two overlapping maps, ideal for comparing two overlapping data sets.
    /// </summary>
    public class SwipeMap: IMapView
    {
        #region Private Properties

        /// <summary>
        /// A unique identifier for the view.
        /// </summary>
        internal string ViewId { get; private set; } = UniqueId.Get("SwipeMap");

        private bool _mapsReady = false;

        private bool _isReady = false;

        private event EventHandler? OnReadyHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// A UI view that allows swiping between two overlapping maps, ideal for comparing two overlapping data sets.
        /// </summary>
        public SwipeMap()
        {
            MapViewFileName = "SwipeMapView.html";

            //Set the target for JavaScript interop.
            JsInterlop = new MapViewJsInterlop(this);

            PrimaryMap = new Map("primaryMap", JsInterlop);
            JsInterlop._maps.Add(PrimaryMap);

            SecondaryMap = new Map("secondaryMap", JsInterlop);
            JsInterlop._maps.Add(SecondaryMap);

            //Add the web view to the map container.
            this.Children.Insert(0, JsInterlop._webView);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Event triggered when the map is ready.
        /// </summary>
        public event EventHandler OnReady
        {
            add
            {
                OnReadyHandler += value;

                //If the map is already ready, trigger the event.
                if (_isReady)
                {
                    OnReadyHandler?.Invoke(this, EventArgs.Empty);
                }
            }
            remove
            {
                OnReadyHandler -= value;
            }
        }

        /// <summary>
        /// Initial options to set on the swipe map when loading.
        /// </summary>
        public SwipeMapOptions Settings { get; set; } = new SwipeMapOptions()
        {
            Interactive = true,
            Orientation = MapOrientation.Horizontal,
            SliderPosition = null,
            Style = Control.ControlStyle.Light
        };

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map PrimaryMap { get; internal set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map SecondaryMap { get; internal set; }

        #endregion

        #region Module methods

        /// <summary>
        /// Loads a module into the map.
        /// </summary>
        /// <param name="moduleInfo"></param>
        /// <returns></returns>
        public override Task LoadModuleAsync(MapModuleInfo moduleInfo)
        {
            return JsInterlop.LoadModule(moduleInfo);
        }

        /// <summary>
        /// Checks if a module is loaded.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public override bool IsModuleLoaded(string moduleName)
        {
            return JsInterlop.IsModuleLoaded(moduleName);
        }

        #endregion

        #region Private Methods

        /// <inerhitdoc />
        internal override async Task InitView(AzureMapsConfiguration? config = null)
        {
            //Wait for both maps to be ready.
            PrimaryMap.OnReady += ChildMaps_Ready;
            SecondaryMap.OnReady += ChildMaps_Ready;

            PrimaryMap.Settings = Settings.PrimaryMapSettings;

            //Ignore any camera options on the secondary map if any camera options set on primary map.
            if (Settings.PrimaryMapSettings != null && Settings.SecondaryMapSettings != null &&
               (Settings.PrimaryMapSettings.Center != null || Settings.PrimaryMapSettings.Zoom != null || Settings.PrimaryMapSettings.Bounds != null))
            {
                Settings.SecondaryMapSettings.Center = null;
                Settings.SecondaryMapSettings.Zoom = null;
                Settings.SecondaryMapSettings.Bearing = null;
                Settings.SecondaryMapSettings.Pitch = null;
                Settings.SecondaryMapSettings.Bounds = null;
                Settings.SecondaryMapSettings.CenterOffset = null;
            }

            SecondaryMap.Settings = Settings.SecondaryMapSettings;

            //Initialize both maps.
            await Task.WhenAll([PrimaryMap.InitView(config), SecondaryMap.InitView(config)]);
        }

        private async void ChildMaps_Ready(object? sender, MapEventArgs e)
        {
            //Wait for both maps to be ready.
            if(!_mapsReady && PrimaryMap._isReady && SecondaryMap._isReady)
            {
                _mapsReady = true;

                await JsInterlop.InvokeJsMethodAsync("loadSwipeMap", PrimaryMap.Id, SecondaryMap.Id, Settings);
                
                _isReady = true;

                //Trigger the ready event.
                OnReadyHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inerhitdoc />
        internal override void WebPageUnloaded()
        {
            _mapsReady = false; 
            _isReady = false;

            PrimaryMap.WebPageUnloaded();
            SecondaryMap.WebPageUnloaded();
        }

#if MAUI
        private static async void SettingsChanged(BindableObject bindable, object oldValue, object newValue)
        {
#else
        private static async void SettingsChanged(DependencyObject bindable, DependencyPropertyChangedEventArgs e)
        {
            object oldValue = e.OldValue;
            object newValue = e.NewValue;
#endif
            if (bindable is SwipeMap swipeMap && swipeMap._isReady)
            {
                if (oldValue is SwipeMapOptions oldOptions && newValue is SwipeMapOptions options)
                {
                    if(oldOptions.Style == null && options.Style != null)
                    {
                        options.StyleColor = null;
                    }
                    else if (oldOptions.StyleColor == null && options.StyleColor != null)
                    {
                        options.Style = null;
                    }

                    await swipeMap.JsInterlop.InvokeJsMethodAsync("swipeMap.setOptions", options);
                }
            }
        }

        #endregion
    }
}
