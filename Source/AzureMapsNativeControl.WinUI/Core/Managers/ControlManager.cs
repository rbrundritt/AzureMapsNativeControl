using AzureMapsNativeControl.Control;
using System.Collections;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's controls. Exposed through the controls property of the atlas.Map class. Cannot be instantiated by the user.
    /// </summary>
    public sealed class ControlManager : ObservableRangeCollection<IBaseControl>
    {
        private Map _map;

        /// <summary>
        /// A manager for the map control's controls. Exposed through the controls property of the atlas.Map class. Cannot be instantiated by the user.
        /// </summary>
        /// <param name="map"></param>
        public ControlManager(Map map)
        {
            _map = map;

            if (map != null)
            {
                // Add controls when the map is ready.
                map.OnReady += Map_OnReady;
            }

            this.CollectionChanged += ControlManager_CollectionChanged;
        }

        #region Private Methods

        private async void ControlManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await AddControls(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    await RemoveControls(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    await RemoveControls(e.OldItems);
                    await AddControls(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    await RemoveControls(e.OldItems);
                    break;
                default:
                    break;
            }
        }

        private async Task AddControls(IEnumerable? controls)
        {
            if (_map != null && _map._isReady && controls != null)
            {
                foreach (BaseControl s in controls)
                {
                    s._map = _map;

                    if (s.ModuleInfo != null && !_map.JsInterlop.IsModuleLoaded(s.ModuleInfo.Name))
                    {
                        await _map.JsInterlop.LoadModule(s.ModuleInfo);
                    }

                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "addControl", s);

                    if (s is OverviewMapControl omc)
                    {
                        omc.MapAttached();
                    }
                }
            }
        }

        private async Task RemoveControls(IEnumerable? controls)
        {
            if (controls != null)
            {
                foreach (BaseControl m in controls)
                {
                    if (m != null)
                    {
                        if (_map != null)
                        {
                            await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeControl", m.Id);
                        }

                        m._map = null;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the map is ready. Add controls that have been waiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Map_OnReady(object? sender, MapEventArgs e)
        {
            await AddControls(this);
        }

        #endregion
    }
}
