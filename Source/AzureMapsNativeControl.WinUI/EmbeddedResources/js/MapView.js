var map, swipeMap;

//Increase workers for performance.
const halfCpuCount = Math.round(navigator.hardwareConcurrency / 2);
if (halfCpuCount > atlas.getWorkerCount()) {
    atlas.setWorkerCount(halfCpuCount);
}

//Prewarm Azure Maps resources to improve performance.
atlas.prewarm();

const protocol = new pmtiles.Protocol();
atlas.addProtocol("pmtiles", (request) => {
    return new Promise((resolve, reject) => {
        const callback = (err, data) => {
            if (err) {
                reject(err);
            } else {
                resolve({ data });
            }
        };
        protocol.tile(request, callback);
    });
});

function loadMap(id, mapOptions, configOptions) {
    MapUtils.initMap(id, mapOptions || {}, configOptions || {});
}

function loadSwipeMap(primaryMapId, secondaryMapId, options) {
    const map1 = getMapInterface(primaryMapId);
    const map2 = getMapInterface(secondaryMapId);
    swipeMap = new atlas.SwipeMap(map1.map, map2.map, options);
}

function setSwipeMapOptions(options) {
    if (swipeMap) {
        if (options.styleColor) {
            options.style = options.styleColor;
        }
        swipeMap.setOptions(options);
    }
}

function getMapInterface(id) {
    if (id === 'swipeMap') {
        return swipeMap;
    }

    return MapUtils.maps[id];
}

/**
 * A set of common utility functions for working with Azure Maps.
 */
class MapUtils
{
    /**
     * URL paths for proxying requests.
     */
    static proxyUrls = {
        /**
         * Gets the URL for a web request proxy.
         */
        get webRequest() {
            return '/proxy?operation=proxyWebRequest&url=';
        },

        /**
         * Gets the URL for an embedded resource proxy.
         */
        get embeddedResource() {
            return '/proxy?operation=embeddedResource&resourceName='
        }
    };

    /**
     * Reference to all map instances. 
     * 
     * Structure:
     * {
     *   [mapId]: MapInterface
     * }
     */
    static maps = {};
    
    /**
     * A list of all the modules that have been registered.
     * 
     * Structure: 
     * { 
     *     [moduleName] : { 
     *         scripts: [script1, script2, ...], 
     *         styles: [style1, style2, ...],
     *         isLoaded: true/false,
     *         waitingTaskIds: [taskId1, taskId2, ...]
     *     }
     * }
     */
    static mapModules = {};
    
    /**
     * Initializes the map interface if it doesn't already exist.
     * @param {string} id ID of the map (should match the ID of the map container element)
     * @returns {MapInterface} The map instance.
     */
    static initMap(id, mapOptions, configOptions) {
        if (!MapUtils.maps[id]) {
            mapOptions = Object.assign(mapOptions || {}, configOptions || {});
            MapUtils.maps[id] = new MapInterface(id, mapOptions);
        }
    }
    
    //#region Property Managment

    /**
     * Gets a property from a namespaced object.
     * @param {string} property The property to get.
     */
    static getNamespacedProperty(property) {
        const parts = property.split('.');
        let value;

        if (parts.length > 0) {
            value = window[parts[0]];

            for (let i = 1; i < parts.length; i++) {
                value = value[parts[i]];

                if (value === undefined) {
                    break;
                }
            }
        }

        return value;
    }

    /**
     * Sanitizes options object.
     * - Literal expressions of undefined are explicitly set to the value of undefined.
     * - If a tileUrl is provided and useProxy is set to true, the tileUrl is wrapped in a proxy request.
     */
    static sanitizeOptions(options) {
        Object.keys(options).forEach((key) => {
            const val = options[key];

            if (val && Array.isArray(val) && val.length == 2 &&
                val[0] === "literal" && val[1] === "undefined") {
                options[key] = undefined;
            }

            if (key === 'textOptions' || key === 'iconOptions') {
                MapUtils.sanitizeOptions(val);
            }

            //Check to see if the url needs to be wrapped in a proxy request.
            if (val && key === 'url' && options.useProxy && !val.startsWith(MapUtils.proxyUrls.webRequest) && !val.startsWith('data:')) {
                let encodedUrl = encodeURIComponent(val);

                //Need to decode placeholders for the tile requests.
                encodedUrl = encodedUrl.replace(/%7B/g, '{').replace(/%7D/g, '}');

                options[key] = MapUtils.proxyUrls.webRequest + encodedUrl;
            }
        });
    }

    /**
     * Sanitizes shapes and features features into a format that can be serialized.
     * @param {Array} shapeFeatures Array containing shapes and features.
     */
    static sanitizeShapefeatures(shapeFeatures) {

        const features = [];
        const shapeIds = [];

        for (let i = 0; i < shapeFeatures.length; i++) {
            const shape = shapeFeatures[i];

            if (shape instanceof atlas.Shape) {
                const f = shape.toJson();

                //Add internal info.
                if (shape.dataSource) {
                    f.source = shape.dataSource.id;
                }

                shapeIds.push(shape.getId());
                features.push(f);
            } else if (shape.type && shape.type === 'Feature') {
                features.push(shape);
            }
        }

        return { shapeIds: shapeIds, features: features };
    }

    /** Generates a unique GUID. */
    static uuid() {
        //@ts-ignore
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }

    //#endregion

    //#region Event Management

    /**
     * Notifies .NET code that an async callback in JavaScript has completed.
     * @param {string} taskId The task id of the async operation.
     * @param {any} result The result of the async operation.
     */
    static triggerAsyncCallback(taskId, result) {
        //Make sure the result is a string.
        if (result) {
            if (typeof (result) !== 'string') {
                result = JSON.stringify(result);
            }
        } else {
            result = '';
        }

        HybridWebView.SendInvokeMessageToDotNet('AsyncTaskCompleted', [taskId, result]);
    }

    /**
     * Notifies .NET code that an event has occurred.
     * @param {any} eventArgs The event arguments.
     * @param {string} targetId The id of the target object that triggered the event.
     * @param {string} targetType The type of the target object that triggered the event
     */
    static triggerEvent(eventArgs, targetId, targetType) {
        //Convert shape data into a format that can be serialized.
        if (eventArgs) {

            //Special case for keyboard events.
            if (eventArgs instanceof KeyboardEvent) {
                eventArgs = {
                    type: eventArgs.type,
                    mapId: eventArgs.mapId,
                    altKey: eventArgs.altKey,
                    ctrlKey: eventArgs.ctrlKey,
                    shiftKey: eventArgs.shiftKey,
                    keyCode: eventArgs.code,
                    key: eventArgs.key
                };
            } else {
                if (eventArgs.shapes) {
                    const sanitizedShapes = MapUtils.sanitizeShapefeatures(eventArgs.shapes);
                    eventArgs.shapeIds = sanitizedShapes.shapeIds;
                    eventArgs.features = sanitizedShapes.features;

                    //Remove the shapes from the event args.
                    delete eventArgs.shapes;
                }

                //Capture shape change event info.
                if (eventArgs.shapechanged) {
                    eventArgs.shapeIds = [eventArgs.shapechanged.getId()];
                    delete eventArgs.shapechanged;
                }

                const orgEvent = eventArgs.originalEvent;

                //Handle key events.
                if (orgEvent) {
                    eventArgs.altKey = orgEvent.altKey;
                    eventArgs.ctrlKey = orgEvent.ctrlKey;
                    eventArgs.shiftKey = orgEvent.shiftKey;
                    eventArgs.code = orgEvent.code;
                    eventArgs.key = orgEvent.key;
                }

                delete eventArgs.preventDefault;
                delete eventArgs.map;
                delete eventArgs.originalEvent;
            }

            if (targetType) {
                if (targetType === 'atlas.HtmlMarker') {
                    eventArgs.markerId = targetId;

                    if (eventArgs.target instanceof atlas.HtmlMarker) {
                        eventArgs.position = eventArgs.target.getOptions().position;
                    }
                } else if (targetType === 'atlas.Popup') {
                    eventArgs.popupId = targetId;

                    if (eventArgs.target instanceof atlas.Popup) {
                        eventArgs.position = eventArgs.target.getOptions().position;
                    }
                } else if (targetType === 'atlas.drawing.DrawingManager') {
                    eventArgs.drawingManagerId = targetId;
                } else if (targetType.startsWith('atlas.animations.')) {
                    eventArgs.animationId = targetId;
                    delete eventArgs.animation;
                }
            }
        }

        if (eventArgs.target) {
            delete eventArgs.target;
        }

        const eventArgsStr = JSON.stringify(eventArgs);
        HybridWebView.SendInvokeMessageToDotNet('EventTriggered', eventArgsStr);
    }

    //#endregion

    //#region Module Management

    /**
     * Creates an async promise that loads a JavaScript or CSS resource.
     * @param {any} url The URL to the resource.
     * @param {any} type The type of resource. Either 'script' or 'style'. Defaults to 'script'.
     * @returns {Promise} A promise that is resolved when the resource is loaded.
     */
    static async loadResource(url, type) {
        return new Promise((resolve, reject) => {
            if (type === 'style') {
                const link = document.createElement('link');
                link.onload = resolve;
                link.onerror = reject;
                link.rel = 'stylesheet';
                link.href = url;
                document.head.appendChild(link);
            } else {
                const script = document.createElement('script');
                script.onload = resolve;
                script.onerror = reject;
                script.src = url;
                document.head.appendChild(script);
            }
        });
    }

    /**
     * Adds CSS string to the document.
     * @param {any} css
     */
    static async injectCss(css) {
        const style = document.createElement('style');
        style.innerHTML = css;
        document.head.appendChild(style);
    }
    
    /**
     * Loads a module.
     * @param {string} moduleName The name of the module to load.
     * @param {string} taskId The task id to trigger when the module is loaded.
     */
    static async loadModule(moduleName, moduleScripts, moduleStyles) {
        let module = MapUtils.mapModules[moduleName];

        //Check to see if the module has been already loaded.
        if (!module) {
            //Create the module.
            module = {
                scripts: moduleScripts || [],
                styles: moduleStyles || [],
                isLoaded: false
            };

            //Register the module.
            MapUtils.mapModules[moduleName] = module;

            //Load the module.
            const tasks = [];

            if (module.scripts) {
                module.scripts.forEach(s => tasks.push(MapUtils.loadResource(s, 'script')));
            }

            if (module.styles) {
                module.styles.forEach(s => tasks.push(MapUtils.loadResource(s, 'style')));
            }

            try {
                //Load scripts and styles in parallel.
                await Promise.all(tasks);

                //Mark the module as loaded.
                module.isLoaded = true;
            } catch { }

            return false;
        }

        return false;
    }

    /**
     * Checks to see if a module is loaded.
     * @param {string} moduleName The name of the module to check.
     * @returns {boolean} True if the module is loaded, else false.
     */
    static isModuleLoaded(moduleName) {
        let module = MapUtils.mapModules[moduleName];

        if (module) {
            return module.isLoaded;
        }
        
        return false;
    }

    //#endregion
}

/**
 * An interface to Azure Maps map control instance. 
 */
class MapInterface {

    /**
     * Creates a new map interface.
     * @param {any} id The id of the map instance.
     * @param {any} mapOptions The options to create the map with.
     */
    constructor(id, mapOptions) {
        const self = this;
        self.id = id;
        self.controls = {};
        self.popups = {};
        self.markers = {};
        self.itemCache = {};
        self.animations = {};

        //{ targetId: { type: targetType, [eventName]: eventCallback } }
        self.eventCache = {};

        mapOptions = mapOptions || {};
        self.allowFileDrop = mapOptions.allowFileDrop;
        self.platform = mapOptions.platform;

        //Add token callback to auth options if needed.
        if (mapOptions.authOptions && (mapOptions.authOptions.authType === 'anonymous' || mapOptions.authOptions.authType === 'sas')) {
            mapOptions.authOptions.getToken = (resolve, reject, map) => {
                HybridWebView.SendInvokeMessageToDotNetAsync('AuthTokenRequested').then(token => resolve(token));
            };
        }

        self.map = new atlas.Map(id, mapOptions);
        const mapDiv = document.getElementById(id);

        if (mapOptions && mapOptions.backgroundStyle) {
            mapDiv.style.background = mapOptions.backgroundStyle;
        }

        this.map.events.add('ready', () => {
            //Remove fade duration for faster transitions.
            map.map._fadeDuration = 0;

            MapUtils.triggerEvent({
                type: 'ready',
                mapId: id,
                camera: self.getCamera()
            });

            //navigator.geolocation.getCurrentPosition((position) => {
            //    alert("Latitude: " + position.coords.latitude +
            //        "<br>Longitude: " + position.coords.longitude)
            //});
        });

        self.map.events.add('load', () => {
            MapUtils.triggerEvent({
                type: 'load',
                mapId: id,
                camera: self.getCamera()
            });
        });

        if (self.allowFileDrop) {
            //Add events for handling file drops.
            mapDiv.addEventListener('dragover', (e) => {
                //Stop the browser from performing its default behavior when a file is dragged and dropped.
                e.stopPropagation();
                e.preventDefault();

                e.dataTransfer.dropEffect = 'copy';
            }, false);

            mapDiv.addEventListener('drop', async (e) => {
                //Stop the browser from performing its default behavior when a file is dragged and dropped.
                e.stopPropagation();
                e.preventDefault();

                //Capture the file information and pass it to the .NET code via the event framework.
                const files = e.dataTransfer.files;
                const fileReadTasks = [];

                //Loop through and attempt to read each file. 
                for (var i = 0; i < files.length; i++) {
                    //RawMapDroppedFileInfo: { name: '', type: '', data: 'data://...'}
                    fileReadTasks.push(self.readFileAsync(files[i]));
                }

                const filesInfo = await Promise.all(fileReadTasks);
                MapUtils.triggerEvent({
                    type: 'drop',
                    mapId: id,
                    files: filesInfo
                });
            }, false);
        }

        //For debugging purposes, make a reference to the main or left map (swipe map), easily available.
        if (id === 'mainMap' || id === 'primaryMap') {
            map = self.map;
        }
    }

    readFileAsync(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = (event) => {
                resolve({
                    name: file.name,
                    type: file.type,
                    data: event.target.result
                });
            };
            reader.onerror = () => reject(reader.error);
            reader.readAsDataURL(file);
        });
    }

    //#region Map methods

    setMouseCursor(cursorStyleName) {
        this.map.getCanvasContainer().style.cursor = cursorStyleName;
    }

    disableElevation() {
        this.map.disableElevation();
    }

    enableElevation(id, elevationSource, exaggeration) {
        const self = this;
        const map = self.map;

        if (!map._isLoaded()) {
            map.events.add('load', (e) => {
                self.enableElevation(id, elevationSource, exaggeration);
            });
        } else {
            const options = elevationSource || {};
            let es = map.sources.getById(id);

            if (!es) {
                //Combine tile url and subdomains to create array of tile URLs.
                const tileUrls = [];
                if (options.subdomains && options.subdomains.length > 0) {
                    for (let i = 0; i < options.subdomains.length; i++) {
                        tileUrls.push(options.tileUrl.replace('{subdomain}', options.subdomains[i]));
                    }
                } else {
                    tileUrls.push(options.tileUrl);
                }

                options.tiles = tileUrls;    //Elevation tile source.

                //Convert min/max source zooms to min/max zoom.
                if (options.minSourceZoom !== undefined) {
                    options.minZoom = options.minSourceZoom;
                }

                if (options.maxSourceZoom) {
                    options.maxZoom = options.maxSourceZoom;
                }

                es = new atlas.source.ElevationTileSource(id, options);
                map.sources.add(es);
            }

            map.enableElevation(es, exaggeration);
        }
    }

    getBounds() {
        return this.map.getCamera().bounds;
    }

    getCamera() {
        return this.map.getCamera();
    }

    getStyle() {
        const s = this.map.getStyle();
        const bg = this.map.getMapContainer().style.background;
        if (bg && bg !== '' && typeof bg === 'string') {
            s.backgroundStyle = bg;
        }
        return s;
    }

    getTraffic() {
        return this.map.getTraffic();
    }

    getUserInteraction() {
        return this.map.getUserInteraction();
    }

    setCamera(options, animationOptions) {
        //Workaround for weird bug in Azure Maps.
        if (this.map.getCamera().minZoom < 1) {
            options.minZoom = 1;
        }

        options = Object.assign(options, animationOptions || {});

        this.map.setCamera(options);
    }

    setStyle(options) {
        this.map.setStyle(options);

        if (options && options.backgroundStyle) {
            document.getElementById(this.id).style.background = options.backgroundStyle;
        }
    }

    setTraffic(options) {
        this.map.setTraffic(options);
    }

    setUserInteraction(options) {
        this.map.setUserInteraction(options);
    }

    pixelsToPositions(data) {
        return { positions: this.map.pixelsToPositions(data.pixels) };
    }

    positionsToPixels(data) {
        return { pixels: this.map.positionsToPixels(data.positions) };
    }

    resize() {
        this.map.resize();
    }

    //#endregion

    //#region Image Sprite Manager

    async addToImageSprite(id, imageData) {
        try {
            //Decode encoded SVG data.
            if (imageData.toLowerCase().indexOf('%3Csvg%20') !== -1) {
                imageData = decodeURIComponent(imageData);
            }

            await this.map.imageSprite.add(id, imageData);
        } catch (e) {
            console.log(e);
        }
    }

    async addImageFromTemplate(id, templateName, color, secondaryColor, scale) {
        try {
            await this.map.imageSprite.createFromTemplate(id, templateName, color, secondaryColor, scale);
        } catch (e) {
            console.log(e)
        }
    }

    removeImageSprite(id) {
        this.map.imageSprite.remove(id);
    }

    clearImageSprite() {
        this.map.imageSprite.clear();
    }

    addImageTemplate(templateName, template, override) {
        template = decodeURIComponent(template);
        atlas.addImageTemplate(templateName, template, override);
    }

    getAllImageTemplateNames() {
        return atlas.getAllImageTemplateNames();
    }

    //#endregion

    //#region Html Marker Management

    addMarker(id, options) {
        let m = this.markers[id];

        if (m) {
            this.map.markers.remove(m)
        }

        m = new atlas.HtmlMarker(options);
        this.map.markers.add(m);
        this.markers[id] = m;
    }

    removeMarkers(ids) {
        if (ids) {
            const self = this;
            for (let i = 0; i < ids.length; i++) {
                const id = ids[i];
                const p = self.markers[id];
                if (p) {
                    self.map.markers.remove(p);
                    self.markers[id] = null;
                }
            }
        }
    }

    //#endregion

    //#region Popup Methods

    addPopup(id, options) {
        const self = this;
        let p = self.popups[id];
        if (p) {
            self.map.popups.remove(p);
        }

        if (options) {
            if (options.popupTemplate) {
                options.content = atlas.PopupTemplate.applyTemplate(options.popupTemplate.properties, options.popupTemplate)
            } else if (typeof (options.content) === 'string') {
                options.popupTemplate = undefined;
            }
        }

        p = new atlas.Popup(options);
        self.map.popups.add(p);
        self.popups[id] = p;
    }

    closeAllPopups() {
        const popups = this.popups;
        Object.keys(popups).forEach((key) => {
            popups[key].close();
        });
    }

    removePopups(ids) {
        if (ids) {
            const self = this;
            for (let i = 0; i < ids.length; i++) {
                const id = ids[i];
                const p = self.popups[id];
                if (p) {
                    self.map.popups.remove(p);
                    self.popups[id] = null;
                }
            }
        }
    }

    setPopupOptions(id, options) {
        const p = this.popups[id];
        if (p) {
            if (options) {
                if (options.popupTemplate) {
                    options.content = atlas.PopupTemplate.applyTemplate(options.popupTemplate.properties, options.popupTemplate)
                } else if (typeof (options.content) === 'string') {
                    options.popupTemplate = undefined;
                }
            }

            p.setOptions(options);
        }
    }

    openPopup(id) {
        const p = this.popups[id];
        if (p) {
            p.open();
        }
    }

    closePopup(id) {
        const p = this.popups[id];
        if (p) {
            p.close();
        }
    }

    isPopupOpen(id) {
        const p = this.popups[id];
        if (p) {
            return p.isOpen();
        }

        return false;
    }

    //#endregion

    //#region Source Manager

    getSourceRenderedShapes(sourceId, filter, sourceLayer) {
        //Use a raw event data object to simplify serialization.
        const result = {
            type: 'renderedShapes',
            shapeIds: [],
            features: []
        };

        const shapeFeatures = this.map.sources.getRenderedShapes(sourceId, filter, sourceLayer);

        if (shapeFeatures) {
            const sanitizedShapes = MapUtils.sanitizeShapefeatures(shapeFeatures);
            result.shapeIds = sanitizedShapes.shapeIds;
            result.features = sanitizedShapes.features;
        }

        return result;
    }

    getBasemapSourcesInfo(userSourceIds) {
        const sources = this.map.sources.getSources();

        const sourceInfo = [];

        for (let i = 0; i < sources.length; i++) {
            const s = sources[i];
            const id = s.getId ? s.getId() : s.id;

            if (id && id != '' && userSourceIds.indexOf(id) === -1 && s.source) {

                const info = {
                    id: id,
                    type: s.source.type
                };

                if (typeof s.source.attribution !== 'undefined') {
                    info.attribution = s.source.attribution;
                }

                //Default to world bounds
                let bounds = [-180, -85.05112878, 180, 85.05112878];

                if (typeof s.source.bounds !== 'undefined') {
                    info.bounds = s.source.bounds;
                } else if (s.source.type === 'video' || s.source.type === 'image') {
                    info.bounds = atlas.math.BoundingBox.fromPositions(s.source.coordinates)
                }

                info.bounds = bounds;

                if (typeof s.source.minzoom !== 'undefined') {
                    info.minzoom = s.source.minzoom;
                }

                if (typeof s.source.maxzoom !== 'undefined') {
                    info.maxzoom = s.source.maxzoom;
                }

                sourceInfo.push(info);
            }
        }

        return sourceInfo;
    }

    addSource(sourceType, id, options, data) {
        //Try can get the control from the namespace.
        const sourceClass = MapUtils.getNamespacedProperty(sourceType);
        const map = this.map;

        if (sourceClass) {
            options = options || {};

            //Remove the source if it already exists.
            if (map.sources.getById(id)) {
                map.sources.remove(id)
            }

            //Vector tile and Elevation sources use the TileSource class, need to modify options.
            if ((sourceType === 'atlas.source.VectorTileSource' ||
                sourceType === 'atlas.source.ElevationTileSource') &&
                options.tileUrl) {

                //Check to see if the source is PMTiles, and register if needed.
                if (options.tileUrl.startsWith('pmtiles://')) {
                    options.url = options.tileUrl;
                    delete options.tileUrl;
                    protocol.add(new pmtiles.PMTiles(options.url.replace('pmtiles://', '')));
                    options.type = 'vector';
                } else {
                    //Combine tile url and subdomains to create array of tile URLs.
                    const tileUrls = [];
                    if (options.subdomains && options.subdomains.length > 0) {
                        for (let i = 0; i < options.subdomains.length; i++) {
                            tileUrls.push(options.tileUrl.replace('{subdomain}', options.subdomains[i]));
                        }
                    } else {
                        tileUrls.push(options.tileUrl);
                    }

                    options.tileUrls = tileUrls; //Vector tile source.
                    options.tiles = tileUrls;    //Elevation tile source.
                }

                //Convert min/max source zooms to min/max zoom.
                if (options.minSourceZoom !== undefined) {
                    options.minZoom = options.minSourceZoom;
                }

                if (options.maxSourceZoom) {
                    options.maxZoom = options.maxSourceZoom;
                }
            }

            const source = new sourceClass(id, options);

            if (data && source.add) {
                source.add(data);
            }

            map.sources.add(source);
        }
    }

    removeSources(ids) {
        this.map.sources.remove(ids);
    }

    isSourceLoaded(id) {
        return this.map.sources.isSourceLoaded(id);
    }

    getFeatureState(shapeFeatureId, sourceId, sourceLayer) {
        return this.map.sources.getFeatureState(shapeFeatureId, sourceId, sourceLayer);
    }

    removeFeatureState(shapeFeatureId, sourceId, sourceLayer, stateKey) {
        this.map.sources.removeFeatureState(shapeFeatureId, sourceId, sourceLayer, stateKey);
    }

    setFeatureState(shapeFeatureId, sourceId, state, sourceLayer) {
        this.map.sources.setFeatureState(shapeFeatureId, sourceId, state, sourceLayer);
    }

    //#endregion

    //#region Data Source

    addShapes(sourceId, data) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.add) {
            ds.add(data);
        }
    }

    setShapes(sourceId, data) {
        const ds = this.map.sources.getById(sourceId);

        if (ds) {
            //Override for Gridded Data source.
            if (ds.setPoints) {
                ds.setPoints(data);
            } else if (ds.setShapes) {
                ds.setShapes(data);
            }
        }
    }

    updateShape(sourceId, feature) {
        const ds = this.map.sources.getById(sourceId);
        let handled = false;

        if (ds) {
            if (feature.id) {
                if (ds.getPoints) {
                    //Support for Gridded Data Source.
                    const points = ds.getPoints();

                    if (points.features) {

                        //Loop through each point and try to find a matching shape to update.
                        for (let i = 0; i < points.features.length; i++) {
                            const f = points.features[i];

                            if (f.id === feature.id) {
                                //Update the data in the shape.
                                f.properties = feature.properties;
                                f.geometry = feature.geometry;

                                handled = true;
                            }
                        }

                        if (handled) {
                            //Update the data source with the new points.
                            ds.setPoints(points);
                        }
                    }
                } else if (ds.getShapeById) {
                    const shape = ds.getShapeById(feature.id);
                    if (shape) {
                        //For performance, editting the Shape object directly. This may break in the future. So this falls back to the slower method.
                        if (shape.data && ds._updateSource) {

                            feature.properties = feature.properties || {};
                            feature.properties._azureMapsShapeId = feature.id;

                            //Update the data in the shape.
                            shape.data = feature;

                            if (shape._handleCircle) {
                                shape._handleCircle();
                            }

                            //Trigger the data source to refresh the shape.
                            ds._updateSource();

                            //Invoke the shape changed event.
                            if (shape._invokeEvent) {
                                shape._invokeEvent("shapechanged", shape);
                            }

                            handled = true;
                        }
                        else {
                            //Check to see if the shape geometry has changed.
                            if (shape.getType() !== feature.geometry.coordinates) {
                                //WORKAROUND: Need to override the geometry type of the shape object.
                                shape['data'].geometry.type = g.type;
                            }

                            shape.setProperties(feature.properties);
                            shape.setCoordinates(feature.geometry.coordinates);

                            handled = true;
                        }
                    }
                }
            }
        }

        if (!handled) {
            //No match. Add feature to the data source.
            ds.add(feature);
        }
    }

    updateShapes(sourceId, features) {
        const ds = this.map.sources.getById(sourceId);

        if (ds) {
            //Monitor first method for success.
            let success = false;

            const noMatchingShapes = [];

            if (ds.getPoints) {
                //Support for Gridded Data Source.
                const points = ds.getPoints();

                if (points.features) {

                    for (let j = 0; j < features.length; j++) {
                        const f1 = features[j];

                        success = false;

                        //Loop through each point and try to find a matching shape to update.
                        for (let i = 0; i < points.features.length; i++) {
                            const f = points.features[i];

                            if (f.id === f1.id) {
                                //Update the data in the shape.
                                f.properties = f1.properties;
                                f.geometry = f1.geometry;
                                success = true;
                                break;
                            }
                        }

                        if (!success) {
                            noMatchingShapes.push(f1);
                        }
                    }

                    if (noMatchingShapes.length > 0) {
                        points.features.push(...noMatchingShapes);
                    }

                    if (handled) {
                        //Update the data source with the new points.
                        ds.setPoints(points);
                    }
                }

                success = true;
            }
            //For performance, try an undocumented way of updating the shapes in the data source without triggering a refresh.
            //Fallback to traditional method if it fails.
            else if (ds.getShapeById && ds._updateSource && ds._addNoUpdate) {

                //Loop through each feature, and try and find a matching shape to update.
                for (let i = 0; i < features.length; i++) {
                    const f = features[i];
                    if (f.Id) {
                        const shape = ds.getShapeById(f.Id);

                        f.properties = f.properties || {};
                        f.properties._azureMapsShapeId = feature.id;

                        //Update the data in the shape.
                        shape.data = f;

                        if (shape._handleCircle) {
                            shape._handleCircle();
                        }

                        //Invoke the shape changed event.
                        if (shape._invokeEvent) {
                            shape._invokeEvent("shapechanged", shape);
                        }
                    } else {
                        noMatchingShapes.push(f);
                    }
                }

                if (noMatchingShapes.length > 0) {
                    ds._addNoUpdate(noMatchingShapes);
                }

                //Trigger the data source to refresh the shape.
                ds._updateSource();

                success = true;
            }

            if (!success && ds.ToJson && ds.setShapes) {
                //When updating a lot of features, it is better to convert the data source to a
                //feature collection, modify that, then use setShapes.


                //Convert the data source to a feature collection.
                const fc = ds.toJson().features;

                //For performance, create a lookup map of Ids to array indicies.
                const idLookup = {};

                for (let i = 0; i < fc.length; i++) {
                    idLookup[fc[i].id] = i;
                }

                //Loop over each feature and update the matching shape.
                for (let i = 0; i < features.length; i++) {
                    const feature = features[i];
                    let found = false;

                    //Make sure the feature has an ID.
                    if (feature.id) {
                        const featureIdx = idLookup[f.Id];

                        found = featureIdx != undefined;

                        //If found, update the feature.
                        if (found) {
                            const f = fc[featureIdx];
                            f.properties = feature.properties;
                            f.geometry = feature.geometry;
                        }
                    }

                    //No matching shape. Add it to the collection.
                    if (!found) {
                        fc.push(feature);
                    }
                }

                //Update the data source.
                ds.setShapes(fc);
            }
        }
    }

    updateShapeProperties(sourceId, featureId, properties, mergeProperties) {
        const ds = this.map.sources.getById(sourceId);

        if (ds) {
            if (ds.getPoints) {
                //Support for Gridded Data Source.
                const points = ds.getPoints();

                if (points.features) {

                    //Loop through each point and try to find a matching shape to update.
                    for (let i = 0; i < points.features.length; i++) {
                        const f = points.features[i];

                        if (f.id === feature.id) {
                            //Update the data in the shape.
                            f.properties = feature.properties;
                        }
                    }

                    if (handled) {
                        //Update the data source with the new points.
                        ds.setPoints(points);
                    }
                }
            } else if (ds.getShapeById) {
                const s = ds.getShapeById(featureId);

                if (s) {
                    if (mergeProperties) {
                        //Merge properties.
                        properties = Object.assign({}, s.getProperties(), properties);
                    }
                    s.setProperties(properties);
                }
            }
        }
    }

    removeShapesById(sourceId, shapeIds) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.removeById) {
            ds.removeById(shapeIds);
        }
    }

    //#region Gridded Data Source

    getGridCells(sourceId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.getCells) {
            return ds.getCells();
        }

        return null;
    }

    getCellChildren(sourceId, cellId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.getCells) {
            return ds.getCellChildren();
        }

        return null;
    }

    //#endregion

    //async importDataFromUrl(sourceId, url, returnData, taskId) {
    //    const ds = this.map.sources.getById(sourceId);
    //    let newFeatures = null;
    //    try {
    //        if (ds && ds.importDataFromUrl) {
    //            //Get the number of shapes already in the data source. This will be the index of the first shape added.
    //            const lastIdx = ds.getShapes().length;

    //            if (returnData) {
    //                await ds.importDataFromUrl(url);

    //                if (returnData) {
    //                    const newShapes = ds.getShapes().slice(lastIdx);

    //                    if (newShapes.length > 0) {
    //                        const sanitizeData = MapUtils.sanitizeShapefeatures(newShapes);

    //                        //Get the new shapes that were added as a feature collection.
    //                        newFeatures = new atlas.data.FeatureCollection(sanitizeData.features);
    //                    }
    //                }
    //            } else {
    //                ds.importDataFromUrl(url);
    //            }
    //        }
    //    } catch (e) {
    //        console.log(e);
    //    }

    //    MapUtils.triggerAsyncCallback(taskId, newFeatures);
    //}

    clearDataSource(sourceId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.clear) {
            ds.clear();
        }
    }

    setDataSourceOptions(sourceId, options) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.setOptions) {
            ds.setOptions(options);
        }
    }

    async getClusterChildren(sourceId, clusterId) {
        const ds = this.map.sources.getById(sourceId);
        let r;
        try {
            if (ds && ds.getClusterChildren) {
                r = await ds.getClusterChildren(clusterId);
            }
        } catch (e) {
            console.log(e)
        }

        return r;
    }

    async getClusterExpansionZoom(sourceId, clusterId) {
        const ds = this.map.sources.getById(sourceId);
        let r;

        try {
            if (ds && ds.getClusterExpansionZoom) {
                r = await ds.getClusterExpansionZoom(clusterId);
            }
        } catch (e) {
            console.log(e)
        }

        return r;
    }

    async getClusterLeaves(sourceId, clusterId, limit, offset) {
        const ds = this.map.sources.getById(sourceId);
        let r;

        try {
            if (ds && ds.getClusterLeaves) {
                r = await ds.getClusterLeaves(clusterId, limit, offset);
            }
        } catch (e) {
            console.log(e)
        }

        return r;
    }

    //#endregion

    //#region Data Source Lite

    getDataSourceBoundingBox(sourceId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds instanceof atlas.source.DataSource) {
            return atlas.data.BoundingBox.fromData(ds.toJson().features);
        }

        return null;
    }

    getDataSourceSize(sourceId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.shapes) {
            return datasource.shapes.length;
        }

        return 0;
    }

    getDataSourceFeatures(sourceId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.getShapes) {
            return MapUtils.sanitizeShapefeatures(ds.getShapes()).features;
        }

        return [];
    }

    getDataSourceFeatureAt(sourceId, index) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.getShapes) {
            var s = ds.getShapes();
            index = index || 0;

            if (index < s.length) {
                return MapUtils.sanitizeShapefeatures([s[index]]).features[0];
            }
        }

        return null;
    }

    getDataSourceFeature(sourceId, featureId) {
        const ds = this.map.sources.getById(sourceId);

        if (ds && ds.getShapeById) {
            const s = ds.getShapeById(featureId);
            if (s) {
                return MapUtils.sanitizeShapefeatures([s]).features[0];
            }
        }

        return null;
    }

    tryGetShape(sourceId, featureId) {
        if (featureId) {
            //Try getting source by ID.
            if (sourceId) {
                const ds = this.map.sources.getById(sourceId);
                if (ds && ds.getShapeById) {
                    return ds.getShapeById(featureId);
                }
            } else {
                //Try finding the source for the feature.
                const sources = this.map.sources.getSources();
                for (let i = 0; i < sources.length; i++) {
                    const s = sources[i];
                    if (s && s.getShapeById) {
                        return s.getShapeById(featureId);
                    }
                }
            }
        }

        return null;
    }

    //#endregion

    //#region Layer Management

    setInternalLayerZoomRange(layerId, minZoom, maxZoom) {
        this.map._getMap().setLayerZoomRange(layerId, minZoom, maxZoom);
    }

    getLayerRenderedShapes(positionOrBounds, layerIds, expression) {
        //Use a raw event data object to simplify serialization.
        const result = {
            type: 'renderedShapes',
            shapeIds: [],
            features: []
        };

        const shapeFeatures = this.map.layers.getRenderedShapes(positionOrBounds, layerIds, expression);

        if (shapeFeatures) {
            const ss = MapUtils.sanitizeShapefeatures(shapeFeatures);
            result.shapeIds = ss.shapeIds;
            result.features = ss.features;
        }

        return result;
    }

    clearStrokeDashArray(id) {
        const l = this.map.layers.getLayerById(id);

        if (l && l.setOptions) {
            l.setOptions({ strokeDashArray: undefined });
        }
    }

    addLayers(layersInfo) {
        //Loop through layers info and create an array of layer objects.
        const layerGroups = {
            _default: [],
        };

        const mapLayers = this.map.layers;

        for (let i = 0; i < layersInfo.length; i++) {
            const layerInfo = layersInfo[i];

            //Remove the layer if it already exists.
            if (mapLayers.getLayerById(layerInfo.id)) {
                mapLayers.remove(layerInfo.id);
            }

            //Try can get the layer from the namespace.
            const layerClass = MapUtils.getNamespacedProperty(layerInfo.jsNamespace);

            if (layerClass) {
                let l;

                if (layerInfo.options) {
                    MapUtils.sanitizeOptions(layerInfo.options);
                }

                if (layerInfo.jsNamespace === 'atlas.layer.AnimatedTileLayer') {
                    const o = Object.assign({ visible: true }, layerInfo.options || {});
                    let opt = Object.assign({
                        tileLayerOptions: [],
                        visible: true
                    }, layerInfo.animationOptions || {});

                    if (layerInfo.sources) {
                        for (let i = 0; i < layerInfo.sources.length; i++) {
                            opt.tileLayerOptions.push(Object.assign({}, o, layerInfo.sources[i]));
                        }
                    }

                    //Need to override the id of the layer as no option to set this when creating the layer.
                    //Need to register the layer as an animation as well.
                    l = new layerClass(opt);
                    l.id = layerInfo.id;
                    this.animations[l.id] = l.getPlayableAnimation();
                } else if (layerClass === atlas.layer.TileLayer ||
                    layerClass === atlas.layer.ImageLayer) {

                    //Copy source information into options.
                    if (layerInfo.source && layerInfo.source.id) {
                        Object.assign(layerInfo.options, layerInfo.source);
                    }

                    //Tile layer and image layers defined their source in their options. Also reverse the order of the arguments.
                    l = new layerClass(layerInfo.options, layerInfo.id);
                } else if (layerInfo.source && layerInfo.source.id) {
                    l = new layerClass(layerInfo.source.id, layerInfo.id, layerInfo.options);
                }

                if (l) {
                    const groupName = layerInfo.beforeLayerId || '_default';

                    if (!layerGroups[groupName]) {
                        layerGroups[groupName] = [];
                    }

                    layerGroups[groupName].push(l);
                }
            }
        }

        Object.keys(layerGroups).forEach(key => {
            const value = layerGroups[key];
            if (key === '_default') {
                if (value.length > 0) {
                    try {
                        //Append the layers to the this.map.
                        mapLayers.add(value);
                    } catch (e) {
                        console.log("Error adding layers: " + e);
                    }
                }
            } else {
                try {
                    mapLayers.add(value, key);
                } catch (e) {
                    console.log("Error adding layers: " + e);
                }
            }
        });
    }

    setLayerOptions(id, options) {
        if (options) {
            const l = this.map.layers.getLayerById(id);

            if (l && l.setOptions) {
                MapUtils.sanitizeOptions(options);
                l.setOptions(options);
            }
        }
    }

    setAnimatedTileLayerOptions(id, sources, options) {
        const l = this.map.layers.getLayerById(id);

        if (l && l.setOptions) {
            options = options || {};

            //Do not allow fade duration or visble to be changed in individual layers.
            options.fadeDuration = 0;
            options.visible = true;

            //Only update the media layer options if no sources specified.
            if (!sources) {
                //Don't let user set opacity as this is handled by the layer.
                if (typeof options.opacity === 'number') {
                    delete options.opacity;
                }

                l._tileLayers.setOptions(options);
            } else {
                const opt = {
                    tileLayerOptions: [],
                    visible: typeof options.visible === 'boolean' ? options.visible: l.getOptions().visible
                };

                //Loop through each source and add the options to the tile layer.
                for (let i = 0; i < sources.length; i++) {
                    opt.tileLayerOptions.push(Object.assign({}, options, sources[i]));
                }
                l.setOptions(opt);
            }
        }
    }

    moveLayer(id, beforeLayerId) {
        this.map.layers.move(id, beforeLayerId);
    }

    removeLayers(layerIds) {
        //Remove all the layers at one time.
        this.map.layers.remove(layerIds);
    }

    getImageLayerPixels(id, positions) {
        const l = this.map.layers.getLayerById(id);

        if (l && l.getPixels) {
            return l.getPixels(positions);
        }

        return null;
    }

    getImageLayerPositions(id, pixels) {
        const l = this.map.layers.getLayerById(id);

        if (l && l.getPositions) {
            return l.getPositions(pixels);
        }

        return null;
    }

    //Basemap layer method.
    getBasemapLayersInfo(userLayerIds) {
        const layers = this.map.layers.getLayers();

        const layerIds = [];

        for (let i = 0; i < layers.length; i++) {
            const l = layers[i];
            let id = l.getId ? l.getId() : l.id;

            if (id && id != '') {
                //Check to see if this is a layer collection.
                if (l.layers) {
                    for (let j = 0; j < l.layers.length; j++) {
                        const subLayer = l.layers[j];
                        id = subLayer.getId ? subLayer.getId() : subLayer.id;

                        if (id && id != '' && userLayerIds.indexOf(id) === -1) {
                            layerIds.push({
                                id: id,
                                type: subLayer.type,
                                sourceId: subLayer.source,
                                sourceLayer: subLayer['source-layer'],
                                visible: subLayer.layout ? subLayer.layout.visibility === 'visible' : true,
                                minzoom: subLayer.minzoom,
                                maxzoom: subLayer.maxzoom
                            });
                        }
                    }
                } else if (userLayerIds.indexOf(id) === -1) {
                    layerIds.push({
                        id: id,
                        type: l.type,
                        sourceId: l.source,
                        sourceLayer: l['source-layer'],
                        visible: l.layout ? l.layout.visibility === 'visible' : true,
                        minzoom: l.minzoom,
                        maxzoom: l.maxzoom
                    });
                }
            }
        }

        return layerIds;
    }

    //Basemap layer method.
    setLayerFilter(id, filter) {
        const l = this.map.layers.getLayerById(id);

        if (l) {
            if (l.setOptions) {
                l.setOptions({
                    filter: filter
                });
            } else if (l.setFilter) {
                l.setFilter(filter);
            }
        }
    }

    //Basemap layer method.
    setLayoutProperty(id, property, value) {
        this.map._getMap().setLayoutProperty(id, property, value);
    }

    //Basemap layer method.
    setPaintProperty(id, property, value) {
        this.map._getMap().setPaintProperty(id, property, value);
    }

    //#endregion

    //#region Control manager

    setControlOptions(options) {
        if (options && options.id) {
            const controls = this.controls;
            let control = controls[options.id];

            if (control) {

                if (options.childControl && options.childControl.id && options.childControlName) {
                    //If there is a child control, try and get it's instance.
                    const childControl = this.controls[options.childControl.id];
                    if (childControl) {
                        options[options.childControlName] = childControl;
                    }
                }

                //Check to see if the options has a property other than position and id.
                const hasOptions = (Object.keys(options).length - 1) > (options.position ? 1 : 0);

                //Try and get the position from the options of the control.
                let originalPosition = (control.getOptions ? control.getOptions() : (control.options || control._options || {})).position;

                //If there is no position property in the options, try and determine the position from the control container (most Azure Maps controls have a _container property).
                if (!originalPosition && control._container) {
                    const controlPositions = ['non-fixed', 'top-left', 'top-right', 'bottom-right', 'bottom-left'];
                    //The _container property is the HTML element that was insterted into the map.
                    //Check the control containers.
                    this.map.controls.controlContainer.childNodes.forEach(c => {
                        if (c.contains(control._container)) {
                            c.classList.forEach(className => {
                                if (controlPositions.indexOf(classList) > -1) {
                                    originalPosition = className;
                                }
                            });
                        }
                    });
                }

                //Check to see if the control has a setOptions method.
                //If the control does not have a setOptions method, don't update it.
                if (control.setOptions && hasOptions) {

                    //If there is a style color property, we need to set this as the style property.
                    //In .NET we have an Enum for the style property and a string property for CSS colors, but in JS it is single string property.
                    if (options.styleColor) {
                        options.style = options.styleColor;
                    }

                    control.setOptions(options);
                }

                //Check to see if the position has changed.
                if (options.position && (!originalPosition || options.position != originalPosition)) {
                    //Update the controls position by removing and then re-adding it.
                    map.controls.remove(control);
                    map.controls.add(control);
                }
            }
        }
    }

    addControl(controlInfo) {
        let control = this.controls[controlInfo.id];

        if (controlInfo.childControl && controlInfo.childControl.id && controlInfo.childControlName) {
            //If there is a child control, try and get it's instance.
            const childControl = this.controls[controlInfo.childControl.id];
            if (childControl) {
                controlInfo[controlInfo.childControlName] = childControl;
            }
        }

        //If control already exists, update it's options.
        if (control) {
            setControlOptions(controlInfo);
        } else {
            //Try can get the control from the namespace.
            const controlClass = MapUtils.getNamespacedProperty(controlInfo.jsNamespace);

            if (controlClass) {
                control = new controlClass(controlInfo);

                if (control) {
                    this.map.controls.add(control, {
                        position: controlInfo.position
                    });

                    this.controls[controlInfo.id] = control;
                }
            }
        }
    }

    removeControl(id) {
        const control = this.controls[id];
        if (control) {
            this.map.controls.remove(control);
            this.controls[id] = null;
        }
    }

    removeAllControls() {
        const c = this.controls;
        Object.keys(c).forEach((key) => {
            this.map.controls.remove(c[key]);
        });

        this.controls = {};
    }

    //#endregion

    //#region Drawing Module

    updateDrawingManager(options) {
        if (options.id && atlas.drawing.DrawingManager) {
            const self = this;
            let dm = self.itemCache[options.id];

            //If it already exists we will just update it's options.
            if (!dm) {
                options.toolbar = new atlas.control.DrawingToolbar(options.toolbarOptions || { visible: false });

                dm = new atlas.drawing.DrawingManager(self.map, options);

                //Need to add the custom source after creating the drawing manager due to a bug in the DrawingManager.
                if (options.sourceId) {
                    //WORKAROUND: setOptions doesn't work for the source property. This is a big hack.
                    //dm.setOptions({ source: self.map.sources.getById(options.sourceId) });

                    const newSource = self.map.sources.getById(options.sourceId);

                    if (newSource) {
                        //Get reference to the current source so we can remove it later.
                        const dmSource = dm.getSource();

                        dm.source = newSource;

                        if (dm.getOptions().mode !== 'idle') {
                            dm.setOptions({ mode: 'idle' });
                        }

                        dm._setLayerSources();

                        dm.options.source = newSource;
                        dm.editHelper.setOptions({ source: newSource });
                        dm.inputHelper.setOptions({ source: newSource });
                        dm.drawingHelper.setOptions({ source: newSource });
                        dm.inputHelper.setOptions({ source: newSource });

                        self.map.sources.remove(dmSource);
                    }
                }

                if (dm) {
                    self.itemCache[options.id] = dm;
                }
            } else {
                if (options.toolbarOptions) {
                    dm.getOptions().toolbar.setOptions(options.toolbarOptions);
                }

                dm.setOptions(options);
            }

            //Update layer styles.
            if (dm) {
                const layers = dm.getLayers();
                const previewLayers = dm.getPreviewLayers();

                if (options.pointLayerOptions) {
                    let defaultImage = dm.editHelper.defaultPointStyle;

                    if (options.pointLayerOptions.image) {
                        dm.editHelper.defaultPointStyle = options.pointLayerOptions.image;
                        defaultImage = dm.editHelper.defaultPointStyle;
                    }

                    if (options.pointLayerOptions.previewImage) {
                        dm.editHelper.editedPointStyle = options.pointLayerOptions.previewImage;
                    }

                    options.pointLayerOptions.image = [
                        "case",
                        [
                            "has",
                            "_azureMapsMarker"
                        ],
                        [
                            "get",
                            "_azureMapsMarker"
                        ],
                        defaultImage
                    ];

                    layers.pointLayer.setOptions({ iconOptions: options.pointLayerOptions });

                    //Need to reset the state of the drawing manager to update the point layer.
                    var lastMode = dm.getOptions().mode;
                    dm.setOptions({ mode: 'idle' });

                    var shapes = dm.getSource().getShapes();
                    shapes.forEach(s => {
                        s.addProperty("_azureMapsMarker", defaultImage);
                    });

                    dm.setOptions({ mode: lastMode });
                }

                //Update all other layer options.
                Object.keys(options).forEach(key => {
                    if (key.endsWith('LayerOptions') && key.indexOf('Point') == -1 && options[key]) {
                        const layerName = key.replace('Preview', '').replace('Options', '');
                        const l = key.indexOf('Preview') > -1 ? previewLayers[layerName] : layers[layerName];

                        if (l) {
                            l.setOptions(options[key]);
                        }
                    }
                });
            }
        }
    }

    drawingManagerEditShape(dmId, feature) {
        if (dmId && feature) {
            const dm = this.itemCache[dmId];
            if (dm && dm.edit) {
                const id = typeof feature === 'string' ? feature : feature.id;

                //Get the shape object.
                var s = dm.getSource().getShapeById(id);
                if (s) {
                    dm.edit(s);
                }
            }
        }
    }

    updateSnapGridManager(options) {
        if (options.id && atlas.drawing.SnapGridManager) {
            const self = this;
            let dm = self.itemCache[options.id];

            //If it already exists we will just update it's options.
            if (!dm) {
                dm = new atlas.drawing.SnapGridManager(self.map, options);

                if (dm) {
                    self.itemCache[options.id] = dm;
                }
            } else {
                dm.setOptions(options);
            }

            if (dm && options.gridLayerOptions) {
                dm.getGridLayer().setOptions(options.gridLayerOptions);
            }
        }
    }

    snapGridManagerSnapPositions(id, positions, zoom) {
        if (id && positions) {
            const dm = this.itemCache[id];
            if (dm && dm.snapPositions) {
                return dm.snapPositions(positions, zoom);
            }
        }

        return null;
    }

    snapGridManagerSnapFeature(id, feature, zoom) {
        if (id && feature) {
            const dm = this.itemCache[id];
            if (dm && dm.snapFeature) {
                dm.snapFeature(feature, zoom);
                return feature;
            }
        }

        return null;
    }

    //#endregion

    //#region Animations module

    animationCommand(animationId, command, options) {
        const animation = this.animations[animationId];
        if (animation) {
            switch (command) {
                // IPlayableAnimation commands
                case 'play':
                    animation.play(options);
                    break;
                case 'stop':
                    animation.stop();
                    break;
                case 'reset':
                    animation.reset();
                    break;
                case 'dispose':
                    animation.dispose();
                    delete this.animations[animationId];
                    break;
                case 'isPlaying':
                    return animation.isPlaying();
                case 'getDuration':
                    return animation.getDuration();

                // Additional animation commands available in some animation types.
                case 'pause':
                    if (animation.pause) {
                        animation.pause();
                    }
                    break;
                case 'seek':
                    if (options && animation.seek) {
                        animation.seek(options);
                    }
                    break;
                case 'getOptions':
                    if (animation.getOptions) {
                        return animation.getOptions();
                    }
                case 'setOptions':
                    if (options && animation.setOptions) {
                        animation.setOptions(options);
                    }
                    break;
                case 'setFrameIdx':
                    if (options && animation.setFrameIdx) {
                        animation.setFrameIdx(options);
                    }
                    break;
                case 'setNumberOfFrames':
                    if (options && animation.setNumberOfFrames) {
                        animation.setNumberOfFrames(options);
                    }
                    break;
                case 'getCurrentFrameIdx':
                    if (animation.getCurrentFrameIdx) {
                        return animation.getCurrentFrameIdx();
                    }
                    return 0;
            }
        }
    }

    dropAnimation(animationId, pointFeatures, sourceId, height, options) {
        const self = this;

        const source = self.map.sources.getById(sourceId);

        if (source) {
            //Check to see if the point features are already in the source.
            const shapes = [];

            pointFeatures.forEach(f => {
                if (f.type === 'Feature') {
                    shapes.push(source.getShapeById(f.id) || f);
                } else {
                    //Add geometries. 
                    shapes.push(f);
                }
            });

            const animation = atlas.animations.drop(shapes, source, height, options);
            self.animations[animationId] = animation;
        }
    }

    dropMarkersAnimation(animationId, markerIds, height, options) {
        const self = this;

        //Get all marker instances.
        const markers = [];

        markerIds.forEach(id => {
            const m = self.markers[id];

            if (m) {
                markers.push(m);
            }
        });

        const animation = atlas.animations.dropMarkers(markers, self.map, height, options);
        self.animations[animationId] = animation;
    }

    groupAnimation(groupId, animationIds, options) {
        const self = this;

        if (animationIds && animationIds.length > 0) {
            //Get the animation objects.
            const animations = [];
            animationIds.forEach(id => {
                const animation = self.animations[id];
                if (animation) {
                    animations.push(animation);
                }
            });

            const animation = new atlas.animations.GroupAnimation(animations, options);
            self.animations[groupId] = animation;
        }
    }

    snakelineAnimation(animationId, featureId, sourceId, options, animateMap = false) {
        const self = this;

        //Try and get shape.
        const shape = self.tryGetShape(sourceId, featureId);

        if (shape) {
            options = options || {};

            if (animateMap) {
                options.map = self.map;
            }

            const animation = atlas.animations.snakeline(shape, options);
            self.animations[animationId] = animation;
        }
    }

    setCoordinatesAnimation(animationId, featureId, newPosition, sourceId, options, animateMap = false) {
        const self = this;
        let shape = null;

        if (sourceId) {
            //Try and get shape.
            shape = self.tryGetShape(sourceId, featureId);
        }

        if (!shape) {
            //Try and get HTML marker.
            shape = self.markers[featureId];
        }

        if (shape) {
            options = options || {};

            if (animateMap) {
                options.map = self.map;
            }

            const animation = atlas.animations.setCoordinates(shape, newPosition, options);
            self.animations[animationId] = animation;
        }
    }

    moveAlongPathAnimation(animationId, path, featureId, sourceId, options, animateMap = false) {
        const self = this;
        let shape = null;

        if (sourceId) {
            //Try and get shape.
            shape = self.tryGetShape(sourceId, featureId);
        }

        if (!shape) {
            //Try and get HTML marker.
            shape = self.markers[featureId];
        }

        if (shape) {
            options = options || {};

            if (animateMap) {
                options.map = self.map;
            }

            const animation = atlas.animations.moveAlongPath(path, shape, options);
            self.animations[animationId] = animation;
        }
    }

    moveAlongRouteAnimation(animationId, route, featureId, sourceId, options, animateMap = false) {
        const self = this;
        let shape = null;

        if (sourceId) {
            //Try and get shape.
            shape = self.tryGetShape(sourceId, featureId);
        }

        if (!shape) {
            //Try and get HTML marker.
            shape = self.markers[featureId];
        }

        if (shape) {
            options = options || {};

            if (animateMap) {
                options.map = self.map;
            }

            const animation = atlas.animations.moveAlongRoute(route, shape, options);
            self.animations[animationId] = animation;
        }
    }

    flowingDashedLineAnimation(animationId, layerId, options) {
        const self = this;

        const layer = self.map.layers.getLayerById(layerId);
        if (layer) {
            options = options || {};
            const animation = atlas.animations.flowingDashedLine(layer, options);
            self.animations[animationId] = animation;
        }
    }

    morphAnimation(animationId, featureId, sourceId, newGeometry, options) {
        const self = this;
        let shape = null;

        if (sourceId) {
            //Try and get shape.
            shape = self.tryGetShape(sourceId, featureId);
        }

        if (shape) {
            const animation = atlas.animations.morph(shape, newGeometry, options);
            self.animations[animationId] = animation;
        }
    }

    //#endregion

    //#region Events

    attachEvent(targetId, targetType, eventName, addOnce, preventDefault) {
        const self = this;

        //eventCache => { targetId: { type: targetType, [eventName]: eventCallback } }
        let targetEvents = self.eventCache[targetId];

        if (!targetEvents) {
            targetEvents = {
                type: targetType
            };
            self.eventCache[targetId] = targetEvents;
        }
        
        //Make sure the event isn't already attached.
        let targetEvent = targetEvents[eventName];

        if (!targetEvent) {
            //Create a new event callback.
            targetEvent = (e) => {
                e = e || { type: eventName };

                //Check to see if the event handler recieved a shape. The drawing manager does this a lot.
                if (e instanceof atlas.Shape) {
                    e = { shapes: [e], type: eventName };

                    //Try getting the drawing manager.
                    const dm = self.itemCache[targetId];
                    if (dm) {
                        e.drawingManagerId = targetId;
                        e.drawingMode = dm.getOptions().mode;
                    }
                }
                //Check to see if the event is a string and is a drawing mode of the drawing manager.
                else if (typeof e === 'string' && (e.startsWith('draw-') || e.startsWith('edit-') || e.startsWith('erase-') || e === 'idle')) {
                    const mode = e;
                    e = {
                        type: eventName,
                        drawingMode: mode,
                    };

                    //Try getting the drawing manager.
                    const dm = self.itemCache[targetId];
                    if (dm) {
                        e.drawingManagerId = targetId;
                    }
                }

                e.mapId = self.id;
                e.camera = self.getCamera();

                MapUtils.triggerEvent(e, targetId, targetType);
                
                if (preventDefault) {
                    if (e.preventDefault) {
                        e.preventDefault();
                    }

                    if (e.stopPropagation) {
                        e.stopPropagation();
                    }
                }
            };

            const target = self.getItemFromCache(targetType, targetId);

            if (target) {
                targetEvents[eventName] = targetEvent;

                if (eventName === 'drop') {
                    return;
                }

                //Check to see if the event is for the map.
                if (target instanceof atlas.Map) {
                    if (eventName.startsWith('key')) {
                        //The map doesn't currently expose any key events. Add the event to the map container div.
                        target.getMapContainer().addEventListener(eventName, targetEvent);
                    } else {
                        if (addOnce) {
                            target.events.addOnce(eventName, targetEvent);
                        } else {
                            target.events.add(eventName, targetEvent);
                        }
                    }
                } else {
                    if (addOnce) {
                        self.map.events.addOnce(eventName, target, targetEvent);
                    } else {
                        self.map.events.add(eventName, target, targetEvent);
                    }
                }
            }
        }
    }

    invokeEvent(targetId, eventName, eventData) {
        //eventCache => { targetId: { eventName: eventCallback } }
        const targetEvents = this.eventCache[targetId];

        //Make sure target has the event attached.
        if (targetEvents && targetEvents[eventName]) {
            const target = this.getItemFromCache(targetEvent.type, targetId);

            if (target) {
                eventData.map = this.map;
                eventData.target = target;

                //Check to see if the event is for the map.
                if (target instanceof atlas.Map) {
                    //invoking key events on the map not supported.
                    if (!eventName.startsWith('key')) {
                        target.events.invoke(eventName, eventData);
                    }
                } else {
                    //Handle drawing manager events.
                    if (atlas.drawing) {
                        if (eventData) {
                            const e = eventName.indexOf('mode') > 0 ? eventData.mode : eventData.feature;
                            this.map.events.invoke(eventName, target, e);
                        }
                    } else {
                        this.map.events.invoke(eventName, target, eventData);
                    }
                }
            }
        }
    }

    removeEvent(targetId, eventName) {
        const targetEvents = this.eventCache[targetId];

        //Make sure target has the event attached.
        if (targetEvents && targetEvents[eventName]) {
            const targetEvent = targetEvents[eventName];

            const target = this.getItemFromCache(targetEvents.type, targetId);

            if (target) {
                //Check to see if the event is for the map.
                if (target instanceof atlas.Map) {
                    if (eventName.startsWith('key')) {
                        //The map doesn't currently expose any key events. Add the event to the map container div.
                        target.getMapContainer().removeEventListener(eventName, targetEvent);
                    } else {
                        target.events.remove(eventName, targetEvent);
                    }
                } else {
                    this.map.events.remove(eventName, target, targetEvent);
                }
                
                targetEvents[eventName] = null;
            }
        }
    }

    removeAllEvents(targetId) {
        const self = this;
        const targetEvents = self.eventCache[targetId];

        //Make sure target has events attached.
        if (targetEvents) {
            Object.keys(targetEvents).forEach((key) => {
                if (key !== 'type') {
                    self.removeEvent(targetId, key);
                }
            });
        }
    }

    //#endregion

    //#region Utility Methods

    getItemFromCache(targetType, targetId) {
        const self = this;
        if (targetType === 'Map' || targetType === 'atlas.Map') {
            return self.map;
        } else if (targetType.startsWith('atlas.layer.')) {
            return map.layers.getLayerById(targetId);
        } else if (targetType === 'atlas.HtmlMarker') {
            return self.markers[targetId];
        } else if (targetType === 'atlas.Popup') {
            return self.popups[targetId];
        } else if (targetType.startsWith('atlas.source.')) {
            return map.sources.getById(targetId);
        } else if (targetType === 'atlas.controls.') {
            return self.controls[targetId];
        } else if (targetType.startsWith('atlas.animations.')) {
            return self.animations[targetId];
        }

        //Try the generic cache.
        return self.itemCache[targetId];
    }
        
    /**
     * Calls the function on a generic item.
     * @param {any} id The id of the item to call the function on.
     * @param {any} cacheName The name of the cache to retrieve the item instance from.
     * @param {any} functionName The name of the function to call.
     * @param {any} args Any arguments to pass to the function.
     * @returns
     */
    callGenericItemFunction(id, cacheName, functionName, args) {
        const cache = this[cacheName];

        if (cache && functionName) {
            const c = cache[id];

            if (c) {
                const parts = functionName.split('.');

                let context = c;
                let func = c;

                for (let i = 0; i < parts.length; i++) {
                    if (func) {
                        if (i > 0) {
                            context = func;
                        }
                        func = func[parts[i]];
                    }
                }

                if (func && typeof func === 'function') {
                    if (args === undefined) {
                        args = [];
                    }

                    if (!Array.isArray(args)) {
                        args = [args];
                    }

                    return func.apply(context, args);
                }
            }
        }

        return null;
    }
    
    /**
     * This is a reusable function that sets the Azure Maps platform domain,
     * signs the request, and makes use of any transformRequest set on the map.
     * Use like this: `const data = await processRequest(url);`
     */
    async makeGetRequest(url) {
        try {
            const requestParams = await this.signRequest(url);

            const response = await fetch(requestParams.url, {
                method: 'GET',
                mode: 'cors',
                headers: new Headers(requestParams.headers)
            });

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
            }

            return await response.text();
        } catch (e) {
            console.log("Error when calling makeGetRequest: " + e);
        }

        return null;
    }

    /**
     * This is a reusable function that sets the Azure Maps platform domain,
     * signs the request, and makes use of any transformRequest set on the map.
     * Use like this: `const data = await processPostRequest(url, body);`
     */
    async makePostRequest(url, body)
    {
        try {
            const requestParams = await this.signRequest(url);

            const response = await fetch(requestParams.url, {
                method: 'POST',
                mode: 'cors',
                headers: new Headers(requestParams.headers),
                body: body
            });

            if (!response.ok) {
                throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
            }

            return await response.text();
        } catch (e) {
            console.log("Error when calling makePostRequest: " + e);
        }

        return null;
    }

    /**
     * Adds Azure Maps authentication details to the request.
     * @param {any} url
     * @returns
     */
    async signRequest(url) {
        // Replace the domain placeholder to ensure the same Azure Maps is used throughout the app.
        url = url.replace('{azMapsDomain}', atlas.getDomain());

        // Get the authentication details from the map for use in the request.
        var requestParams = await this.map.authentication.signRequest({ url });

        // Add content type of body to the headers.
        requestParams.headers['Content-type'] = 'application/json; charset=UTF-8';

        // Transform the request.
        var transform = this.map.getServiceOptions().transformRequest;
        if (transform) {
            requestParams = await transform(url);
        }

        return requestParams;
    } 

    //#endregion
}

//Trigger an event just after the page is loaded.
window.onload = function () {
    HybridWebView.SendInvokeMessageToDotNet('PageLoaded');
};

//Trigger an event just before the page is unloaded.
window.onbeforeunload = function () {
    HybridWebView.SendInvokeMessageToDotNet('PageUnloaded');
};