using AzureMapsNativeControl.Control;
using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl.Drawing
{
    /// <summary>
    /// Options for the drawing toolbar.
    /// </summary>
    public class DrawingToolbarOptions : IDeepCloneable<DrawingToolbarOptions>, INotifyPropertyChanged
    {
        #region Private Properties

        private IList<DrawingMode>? _buttons = new List<DrawingMode>() {
            DrawingMode.DrawPoint,
            DrawingMode.DrawLine,
            DrawingMode.DrawPolygon,
            DrawingMode.DrawCircle,
            DrawingMode.DrawRectangle,
            DrawingMode.EditGeometry,
            DrawingMode.EraseGeometry
        };
        private int? _numColumns = null;
        private bool _visible = true;
        private ControlPosition _position = ControlPosition.NonFixed;
        private ControlStyle _style = ControlStyle.Light;

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// An array of buttons to display in the toolbar. The order of this array will match the order of the buttons in the toolbar. 
        /// Valid values are any drawing mode except "idle". Default is all drawing modes except "idle".
        /// </summary>
        [JsonPropertyName("buttons")]
        public IList<DrawingMode>? Buttons
        {
            get => _buttons;
            set
            {
                if (_buttons != value)
                {
                    _buttons = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Buttons)));
                }
            }
        }

        /// <summary>
        /// The number of columns to display the buttons with. If the number of columns is greater than or equal to the number of buttons 
        /// the toolbar will be a single horizontal row. If only one column is used the toolbar will be a single vertical column.
        /// </summary>
        [JsonPropertyName("numColumns")]
        public int? NumColumns 
        {
            get => _numColumns;
            set
            {
                if (_numColumns != value)
                {
                    _numColumns = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NumColumns)));
                }
            }
        }

        /// <summary>
        /// Specifies if the toolbar is visible or not. Default true.
        /// </summary>
        [JsonPropertyName("visible")]
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
                }
            }
        }

        /// <summary>
        /// The position the control will be placed on the map. If not specified, the control will be located at the default position it defines.
        /// </summary>
        [JsonPropertyName("position")]
        public ControlPosition Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
                }
            }
        }

        /// <summary>
        /// The style of the control.
        /// Can only set before adding the control to the map, unless the control has a setOptions method that supports updating this.
        /// </summary>
        [JsonPropertyName("style")]
        public ControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Style)));
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep clone of the options.
        /// </summary>
        /// <returns></returns>
        public DrawingToolbarOptions DeepClone()
        {
            return new DrawingToolbarOptions()
            {
                Buttons = Buttons?.ToList(),
                NumColumns = NumColumns,
                Position = Position,
                Style = Style,
                Visible = Visible
            };
        }

        /// <summary>
        /// Merges the source options into the target options.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns>True if changes have occured to the target.</returns>
        public static bool Merge(DrawingToolbarOptions source, DrawingToolbarOptions target)
        {
            if (source != null && target != null)
            {
                bool hasChanges = false;

                if (source.Buttons != null && source.Buttons != target.Buttons)
                {
                    target.Buttons = source.Buttons;
                    hasChanges = true;
                }

                if (source.NumColumns != null && source.NumColumns != target.NumColumns)
                {
                    target.NumColumns = source.NumColumns;
                    hasChanges = true;
                }

                if (source.Visible != null && source.Visible != target.Visible)
                {
                    target.Visible = source.Visible;
                    hasChanges = true;
                }

                if (source.Position != null && source.Position != target.Position)
                {
                    target.Position = source.Position;
                    hasChanges = true;
                }

                if (source.Style != null && source.Style != target.Style)
                {
                    target.Style = source.Style;
                    hasChanges = true;
                }

                return hasChanges;
            }

            return false;
        }

        #endregion
    }
}
