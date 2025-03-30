namespace AzureMapsNativeControl.Core
{
    /// <summary>
    /// A manager for the map control's HTML markers. Exposed through the markers property of the atlas.Map class. Cannot be instantiated by the user.
    /// </summary>
    public sealed class HtmlMarkerManager : BaseMapEntityCollection<HtmlMarker, HtmlMarkerOptions>
    {
        #region Contstructor

        /// <summary>
        /// A manager for the map control's HTML markers. Exposed through the markers property of the atlas.Map class. Cannot be instantiated by the user.
        /// </summary>
        /// <param name="map">Map instance manager is attached to.</param>
        public HtmlMarkerManager(Map map) : base(map, "addMarker", "removeMarkers") 
        { 
        }

        #endregion
    }
}
