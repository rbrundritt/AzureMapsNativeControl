using AzureMapsNativeControl.Core;
using AzureMapsNativeControl.Data;
using AzureMapsNativeControl.Data.JsonConverters;
using AzureMapsNativeControl.Internal.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace AzureMapsNativeControl
{
    /// <summary>
    /// Template for the popup.
    /// </summary>
    public class PopupTemplate : IDeepCloneable<PopupTemplate>
    {
        #region Constructor 

        /// <summary>
        /// Creates a new instance of the popup template.
        /// </summary>
        public PopupTemplate()
        {
        }

        /// <summary>
        /// Creates a new instance of the popup template.
        /// </summary>
        /// <param name="Properties">The properties of the feature process with the template.</param>
        public PopupTemplate(PropertiesTable? Properties)
        {
            this.Properties = Properties;
        }

        #endregion
        
        #region Public Properties
                
        /// <summary>
        /// The properties of the feature process with the template.
        /// </summary>
        [JsonPropertyName("properties")]
        [JsonConverter(typeof(PropertiesTableConverter))]
        public PropertiesTable? Properties { get; set; }

        /// <summary>
        /// The CSS background color of the popup template.
        /// </summary>
        [JsonPropertyName("fillColor")]
        public string? FillColor { get; set; }

        /// <summary>
        /// The default CSS text color of the popup template.
        /// </summary>
        [JsonPropertyName("textColor")]
        public string? TextColor { get; set; }

        /// <summary>
        /// A HTML string for the title of the popup that contains placeholders for properties of the feature it is being displayed for.
        /// Placeholders can be in the format "{propertyName}" or "{propertyName/subPropertyName}".
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// If a description is available, it will be written as the content rather than as a table of properties. Default: true
        /// </summary>
        [JsonPropertyName("singleDescription")]
        public bool? SingleDescription { get; set; } = true;

        /// <summary>
        /// An array content information. 
        /// Content can be of type PopupStringContent or PopupPropertyInfoContent.
        /// </summary>
        [JsonPropertyName("content")]
        [JsonConverter(typeof(PopupContentEnumerableConverter))]
        public IEnumerable<IPopupContent>? Content { get; set; }

        /// <summary>
        /// Specifies if content should be wrapped with a sandboxed iframe.
        /// Unless explicitly set to false, the content will be sandboxed within an iframe by default.
        /// When enabled, all content will be wrapped in a sandboxed iframe with scripts, forms, pointer lock and top navigation disabled.
        /// Popups will be allowed so that links can be opened in a new page or tab.
        /// Older browsers that don't support the srcdoc parameter on iframes will be limited to rendering a small amount of content.
        /// </summary>
        [JsonPropertyName("sandboxContent")]
        public bool? SandboxContent { get; set; }

        /// <summary>
        /// If the property is a date object, these options specify how it should be formatted when displayed.
        /// Uses [Date.toLocaleString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toLocaleString).
        /// If not specified, dates will be converted to strings using
        /// [Date.toISOString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toISOString)
        /// </summary>
        [JsonPropertyName("dateFormat")]
        public JSDateTimeFormatOptions? DateFormat { get; set; }

        /// <summary>
        /// If the property is a number, these options specify how it should be formatted when displayed.
        /// Uses [Number.toLocaleString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/toLocaleString).
        /// </summary>
        [JsonPropertyName("numberFormat")]
        public JSNumberFormatOptions? NumberFormat { get; set; }

        /// <summary>
        /// Specifies if hyperlinks and email addresses should automatically be detected and rendered as clickable links. Default: `true`
        /// </summary>
        [JsonPropertyName("detectHyperlinks")]
        public bool? DetectHyperlinks { get; set; } = true;

        /// <summary>
        /// Specifies if property paths should be parsed using forward slashes "/" as sub-property dividers,
        /// or if the whole path should be treated as one property name.
        /// Default: `true`
        /// </summary>
        [JsonPropertyName("parsePropertyPaths")]
        public bool? ParsePropertyPaths { get; set; } = true;

        /// <summary>
        /// Format options for hyperlink strings. 
        /// </summary>
        [JsonPropertyName("hyperlinkFormat")]
        public HyperLinkFormatOptions? HyperlinkFormat { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the popup template.
        /// </summary>
        /// <returns></returns>
        public PopupTemplate DeepClone()
        {
            return new PopupTemplate(Properties?.DeepClone())
            {
                FillColor = FillColor,
                TextColor = TextColor,
                Title = Title,
                SingleDescription = SingleDescription,
                Content = Content?.Select<IPopupContent, IPopupContent>(c => c switch
                {
                    PopupStringContent psc => psc.DeepClone(),
                    PopupPropertyInfoContent ppic => ppic.DeepClone(),
                    _ => throw new NotSupportedException()
                }),
                SandboxContent = SandboxContent,
                DateFormat = DateFormat?.DeepClone(),
                NumberFormat = NumberFormat?.DeepClone(),
                DetectHyperlinks = DetectHyperlinks,
                ParsePropertyPaths = ParsePropertyPaths,
                HyperlinkFormat = HyperlinkFormat?.DeepClone()
            };
        }

        #endregion
    }

    /// <summary>
    /// Interface for popup content.
    /// Can be PopupStringContent or PopupPropertyInfoContent.
    /// </summary>
    [JsonDerivedType(typeof(PopupStringContent))]
    [JsonDerivedType(typeof(PopupPropertyInfoContent))]
    public interface IPopupContent
    {
    }

    /// <summary>
    /// A simple HTML string that has placeholders from property values.
    /// A HTML string for the main content of the popup that contains placeholders for properties of the feature it is being displayed for.
    /// Placeholders can be in the format "{propertyName}" or "{propertyName/subPropertyName}".
    /// </summary>
    public class PopupStringContent: IPopupContent, IDeepCloneable<PopupStringContent>
    {
        #region Public Properties

        /// <summary>
        /// A simple HTML string that has placeholders from property values.
        /// </summary>
        public PopupStringContent() { }

        /// <summary>
        /// A simple HTML string that has placeholders from property values.
        /// Placeholders can be in the format "{propertyName}" or "{propertyName/subPropertyName}".
        /// </summary>
        /// <param name="htmlTemplate">A HTML string that can contain placeholders for properties of the feature it is being displayed for.</param>
        public PopupStringContent(string htmlTemplate)
        {
            HtmlTemplate = htmlTemplate;
        }

        /// <summary>
        /// A HTML string that can contain placeholders for properties of the feature it is being displayed for.
        /// Placeholders can be in the format "{propertyName}" or "{propertyName/subPropertyName}".
        /// </summary>
        public string? HtmlTemplate { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        /// <returns></returns>
        public PopupStringContent DeepClone()
        {
            return new PopupStringContent
            {
                HtmlTemplate = HtmlTemplate
            };
        }

        #endregion
    }

    /// <summary>
    /// A collection of property info objects that specify how properties should be displayed.
    /// </summary>
    public class PopupPropertyInfoContent : List<PopupPropertyInfo>, IPopupContent, IDeepCloneable<PopupPropertyInfoContent>
    {
        public PopupPropertyInfoContent DeepClone()
        {
            var clone = new PopupPropertyInfoContent();

            foreach (var item in this)
            {
                clone.Add(item.DeepClone());
            }

            return clone;
        }
    }

    /// <summary>
    /// Specifies details of how a property is to be displayed.
    /// </summary>
    public class PopupPropertyInfo : IDeepCloneable<PopupPropertyInfo>
    {
        #region Public Properties

        /// <summary>
        /// The path to the property with each sub-property separated with a forward slash "/", for example "property/subproperty1/subproperty2.
        /// Array indices can be added as subproperties as well, for example "property/0".
        /// </summary>
        [JsonPropertyName("propertyPath")]
        public string? PropertyPath { get; set; }

        /// <summary>
        /// The label to display for this property. If not specified, will fallback to the property name.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// Specifies if the label of this property should be hidden and the content should span both columns of the table.
        /// Default: `false`
        /// </summary>
        [JsonPropertyName("hideLabel")]
        public bool? HideLabel { get; set; } = false;

        /// <summary>
        /// If the property is a date object, these options specify how it should be formatted when displayed.
        /// Uses [Date.toLocaleString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toLocaleString).
        /// If not specified, dates will be converted to strings using
        /// [Date.toISOString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toISOString)
        /// </summary>
        [JsonPropertyName("dateFormat")]
        public JSDateTimeFormatOptions? DateFormat { get; set; }

        /// <summary>
        /// If the property is a number, these options specify how it should be formatted when displayed.
        /// Uses [Number.toLocaleString](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/toLocaleString).
        /// </summary>
        [JsonPropertyName("numberFormat")]
        public JSNumberFormatOptions? NumberFormat { get; set; }

        /// <summary>
        /// Format options for hyperlink strings. 
        /// </summary>
        [JsonPropertyName("hyperlinkFormat")]
        public HyperLinkFormatOptions? HyperlinkFormat { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        /// <returns></returns>
        public PopupPropertyInfo DeepClone()
        {
            return new PopupPropertyInfo
            {
                    PropertyPath = PropertyPath,
                    Label = Label,
                    HideLabel = HideLabel,
                    DateFormat = DateFormat?.DeepClone(),
                    NumberFormat = NumberFormat?.DeepClone(),
                    HyperlinkFormat = HyperlinkFormat?.DeepClone()
                };
        }

        #endregion
    }

    /// <summary>
    /// Format option for hyperlink strings.
    /// </summary>
    public class HyperLinkFormatOptions: IDeepCloneable<HyperLinkFormatOptions>
    {
        #region Public Properties

        /// <summary>
        /// Specifies the text that should be displayed to the user.
        /// If not specified, the hyperlink will be displayed.
        /// If the hyperlink is an image, this will be set as the "alt" property of the img tag.
        /// </summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// Specifies if the hyperlink is for an image.
        /// If set to true, the hyperlink will be loaded into an img tag and when clicked,
        /// will open the hyperlink to the image.
        /// </summary>
        [JsonPropertyName("isImage")]
        public bool? IsImage { get; set; }

        /// <summary>
        /// Specifies a scheme to prepend to a hyperlink such as 'mailto:' or 'tel:'.
        /// </summary>
        [JsonPropertyName("scheme")]
        public string? Scheme { get; set; }

        /* Purposely not exposing at this time. Prefer URLs to open outside of the map browser.
            /// <summary>
            /// Specifies the where the hyperlink should open.
            /// Supported values: "_blank", "_self", "_parent", "_top"
            /// </summary>
            [JsonPropertyName("scheme")]
            public string? Target { get; set; }
        */

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        /// <returns></returns>
        public HyperLinkFormatOptions DeepClone()
        {
            return new HyperLinkFormatOptions
            {
                Label = Label,
                IsImage = IsImage,
                Scheme = Scheme
            };
        }

        #endregion
    }

    /// <summary>
    /// JavaScript options for formatting date objects.
    /// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/DateTimeFormat
    /// </summary>
    public class JSDateTimeFormatOptions : IDeepCloneable<JSDateTimeFormatOptions>
    {
        #region Public Properties

        /// <summary>
        /// The representation of the locale.
        /// Possible values are "lookup" and "best fit".
        /// </summary>
        [JsonPropertyName("localeMatcher")]
        public string? LocaleMatcher { get; set; }

        /// <summary>
        /// The representation of the year.
        /// Possible values are "basic" and "best fit".
        /// </summary>
        [JsonPropertyName("formatMatcher")]
        public string? FormatMatcher { get; set; }

        /// <summary>
        /// The representation of the year.
        /// </summary>
        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }

        /// <summary>
        /// The representation of the year.
        /// </summary>
        [JsonPropertyName("hour12")]
        public bool? Hour12 { get; set; }

        /// <summary>
        /// The representation of the year.
        /// Possible values are "h11", "h12", "h23", and "h24". 
        /// </summary>
        [JsonPropertyName("hourCycle")]
        public string? HourCycle { get; set; }

        /// <summary>
        /// The representation of the weekday.
        /// Possible values are "narrow", "short", and "long".
        /// </summary>
        [JsonPropertyName("weekday")]
        public string? Weekday { get; set; }

        /// <summary>
        /// The representation of the era.
        /// Possible values are "narrow", "short", and "long".
        /// </summary>
        [JsonPropertyName("era")]
        public string? Era { get; set; }

        /// <summary>
        /// The representation of the year.
        /// Possible values are "numeric", "2-digit".
        /// </summary>
        [JsonPropertyName("year")]
        public string? Year { get; set; }

        /// <summary>
        /// The representation of the month.
        /// Possible values are "numeric", "2-digit", "narrow", "short", and "long".
        /// </summary>
        [JsonPropertyName("month")]
        public string? Month { get; set; }

        /// <summary>
        /// The representation of the day.
        /// </summary>
        [JsonPropertyName("day")]
        public string? Day { get; set; }

        /// <summary>
        /// The formatting style used for day periods like "in the morning", "am", "noon", "n" etc. 
        /// Possible values are "narrow", "short", and "long".
        /// </summary>
        [JsonPropertyName("dayPeriod")]
        public string? DayPeriod { get; set; }

        /// <summary>
        /// The representation of the hours.
        /// </summary>
        [JsonPropertyName("hour")]
        public string? Hour { get; set; }

        /// <summary>
        /// The representation of the minutes.
        /// </summary>
        [JsonPropertyName("minute")]
        public string? Minute { get; set; }

        /// <summary>
        /// The representation of the seconds.
        /// </summary>
        [JsonPropertyName("second")]
        public string? Second { get; set; }

        /// <summary>
        /// The time zone to use. The only value implementations must recognize is "UTC";
        /// Possible values are: "long", "short", "shortOffset", "longOffset", "shortGeneric", "longGeneric"
        /// </summary>
        [JsonPropertyName("timeZoneName")]
        public string? TimeZoneName { get; set; }

        /// <summary>
        /// The representation of the fractional seconds.
        /// </summary>
        [JsonPropertyName("fractionalSecondDigits")]
        public int? FractionalSecondDigits { get; set; }

        /// <summary>
        /// The representation of the numbering system.
        /// </summary>
        [JsonPropertyName("numberingSystem")]
        public string? NumberingSystem { get; set; }

        /// <summary>
        /// The representation of the calendar.
        /// </summary>
        [JsonPropertyName("calendar")]
        public string? Calendar { get; set; }

        /// <summary>
        /// The representation of the date.
        /// Possible values are "full", "long", "medium", and "short". 
        /// </summary>
        [JsonPropertyName("dateStyle")]
        public string? DateStyle { get; set; }

        /// <summary>
        /// The representation of the time.
        /// Possible values are "full", "long", "medium", and "short".
        /// </summary>
        [JsonPropertyName("timeStyle")]
        public string? TimeStyle { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        /// <returns></returns>
        public JSDateTimeFormatOptions DeepClone()
        {
            return new JSDateTimeFormatOptions
            {
                LocaleMatcher = LocaleMatcher,
                FormatMatcher = FormatMatcher,
                TimeZone = TimeZone,
                Hour12 = Hour12,
                HourCycle = HourCycle,
                Weekday = Weekday,
                Era = Era,
                Year = Year,
                Month = Month,
                Day = Day,
                DayPeriod = DayPeriod,
                Hour = Hour,
                Minute = Minute,
                Second = Second,
                TimeZoneName = TimeZoneName,
                FractionalSecondDigits = FractionalSecondDigits,
                NumberingSystem = NumberingSystem,
                Calendar = Calendar,
                DateStyle = DateStyle,
                TimeStyle = TimeStyle
            };
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Converts a DateTime object to a JavaScript date number.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ToJSDateNumber(DateTime dateTime)
        {
            return Math.Round((dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        /// <summary>
        /// Converts a JavaScript date number to a DateTime object.
        /// </summary>
        /// <param name="jsDateNumber"></param>
        /// <returns></returns>
        public static DateTime FromJSDateNumber(double jsDateNumber)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(jsDateNumber).ToLocalTime();
        }

        #endregion
    }

    /// <summary>
    /// JavaScript options for formatting number objects.
    /// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Intl/NumberFormat
    /// </summary>
    public class JSNumberFormatOptions : IDeepCloneable<JSNumberFormatOptions>
    {
        #region Public Properties

        /// <summary>
        /// The representation of the locale.
        /// Possible values are "lookup" and "best fit".
        /// </summary>
        [JsonPropertyName("localeMatcher")]
        public string? LocaleMatcher { get; set; }

        /// <summary>
        /// The numbering system to use for number formatting.
        /// </summary>
        [JsonPropertyName("numberingSystem")]
        public string? NumberingSystem { get; set; }

        /// <summary>
        /// The formatting style to use.
        /// Possible values are "decimal", "percent", "currency", "unit".
        /// </summary>
        [JsonPropertyName("style")]
        public string? Style { get; set; }

        /// <summary>
        /// The currency to use in currency formatting. 
        /// Possible values are the ISO 4217 currency codes, such as "USD" for the US dollar, or "EUR" for the euro. 
        /// </summary>
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        /// <summary>
        /// The currency display style to use in currency formatting.
        /// Possible values are "symbol", "narrowSymbol", "code", "name".
        /// </summary>
        [JsonPropertyName("currencyDisplay")]
        public string? CurrencyDisplay { get; set; }

        /// <summary>
        /// In many locales, accounting format means to wrap the number with parentheses instead of appending a minus sign.
        /// Possible values are "standard" and "accounting"
        /// </summary>
        [JsonPropertyName("currencySign")]
        public string? CurrencySign { get; set; }

        /// <summary>
        /// The unit to use in unit formatting.
        /// </summary>
        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        /// <summary>
        /// The unit display style to use in unit formatting.
        /// Possible values are "long", "short", "narrow".
        /// </summary>
        [JsonPropertyName("unitDisplay")]
        public string? UnitDisplay { get; set; }

        /// <summary>
        /// The minimum number of integer digits to use.
        /// </summary>
        [JsonPropertyName("minimumIntegerDigits")]
        public int? MinimumIntegerDigits { get; set; }

        /// <summary>
        /// The minimum number of fraction digits to use.
        /// </summary>
        [JsonPropertyName("minimumFractionDigits")]
        public int? MinimumFractionDigits { get; set; }

        /// <summary>
        /// The maximum number of fraction digits to use.
        /// </summary>
        [JsonPropertyName("maximumFractionDigits")]
        public int? MaximumFractionDigits { get; set; }

        /// <summary>
        /// The minimum number of significant digits to use.
        /// </summary>
        [JsonPropertyName("minimumSignificantDigits")]
        public int? MinimumSignificantDigits { get; set; }

        /// <summary>
        /// The maximum number of significant digits to use.
        /// </summary>
        [JsonPropertyName("maximumSignificantDigits")]
        public int? MaximumSignificantDigits { get; set; }

        /// <summary>
        /// The rounding method to use.
        /// Possible values are "auto", "morePrecision", "lessPrecision".
        /// </summary>
        [JsonPropertyName("roundingPriority")]
        public string? RoundingPriority { get; set; }

        /// <summary>
        /// The rounding increment to use.
        /// </summary>
        [JsonPropertyName("roundingIncrement")]
        public int? RoundingIncrement { get; set; }

        /// <summary>
        /// The rounding mode to use.
        /// Possible values are "ceil", "floor", "expand", "trunc", "halfCeil", "halfFloor", "halfExpand", "halfTrunc", "halfEven".
        /// </summary>
        [JsonPropertyName("roundingMode")]
        public string? RoundingMode { get; set; }

        /// <summary>
        /// The strategy for displaying trailing zeros on whole numbers.
        /// Possible values are "auto", "stripIfInteger".
        /// </summary>
        [JsonPropertyName("trailingZeroDisplay")]
        public string? TrailingZeroDisplay { get; set; }

        /// <summary>
        /// The notation to use.
        /// Possible values are "standard", "scientific", "engineering", "compact".
        /// </summary>
        [JsonPropertyName("notation")]
        public string? Notation { get; set; }

        /// <summary>
        /// The compact display option to use.
        /// Possible values are "short" and "long".
        /// </summary>
        [JsonPropertyName("compactDisplay")]
        public string? compactDisplay { get; set; }

        /// <summary>
        /// Whether to use grouping separators, such as thousands separators or thousand/lakh/crore separators.
        /// Possible values are "always", "auto", "min2".
        /// </summary>
        [JsonPropertyName("useGrouping")]
        public string? UseGrouping { get; set; }

        /// <summary>
        /// The sign display option to use.
        /// Possible values are "auto", "never", "negative", "always", "exceptZero".
        /// </summary>
        [JsonPropertyName("signDisplay")]
        public string? SignDisplay { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of the object.
        /// </summary>
        /// <returns></returns>
        public JSNumberFormatOptions DeepClone()
        {
            return new JSNumberFormatOptions
            {
                LocaleMatcher = LocaleMatcher,
                NumberingSystem = NumberingSystem,
                Style = Style,
                Currency = Currency,
                CurrencyDisplay = CurrencyDisplay,
                CurrencySign = CurrencySign,
                Unit = Unit,
                UnitDisplay = UnitDisplay,
                MinimumIntegerDigits = MinimumIntegerDigits,
                MinimumFractionDigits = MinimumFractionDigits,
                MaximumFractionDigits = MaximumFractionDigits,
                MinimumSignificantDigits = MinimumSignificantDigits,
                MaximumSignificantDigits = MaximumSignificantDigits,
                RoundingPriority = RoundingPriority,
                RoundingIncrement = RoundingIncrement,
                RoundingMode = RoundingMode,
                TrailingZeroDisplay = TrailingZeroDisplay,
                Notation = Notation,
                compactDisplay = compactDisplay,
                UseGrouping = UseGrouping,
                SignDisplay = SignDisplay
            };
        }

        #endregion
    }
}
