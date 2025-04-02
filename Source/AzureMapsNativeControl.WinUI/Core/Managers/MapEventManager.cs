using AzureMapsNativeControl.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

#if MAUI 
using Microsoft.Maui.ApplicationModel;
#endif

namespace AzureMapsNativeControl.Core
{
    public class MapEventManager
    {
        #region Private Methods

        private readonly Map _map;

        //Structure: Target -> EventName -> Handlers -> { AddOnce, PreventDefault } (when true, will remove after first invocation)
        private ConcurrentDictionary<IMapEventTarget, Dictionary<string, Dictionary<EventHandler<MapEventArgs>, Tuple<bool, bool>>>> _handlers = new ConcurrentDictionary<IMapEventTarget, Dictionary<string, Dictionary<EventHandler<MapEventArgs>, Tuple<bool, bool>>>>();

        #endregion

        #region Construtor

        public MapEventManager(Map map)
        {
            _map = map;
        }

        #endregion

        #region Add Event Handlers

        /// <summary>
        /// Adds an event handler to the map.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void Add(string eventName, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            AddEvent(eventName, _map, handler, false, preventDefault);
        }

        /// <summary>
        /// Adds an event handler to the map that will only be invoked once.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void AddOnce(string eventName, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            AddEvent(eventName, _map, handler, true, preventDefault);
        }

        /// <summary>
        /// Adds an event handler to a specific target.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="target">The target to attache the event to (layer, marker, popup, DrawingManager, etc).</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void Add(string eventName, IMapEventTarget target, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            AddEvent(eventName, target, handler, false, preventDefault);
        }

        /// <summary>
        /// Adds an event handler to a set of targets.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="targets">A set of targets to add the event to.</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void Add(string eventName, IList<IMapEventTarget> targets, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            foreach (var target in targets)
            {
                AddEvent(eventName, target, handler, false, preventDefault);
            }
        }

        /// <summary>
        /// Adds an event handler to a specific target that will only be invoked once.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="target">The target to attach the event to (layer, marker, popup, DrawingManager, etc).</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void AddOnce(string eventName, IMapEventTarget target, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            AddEvent(eventName, target, handler, true, preventDefault);
        }

        /// <summary>
        /// Adds an event handler to a set of targets that will only be invoked once per target.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="targets">A set of targets to add the event to.</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        public void AddOnce(string eventName, IList<IMapEventTarget> targets, EventHandler<MapEventArgs> handler, bool preventDefault = false)
        {
            foreach (var target in targets)
            {
                AddEvent(eventName, target, handler, true, preventDefault);
            }
        }

        #endregion

        #region Remove Methods

        /// <summary>
        /// Removes an event handler from the map.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="handler">The event handler to remove.</param>
        public void Remove(string eventName, EventHandler<MapEventArgs> handler)
        {
            Remove(eventName, _map, handler);
        }

        /// <summary>
        /// Removes an event handler from a specific target.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="target">The target to remove the event from (layer, marker, popup, DrawingManager, etc).</param>
        /// <param name="handler">The event handler to remove.</param>
        public async void Remove(string eventName, IMapEventTarget target, EventHandler<MapEventArgs> handler)
        {
            //All Azure Maps event names are lowercase. Standardize on this to avoid issues.
            eventName = eventName.ToLower();

            if (_handlers.ContainsKey(target) && _handlers[target].ContainsKey(eventName) && _handlers[target][eventName].ContainsKey(handler))
            {
                //Remove event handler from the list.
                _handlers[target][eventName].Remove(handler);

                //Remove the event from the JS map if there are no more handlers for it. Don't allow removing special map events.
                if (_handlers[target][eventName].Count == 0 && !IsSpecialMapEvent(target, eventName))
                {
                    //Remove the event from the JS map.
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeEvent", target.Id, eventName);
                }
            }
        }

        #endregion

        #region Invoke Methods

        /// <summary>
        /// Invokes an event on the map.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="eventArgs"></param>
        public void Invoke(string eventName, MapEventArgs eventArgs)
        {
            Invoke(eventName, _map, eventArgs);
        }

        /// <summary>
        /// Invokes an event on a specific target.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="target">The target to attach the event to (layer, marker, popup, DrawingManager, etc).</param>
        /// <param name="eventArgs"></param>
        public async void Invoke(string eventName, IMapEventTarget target, MapEventArgs eventArgs)
        {
            //All Azure Maps event names are lowercase. Standardize on this to avoid issues.
            eventName = eventName.ToLower();

            if (_handlers.ContainsKey(target) && _handlers[target].ContainsKey(eventName) && _handlers[target][eventName].Count > 0)
            {
                //Invoke the event handler in the JS Map.
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "invokeEvent", target.Id, eventName, eventArgs);
            }
        }

        #endregion

        /// <summary>
        /// Determines if the specified target has events already attached to it.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool HasEvents(IMapEventTarget target)
        {
            return _handlers.ContainsKey(target);
        }

        #region Private Methods

        /// <summary>
        /// Adds an event to the manager.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="target">The target to attache the event to (layer, marker, popup, DrawingManager, etc).</param>
        /// <param name="handler">The event handler to trigger when the event is raised.</param>
        /// <param name="addOnce">Specifies if the event should only be allowed to be invoked once.</param>
        /// <param name="preventDefault">Specifies if the event should be allowed to bubble up.</param>
        private async void AddEvent(string eventName, IMapEventTarget target, EventHandler<MapEventArgs> handler, bool addOnce = false, bool preventDefault = false)
        {
            //All Azure Maps event names are lowercase. Standardize on this to avoid issues.
            eventName = eventName.ToLower();

            if (!_handlers.ContainsKey(target))
            {
                _handlers.TryAdd(target, new Dictionary<string, Dictionary<EventHandler<MapEventArgs>, Tuple<bool, bool>>>());
            }

            if (!_handlers[target].ContainsKey(eventName))
            {
                _handlers[target].Add(eventName, new Dictionary<EventHandler<MapEventArgs>, Tuple<bool, bool>>());
            }

            var targetEventHandlers = _handlers[target][eventName];

            if (!targetEventHandlers.ContainsKey(handler))
            {
                targetEventHandlers.Add(handler, Tuple.Create(addOnce, preventDefault));

                //Add the event to the JS map if this is the first handler for the event, and it isn't a special map event that is already added by default.
                if (targetEventHandlers.Count == 1 && !IsSpecialMapEvent(target, eventName))
                {
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "attachEvent", target.Id, target.JsNamespace, eventName, addOnce, preventDefault);
                }
            }
        }

        /// <summary>
        /// Method called from JS to raise/invoke the .NET event handlers.
        /// </summary>
        /// <param name="eventArgs"></param>
        internal void EventTriggered(RawMapMsg eventArgs)
        {
            if (eventArgs != null && !string.IsNullOrWhiteSpace(eventArgs.Type))
            {
                //All Azure Maps event names are lowercase. Standardize on this to avoid issues.
                string eventName = eventArgs.Type.ToLower();

                IMapEventTarget target = _map;

                //Check to see if the event occured on a layer.
                if (!string.IsNullOrEmpty(eventArgs.LayerId))
                {
                    //Get the layer instance.
                    var layer = _map.Layers.GetById(eventArgs.LayerId);

                    //Check to see if the layer is an event enabled layer.
                    if (layer is IMapEventTarget l)
                    {
                        target = l;
                    }
                }
                else if (!string.IsNullOrEmpty(eventArgs.MarkerId))
                {
                    //Get the marker by id.
                    var marker = _map.Markers.GetById(eventArgs.MarkerId);

                    if (marker != null)
                    {
                        if (eventArgs.Position != null)
                        {
                            marker._options.Position = eventArgs.Position;
                        }
                        target = marker;
                    }
                }
                else if (!string.IsNullOrEmpty(eventArgs.PopupId))
                {
                    //Get the popup by id.
                    var popup = _map.Popups.GetById(eventArgs.PopupId);

                    if (popup != null)
                    {
                        if (eventArgs.Position != null)
                        {
                            popup._options.Position = eventArgs.Position;
                        }
                        target = popup;
                    }
                }
                else if (!string.IsNullOrEmpty(eventArgs.DrawingManagerId))
                {
                    if (_map.DrawingManagers.ContainsKey(eventArgs.DrawingManagerId))
                    {
                        var drawingManager = _map.DrawingManagers[eventArgs.DrawingManagerId];
                        if (drawingManager != null)
                        {
                            target = drawingManager;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(eventArgs.AnimationId))
                {
                    if(_map.Animations.ContainsKey(eventArgs.AnimationId))
                    {
                        var animation = _map.Animations[eventArgs.AnimationId];
                        if (animation != null)
                        {
                            target = animation;
                        }
                    }
                }

                if (target == _map && IsSpecialMapEvent(target, eventName))
                {
                    //Handle special map events in situations where the page has been refreshed/reloaded.
                    if (eventName.Equals("ready"))
                    {
                        if (_map._pageReloaded)
                        {
                            _map._isMapLoaded = false;
                            _map._pageReloaded = false;
                        }

                        _map._isReady = true;
                    }
                    else if (eventName.Equals("load"))
                    {
                        _map._isMapLoaded = true;
                    }
                }

                if (_handlers.ContainsKey(target) && _handlers[target].ContainsKey(eventName))
                {
                    var args = GetMapEventArgs(eventArgs);
                    var targetEventHandlers = _handlers[target][eventName];

                    var toRemove = new List<EventHandler<MapEventArgs>>();

#if MAUI && ANDROID
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
#endif
                        foreach (var handler in targetEventHandlers)
                        {
                            if (handler.Key != null)
                            {
                                handler.Key.Invoke(target, args);

                                //Check to see if the handler should be removed after first invocation.
                                if (handler.Value.Item1)
                                {
                                    toRemove.Add(handler.Key);
                                }
                            }
                        }

                        //Remove the handlers that should only be invoked once.
                        foreach (var handler in toRemove)
                        {
                            targetEventHandlers.Remove(handler);
                        }
#if MAUI && ANDROID
                    });
#endif

#if WINUI || WPF
                    if (target == _map)
                    {
                        if (eventName.Equals("ready") && _map.OnReadyCommand != null)
                        {
                            _map.OnReadyCommand.Execute(args);
                        }
                        else if (eventName.Equals("load") && _map.OnLoadedCommand != null)
                        {
                            _map.OnLoadedCommand.Execute(args);
                        }
                        else if (eventName.Equals("drop") && _map.OnFilesDroppedCommand != null)
                        {
                            _map.OnFilesDroppedCommand.Execute(args);
                        }
                    }
#endif
                    }
            }
        }

        private MapEventArgs GetMapEventArgs(RawMapMsg eventArgs)
        {
            switch (eventArgs.Type)
            {
                case "error":
                    return new MapErrorEventArgs(_map, eventArgs);
                case "click":
                case "contextmenu":
                case "dblclick":
                case "drag":
                case "dragstart":
                case "dragend":
                case "mousedown":
                case "mouseenter":
                case "mouseleave":
                case "mousemove":
                case "mouseout":
                case "mouseover":
                case "mouseup":
                case "wheel":
                    return new MapMouseEventArgs(_map, eventArgs);
                case "touchstart":
                case "touchend":
                case "touchmove":
                case "touchcancel":
                    return new MapTouchEventArgs(_map, eventArgs);
                case "stylechanged":
                    return new MapStyleChangedEventArgs(_map, eventArgs);
                case "keydown":
                case "keyup":
                    return new MapKeyboardEventArgs(_map, eventArgs);
                case "drawingmodechanged":
                case "drawingchanged":
                case "drawingchanging":
                case "drawingcomplete":
                case "drawingstarted":
                case "drawingerased":
                    return new DrawingManagerEventArgs(_map, eventArgs);
                case "onprogress":
                    return new PlayableAnimationEvent(_map, eventArgs);
                case "onframe":
                    return new FrameBasedAnimationEvent(_map, eventArgs);
                case "drop":
                    return new MapFilesDroppedEventArgs(_map, eventArgs);

                //case "data":
                //case "sourcedata":
                //case "styledata":
                //case "dataadded":
                //case "dataremoved":
                //case "datasourceupdated":
                //Data events are not fully implemented at this time as rarely used.
                //case "ready":
                //case "load":
                //case "boxzoomstart":
                //case "boxzoomend":
                //case "idle":
                //case "movestart":
                //case "move":
                //case "moveend":
                //case "pitchstart":
                //case "pitch":
                //case "pitchend":
                //case "render":
                //case "resize":
                //case "rotatestart":
                //case "rotate":
                //case "rotateend":
                //case "tokenacquired":
                //case "zoomstart":
                //case "zoom":
                //case "zoomend":
                //case "open":
                //case "close":
                default:
                    return new MapEventArgs(_map, eventArgs);
            }
        }

        /// <summary>
        /// Removes all the event handles for a specific target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="silentRemove"></param>
        internal async void RemoveTarget(IMapEventTarget target, bool silentRemove = false)
        {
            //If the target is a map, need to remove all events other than "ready" and "load"
            if (target is Map)
            {
                foreach (var t in _handlers)
                {
                    foreach (var eventInfo in t.Value)
                    {
                        if (t.Key != target || t.Key is not Map || !IsSpecialMapEvent(t.Key, eventInfo.Key))
                        {
                            if (!silentRemove)
                            {
                                await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeEvent", target.Id, eventInfo.Key);
                            }

                            t.Value.Clear();
                        }
                    }
                }
            }
            else if (_handlers.ContainsKey(target)) //Remove events only for the target item.
            {
                if (!silentRemove)
                {
                    //Remove the JS events.
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeAllEvents", target.Id);
                }

                _handlers[target].Clear();
            }
        }

        private bool IsSpecialMapEvent(IMapEventTarget target, string eventName)
        {
            return target is Map && (eventName.Equals("ready") || eventName.Equals("load") || eventName.Equals("drop"));
        }

        /// <summary>
        /// Readds events to a target in the map. Used when an entity (e.g. layer) has events added to it before it is added to the map.
        /// </summary>
        /// <param name="target"></param>
        internal async void ReAddEvents(IMapEventTarget target)
        {
            if (_handlers.ContainsKey(target))
            {
                foreach(var h in _handlers[target])
                {
                    string eventName = h.Key;
                    var targetEventHandlers = h.Value;

                    //Add the event to the JS map if this is the first handler for the event, and it isn't a special map event that is already added by default.
                    if (targetEventHandlers.Count == 1 && !IsSpecialMapEvent(target, eventName))
                    {
                        var options = targetEventHandlers[targetEventHandlers.Keys.First()];
                        await _map.JsInterlop.InvokeJsMethodAsync(_map, "attachEvent", target.Id, target.JsNamespace, eventName, options.Item1, options.Item2);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all events for a set of entities. Used when a set of entities are removed from the map.
        /// </summary>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        internal async Task BulkRemoveEvents(IList<string> entityIds)
        {
            foreach(var entityId in entityIds)
            {
                //try
                //{
                    foreach (var h in _handlers)
                    {
                        if (h.Key.Id.Equals(entityId))
                        {
                            //Remove the JS events.
                            await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeAllEvents", h.Key.Id);

                            h.Value.Clear();
                        }
                    }
                //}
                //catch (Exception ex) { }//In rare situations, when adding/removing multiple events rapidly, a lock might occur.
            }
        }

        #endregion
    }
}
