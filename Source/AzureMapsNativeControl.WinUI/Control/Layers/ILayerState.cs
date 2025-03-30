using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control.Layers
{
    [JsonDerivedType(typeof(LayerState))]
    [JsonDerivedType(typeof(RangeLayerState))]
    public interface ILayerState
    {
    }
}
