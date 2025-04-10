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
            this.CollectionChanged += ControlManager_CollectionChanged;
        }

        /// <summary>
        /// Asynchronously add a control to the map.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public async Task AddAsync(IBaseControl control)
        {
            var controls = new IBaseControl[] { control };

            //Silently add the control to the collection.
            this.AddRangeCore(controls);

            //Asynchronously add the control to the map.
            await AddControls(controls);
        }

        /// <summary>
        /// Asynchronously add a collection of controls to the map.
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        public async Task AddRangeAsync(IEnumerable controls)
        {
            //Silently add the control to the collection.
            foreach(IBaseControl c in controls)
            {
                this.AddRangeCore(new IBaseControl[] { c });
            }

            //Asynchronously add the control to the map.
            await AddControls(controls);
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
        internal async Task Map_OnReady()
        {
            await AddControls(this);
        }

        #endregion
    }
}
