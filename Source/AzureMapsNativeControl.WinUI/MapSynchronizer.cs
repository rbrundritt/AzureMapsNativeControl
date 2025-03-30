using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Synchronizes the camera of multiple maps.
    /// </summary>
    public class MapSynchronizer
    {
        #region Private Properties 

        private bool _isEnabled = false;
        private bool _isMoving = false;

        private IList<Map> _maps;

        private Map? _callingMap;

        #endregion

        #region Constructor

        /// <summary>
        /// Synchronizes the camera of multiple maps.
        /// The camera of the first map is used as the initial sync camera.
        /// </summary>
        /// <param name="maps">2 or more maps to synchronize</param>
        /// <exception cref="ArgumentException">Exception thrown when less than 2 maps specified.</exception>
        public MapSynchronizer(IList<Map> maps)
        {
            if (maps == null || maps.Count < 2)
            {
                throw new ArgumentException("MapSynchronizer requires at least two maps to synchronize.");
            }
           
            _maps = maps;
            WaitForMapsReady();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Enables or disables the synchronization of the maps.
        /// </summary>
        public bool IsEnabled { 
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    EnabledStateChanged();
                }
            }
        }

        #endregion

        #region Private Methods

        private async void WaitForMapsReady()
        {
            bool allReady = true;

            foreach (var map in _maps)
            {
                if (!map._isReady)
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
            {
                foreach (var map in _maps)
                {
                    map.Events.Add("movestart", Map_StartMove);
                    map.Events.Add("moveend", Map_EndMove);
                }

                IsEnabled = true;
            } else
            {
                await Task.Delay(100);
                WaitForMapsReady();
            }
        }

        private async void EnabledStateChanged()
        {
            if (_isEnabled)
            {
                foreach (var map in _maps)
                {
                    map.Events.Add("move", Map_Moving);
                    map.Events.Add("styledata", Map_Moving);
                }

                await SyncMapsAsync(_maps[0], null);
            }
            else
            {
                foreach (var map in _maps)
                {
                    map.Events.Remove("move", Map_Moving);
                    map.Events.Remove("styledata", Map_Moving);
                }
            }
        }

        private void Map_StartMove(object? sender, MapEventArgs e)
        {
            if (sender is Map callingMap && IsEnabled && (_callingMap == null || !_isMoving))
            {
                _callingMap = callingMap;
            }
        }

        private async void Map_Moving(object? sender, MapEventArgs e)
        {
            if (IsEnabled && sender is Map callingMap && e.Camera != null)
            {
                if (!_isMoving && callingMap == _callingMap)
                {
                    _isMoving = true;
                    await SyncMapsAsync(callingMap, e);
                    _isMoving = false;
                }
            }
        }

        private void Map_EndMove(object? sender, MapEventArgs e)
        {
            if (!IsEnabled || sender == null || (!_isMoving && sender as Map == _callingMap))
            {
                _callingMap = null;
            }
        }

        private async Task SyncMapsAsync(Map callingMap, MapEventArgs? e)
        {
            CameraOptions? camera = null;

            if(e != null && e.Camera != null)
            {
                camera = new CameraOptions
                {
                    Center = e.Camera.Center,
                    Zoom = e.Camera.Zoom,
                    Bearing = e.Camera.Bearing,
                    Pitch = e.Camera.Pitch
                };
            }
            else if(callingMap.Settings != null)
            {
                camera = new CameraOptions
                {
                    Center = callingMap.Settings.CenterPosition,
                    Zoom = callingMap.Settings.Zoom,
                    Bearing = callingMap.Settings.Bearing,
                    Pitch = callingMap.Settings.Pitch
                };
            }

            if (camera != null)
            {
                foreach (var map in _maps)
                {
                    if (map != callingMap)
                    {
                        await map.SetCameraAsync(camera, new CameraAnimationOptions { Type = CameraAnimationType.Jump });
                    }
                }
            }
        }

        #endregion
    }
}
