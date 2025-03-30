using AzureMapsNativeControl.Core;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Animations
{
    public enum PointPairInterpolation
    {
        [EnumMember(Value = "linear")]
        Linear,       
        
        [EnumMember(Value = "nearest")]
        Nearest,

        [EnumMember(Value = "min")]
        Min,

        [EnumMember(Value = "max")]
        Max,

        [EnumMember(Value = "avg")]
        Avg
    }

    /// <summary>
    /// Defines how the value of property in two points is extrapolated. 
    /// </summary>
    public class PointPairValueInterpolation: IDeepCloneable<PointPairValueInterpolation>
    {
        /// <summary>
        ///  How the interpolation is performed. Certain interpolations require the data to be a certain value.
        /// - `linear`,`min`, `max`, `avg`: `number` or `Date`
        /// - `nearest`: `any`
        /// Default: `linear`
        /// </summary>
        [JsonPropertyName("interpolation")]
        public PointPairInterpolation Interpolation { get; set; } = PointPairInterpolation.Linear;

        /// <summary>
        /// The path to the property with each sub-property separated with a forward slash "/", for example "property/subproperty1/subproperty2".
        /// Array indices can be added as subproperties as well, for example "property/0".
        /// </summary>
        [JsonPropertyName("propertyPath")]
        public string PropertyPath { get; set; } = string.Empty;

        /// <inheritdoc/>
        public PointPairValueInterpolation DeepClone()
        {
            return new PointPairValueInterpolation
            {
                Interpolation = Interpolation,
                PropertyPath = PropertyPath
            };
        }
    }
}
