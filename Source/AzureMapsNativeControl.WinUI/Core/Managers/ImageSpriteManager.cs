using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#if MAUI && ANDROID 
using Android.Renderscripts;
#endif

namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's image sprite. Exposed through the imageSprite property of the atlas.Map class. Cannot be instantiated by the user.
    /// </summary>
    public sealed class ImageSpriteManager
    {
        #region Private Properties

        private Map _map;

        /// <summary>
        /// Built in icon ids that are always available.
        /// </summary>
        private List<string> _builtInIconIds = new List<string>()
        {
            "marker-black",
            "marker-blue",
            "marker-darkblue",
            "marker-red",
            "marker-yellow",
            "pin-blue",
            "pin-darkblue",
            "pin-red",
            "pin-round-blue",
            "pin-round-darkblue",
            "pin-round-red",
            "none"
        };

        private Dictionary<string, string> _userAddedImageTemplates = new Dictionary<string, string>();
        private Dictionary<string, IList<object>> _userCreatedTemplateImages = new Dictionary<string, IList<object>>();
        private Dictionary<string, string> _userAddedImages = new Dictionary<string, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// A manager for the map control's image sprite. Exposed through the imageSprite property of the atlas.Map class. Cannot be instantiated by the user.
        /// </summary>
        /// <param name="map"></param>
        public ImageSpriteManager(Map map)
        {
            this._map = map;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an image to the maps image sprite for use when styling icon images as well as linestring and polygon patterns. 
        /// </summary>
        /// <param name="id">A unique id for the image which can be used to reference this image when styling a shape.</param>
        /// <param name="imageUrl">Can be a URL to an image (http(s), or relative path within the LocalWebAssetsFolder (e.g. "map_resources") folder.), base64 data URI, or SVG string.</param>
        /// <returns>A boolean indicating if the image was successfully added to the map image sprite.</returns>
        public async Task AddImageFromUrl(string id, string imageUrl)
        {
            if (_map == null || string.IsNullOrEmpty(id) || id.Equals("none"))
            {
                return;
            }

            var url = await Internal.Utils.SanitizeImageUrl(imageUrl, _map);
            if (!string.IsNullOrEmpty(url))
            {
                imageUrl = url;
            }

            //Keep track of what images the user has added to the map.
            if (_userAddedImages.ContainsKey(id))
            {
                _userAddedImages[id] = imageUrl;
            }
            else
            {
                _userAddedImages.Add(id, imageUrl);
            }

            //addToImageSprite(id, imageData, taskId)
            await _map.JsInterlop.InvokeJsMethodAsync(_map, "addToImageSprite", id, imageUrl);
        }

        /// <summary>
        /// Adds an image to the maps image sprite for use when styling icon images as well as linestring and polygon patterns.
        /// </summary>
        /// <param name="id">A unique id for the image which can be used to reference this image when styling a shape.</param>
        /// <param name="stream">Image stream</param>
        /// <param name="contentType">The mime type of the stream content. Can be the mimeType (e.g. "image/png") or common file extension (e.g. "png") value</param>
        /// <returns></returns>
        public async Task AddImageFromStreamAsync(string id, Stream stream, string contentType = "image/png")
        {
            if (_map == null || string.IsNullOrEmpty(id) || id.Equals("none") || stream == null)
            {
                return;
            }

            try
            {
                string? imageUrl = Internal.Utils.GetDataUriFromStream(stream, contentType);

                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    //Keep track of what images the user has added to the map.
                    if (_userAddedImages.ContainsKey(id))
                    {
                        _userAddedImages[id] = imageUrl;
                    }
                    else
                    {
                        _userAddedImages.Add(id, imageUrl);
                    }

                    //addToImageSprite(id, imageData, taskId)
                    await _map.JsInterlop.InvokeJsMethodAsync(_map, "addToImageSprite", id, imageUrl);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to add image stream to the map image sprite.", ex);
            }
        }

        /// <summary>
        /// Adds an image template to the atlas namespace.
        /// </summary>
        /// <param name="templateName">The name of the template.</param>
        /// <param name="template">The SVG template to add. Supports {color}, {secondaryColor}, {scale}, {text}.</param>
        /// <returns></returns>
        public async Task AddImageTemplateAsync(string templateName, string template)
        {
            if (_userAddedImageTemplates.ContainsKey(templateName))
            {
                _userAddedImageTemplates[templateName] = template;
            }
            else
            {
                _userAddedImageTemplates.Add(templateName, template);
            }

            //atlas.addImageTemplate (templateName: string, template: string, override: boolean) 
            template = Uri.EscapeDataString(template);
            await _map.JsInterlop.InvokeJsMethodAsync(_map, "addImageTemplate", templateName, template, true);
        }

        /// <summary>
        /// Creates and adds an image to the maps image sprite. 
        /// Provide the name of the built-in template to use, and a color to apply. 
        /// Optionally, specifiy a secondary color if the template supports one. A scale can also be specified. 
        /// This will allow the SVG to be scaled before it is converted into an image and thus look much better when scaled up.
        /// </summary>
        /// <param name="id">The image's id</param>
        /// <param name="templateName">The name of the template to use.</param>
        /// <param name="color">The primary color value. </param>
        /// <param name="secondaryColor">A secondary color value. </param>
        /// <param name="scale">Specifies how much to scale the template. 
        /// For best results, scale the icon to the maximum size you want to display it on the map, then use the symbol layers icon size option to scale down if needed. 
        /// This will reduce blurriness due to scaling. </param>
        /// <returns></returns>
        public async Task CreateFromTemplateAsync(string id, string templateName, string? color = "#1A73AA", string? secondaryColor = "#FFFFFF", double? scale = 1)
        {
            if (_map == null || string.IsNullOrEmpty(id) || id.Equals("none"))
            {
                return;
            }

            color = color ?? "#1A73AA";
            secondaryColor = secondaryColor ?? "#fff";
            scale = Math.Abs(scale ?? 1);

            if (!_userCreatedTemplateImages.ContainsKey(id))
            {
                var templateData = new List<object> { templateName, color, secondaryColor, scale };
                _userCreatedTemplateImages.Add(id, templateData);
            }

            //addImageFromTemplate(id, templateId, color, secondaryColor, scale, taskId) 
            await _map.JsInterlop.InvokeJsMethodAsync(_map, "addImageFromTemplate", id, templateName, color, secondaryColor, scale);
        }

        /// <summary>
        /// Retrieves an SVG template by name.
        /// </summary>
        /// <param name="templateName">The name of the template.</param>
        /// <param name="scale">Scale that the SVG should be.</param>
        /// <returns></returns>
        public async Task<string?> GetImageTemplateAsync(string templateName, double? scale = 1)
        {
            //atlas.getImageTemplate(templateName: string, scale?: number)           

            string script = $"btoa(atlas.getImageTemplate('{templateName}', {scale ?? 1}))";

#if MAUI && ANDROID
            Func<Task<string>> scriptFunc = async () =>
            {
                return await _map.JsInterlop._webView.EvaluateJavaScriptAsync(script);
            };

            var template = await MainThread.InvokeOnMainThreadAsync(scriptFunc);
#else
            var template = await _map.JsInterlop._webView.EvaluateJavaScriptAsync(script);
#endif

            if (template == null || template.Equals("null"))
            {
                return null;
            }

            try
            {
                //template is base64 string, convert to UTF8 string.
                return Encoding.UTF8.GetString(Convert.FromBase64String(template));
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Retrieves an array of names for all image templates that are available in the atlas namespace.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> GetAllImageTemplateNamesAsync()
        {
            //atlas.getAllImageTemplateNames()
            return await _map.JsInterlop.InvokeJsMethodAsync<IList<string>>("atlas.getAllImageTemplateNames") ?? new List<string>();
        }

        /// <summary>
        /// Gets a list of all the image ids that have been added to the maps image sprite.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetImageIds()
        {
            var iconIds = new List<string>(_userAddedImages.Keys);
            iconIds.AddRange(_userCreatedTemplateImages.Keys);
            return iconIds;
        }

        /// <summary>
        /// Checks to see if an image with the specified id is loaded into the maps image sprite.
        /// </summary>
        /// <param name="id">The id of the image to check.</param>
        /// <returns>A boolean indicating if an image with the specified id is loaded into the maps image sprite.</returns>
        public bool HasImage(string id)
        {
            return _userCreatedTemplateImages.ContainsKey(id) || _userAddedImages.ContainsKey(id) || _builtInIconIds.Contains(id);
        }

        /// <summary>
        /// Removes an image from the map image sprite by id. 
        /// </summary>
        /// <param name="id">The id of the image to remove.</param>
        public async Task Remove(string id)
        {
            if(_map == null || string.IsNullOrEmpty(id) || !id.Equals("none"))
            {
                return;
            }

            //Can only remove custom images.
            if (!_builtInIconIds.Contains(id))
            {
                await _map.JsInterlop.InvokeJsMethodAsync(_map, "removeImageSprite", id);

                if (_userAddedImageTemplates.ContainsKey(id))
                {
                    _userAddedImageTemplates.Remove(id);
                }
                else if (_userAddedImages.ContainsKey(id))
                {
                    _userAddedImages.Remove(id);
                }
            }
        }

        /// <summary>
        /// Removes all user added images from the map image sprite.
        /// </summary>
        public async void Clear()
        {
            _userCreatedTemplateImages.Clear();
            _userAddedImages.Clear();
            await _map.JsInterlop.InvokeJsMethodAsync(_map, "clearImageSprite");
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Silently removes all data without triggering a notification. This is done when the web page is refreshed.
        /// </summary>
        internal void SilentClear()
        {
            _userAddedImageTemplates.Clear();
            _userAddedImages.Clear();
            _userCreatedTemplateImages.Clear();
        }

        #endregion
    }
}