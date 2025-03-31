using AzureMapsNativeControl;
using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AzureMapsWinUISamples.Samples.GettingStarted.MVVM
{
    /// <summary>
    /// A simple view model that binds the map camera to a set of input fields.
    /// 
    /// https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm?view=net-maui-8.0
    /// </summary>
    internal partial class MyMapViewModel : INotifyPropertyChanged
    {
        #region Private Properties

        private string _mapCenter = "0,0";
        private double _mapZoom = 0;
        private double _mapPitch = 0;
        private double _mapBearing = 0;

        private AzureMapsNativeControl.Map? _map = null;

        #endregion

        #region Constructor

        /// <summary>
        /// A simple view model that updates the map position every second.
        /// </summary>
        public MyMapViewModel()
        {
            OnMapReadyCommand = new RelayCommand((e) =>
            {
                if (e != null && e is MapEventArgs args)
                {
                    //The map is ready to work with. We can get a reference to the map control from the event args.
                    _map = args.Map;

                    //Set the our bindable properties based on the maps camera. All events send the current camera information with it.
                    if (args.Camera != null)
                    {
                        if (args.Camera.Center != null)
                        {
                            MapCenter = args.Camera.Center.ToString();
                        }
                        MapZoom = args.Camera.Zoom ?? 0;
                        MapPitch = args.Camera.Pitch ?? 0;
                        MapBearing = args.Camera.Bearing ?? 0;
                    }

                    //Add post map ready code here (add sources, layers....). 

                    //Add some controls.
                    _map.Controls.AddRange([
                        new StyleControl()
                    {
                        MapStyles = new List<MapStyle>()
                        {
                            MapStyle.Road,
                            MapStyle.GrayscaleDark,
                            MapStyle.GrayscaleLight,
                            MapStyle.Satellite,
                            MapStyle.SatelliteRoadLabels,
                        },
                        Position = ControlPosition.TopRight
                    },
                    new CompassControl()
                    {
                        Position = ControlPosition.TopRight
                    },
                    new PitchControl()
                    {
                        Position = ControlPosition.TopRight
                    }
                    ]);

                    //Add an event that monitors the map's movements.
                    _map.Events.Add("moveend", (s, e) =>
                    {
                        //Get the current camera information from the event.
                        var camera = e.Camera;

                        if (camera != null)
                        {
                            //Update our bindable properties when the map is moved.
                            if (camera.Center != null)
                            {
                                MapCenter = camera.Center.ToString();
                            }
                            MapZoom = camera.Zoom ?? 0;
                            MapPitch = camera.Pitch ?? 0;
                            MapBearing = camera.Bearing ?? 0;
                        }
                    });
                }
            });
        }

        #endregion

        #region Bindable Command

        /// <summary>
        /// Command that is triggered when the map is ready to work with.
        /// </summary>
        public ICommand OnMapReadyCommand { get; private set; }

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Bindable Map Center value.
        /// </summary>
        public string MapCenter
        {
            get => _mapCenter.ToString();
            set
            {
                if (value != _mapCenter)
                {
                    _mapCenter = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Bindable Map Zoom value.
        /// </summary>
        public double MapZoom
        {
            get => _mapZoom;
            set
            {
                if (value != _mapZoom)
                {
                    _mapZoom = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Bindable Map Pitch value.
        /// </summary>
        public double MapPitch
        {
            get => _mapPitch;
            set
            {
                if (value != _mapPitch)
                {
                    _mapPitch = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Bindable Map Bearing value.
        /// </summary>
        public double MapBearing
        {
            get => _mapBearing;
            set
            {
                if (value != _mapBearing)
                {
                    _mapBearing = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Public Methods

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            //Update the map camera.
            if (_map != null)
            {
                bool cameraChanged = true;
                var options = new CameraOptions();

                switch (name)
                {
                    case "MapCenter":
                        var p = Position.Parse(_mapCenter);

                        if (p != null)
                        {
                            //Update the map center.
                            options = new CameraOptions()
                            {
                                Center = p
                            };
                        }
                        else
                        {
                            cameraChanged = false;
                        }
                        break;
                    case "MapZoom":
                        //Update the map zoom level.
                        options = new CameraOptions()
                        {
                            Zoom = _mapZoom
                        };
                        break;
                    case "MapPitch":
                        //Update the map pitch.
                        options = new CameraOptions()
                        {
                            Pitch = _mapPitch
                        };
                        break;
                    case "MapBearing":
                        //Update the map bearing.
                        options = new CameraOptions()
                        {
                            Bearing = _mapBearing
                        };
                        break;
                    default:
                        cameraChanged = false;
                        break;
                }

                if (cameraChanged)
                {
                    //Update the map camera.
                    _map.SetCamera(options);
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;

        public RelayCommand(Action<object> executeAction)
        {
            execute = executeAction;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}

