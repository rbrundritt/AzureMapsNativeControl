using AzureMapsNativeControl.Core;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Control
{
    /// <summary>
    /// Interface for all controls.
    /// </summary>
    public interface IBaseControl : IMapEventTarget
    {
        Map? _map { get; internal set; }
    }
}
