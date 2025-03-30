using AzureMapsNativeControl.Core;
using System.Collections.Generic;
using System.Linq;
using System;

#if WINUI
using Windows.UI; 
#elif WPF
using System.Windows.Media;
#endif

namespace AzureMapsNativeControl
{
    /// <summary>
    /// A class for data driven expressions types.
    /// </summary>
    public class Expression: List<object?>, IEquatable<Expression>, IDeepCloneable<Expression>
    {
        #region Constructor

        /// <summary>
        /// A class for data driven expressions types.
        /// </summary>
        public Expression()
        {

        }

        /// <summary>
        /// A class for data driven expressions types.
        /// </summary>
        /// <param name="values"></param>
        public Expression(params object[]? values)
        {
            if(values != null)
            {
                this.AddRange(values);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Specifies if the expression is a literal value.
        /// </summary>
        public bool IsLiteral => this.Count == 2 && this[0]?.Equals("literal") == true;

        /// <summary>
        /// If the expression represents a literal value, this will return the value as an object.
        /// </summary>
        /// <returns></returns>
        public object? GetLiteralValue()
        {
            if (IsLiteral)
            {
                return this[1];
            }

            return null;
        }

        /// <summary>
        /// If the expression represents a literal value, this will check to see if that value is equal to the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsLiteralEquals(object? value)
        {
            if (IsLiteral)
            {
                return this[1]?.Equals(value) == true;
            }

            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tries to create a deep clone of the expression.
        /// </summary>
        /// <returns></returns>
        public Expression DeepClone()
        {
            //Clone the contents of the expression.
            var clone = new Expression();

            foreach (var item in this)
            {
                if (item is IDeepCloneable<object> deepCloneable)
                {
                    clone.Add(deepCloneable.DeepClone());
                }
                else if (item is ICloneable cloneable)
                {
                    clone.Add(cloneable.Clone());
                }
                else
                {
                    clone.Add(item);
                }
            }

            return clone;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(Expression? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Expression));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Expression? left, Expression? right)
        {
            return (left == right);
        }

        #endregion

        #region Static Methods

        #region Exrepssion creation helpers

        /// <summary>
        /// Converts a color to a string expression.
        /// </summary>
        /// <param name="color">Color object</param>
        /// <returns></returns>
        public static Expression<string> Color(Color color)
        {
#if MAUI
            color.ToRgba(out byte r, out byte g, out byte b, out byte a);
#elif WINUI || WPF
            float a = color.A;
            float r = color.R;
            float g = color.G;
            float b = color.B;
#endif

            //Create a CSS rgba string and pass as a literal value.
            return new Expression<string> { "literal", $"rgba({r},{g},{b},{a / 255})" };
        }

        /// <summary>
        /// A data driven expression, defined as a literal value expression. Passes value as is.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Literal(object? value)
        {
            return new Expression() { "literal", value };
        }

#endregion

        #region Common Reusable Expressions

        /// <summary>
        /// A filter that removes all restrictions. Use this to clear a set filter.
        /// </summary>
        /// <returns></returns>
        public static Expression<bool> ClearFilter()
        {
            return new Expression<bool> { "any", true };
        }

        /// <summary>
        /// A filter expression that checks if the geometry type is a Point or MultiPoint.
        /// </summary>
        /// <returns></returns>
        public static Expression<bool> PointTypeFilter()
        {
            //['any', ['==', ['geometry-type'], 'Point'], ['==', ['geometry-type'], 'MultiPoint']]
            return new Expression<bool>
            {
                "any",
                new object[] { "==", new object[] { "geometry-type" }, "Point" },
                new object[] { "==", new object[] { "geometry-type" }, "MultiPoint" }
            };
        }

        /// <summary>
        /// A filter expression that checks if the geometry type is a LineString or MultiLineString.
        /// </summary>
        /// <returns></returns>
        public static Expression<bool> LineStringTypeFilter()
        {
            // ['any', ['==', ['geometry-type'], 'LineString'], ['==', ['geometry-type'], 'MultiLineString']]
            return new Expression<bool>
            {
                "any",
                new object[] { "==", new object[] { "geometry-type" }, "LineString" },
                new object[] { "==", new object[] { "geometry-type" }, "MultiLineString" }
            };
        }

        /// <summary>
        /// A filter expression that checks if the geometry type is a Polygon or MultiPolygon.
        /// </summary>
        /// <returns></returns>
        public static Expression<bool> PolygonTypeFilter()
        {
            //['any', ['==', ['geometry-type'], 'Polygon'], ['==', ['geometry-type'], 'MultiPolygon']]
            return new Expression<bool>
            {
                "any",
                new object[] { "==", new object[] { "geometry-type" }, "Polygon" },
                new object[] { "==", new object[] { "geometry-type" }, "MultiPolygon" }
            };
        }

        /// <summary>
        /// A data driven expression that represents a "undefined" value. This is needed to disable some options, such as fillPattern.
        /// </summary>
        /// <returns></returns>
        public static Expression Undefined()
        {
            return new Expression()
            {
                "literal",
                "undefined"
            };
        }

        #endregion

        #region Comparison checks

        /// <summary>
        /// Determines whether two specified expressions have the same value.
        /// </summary>
        /// <param name="left">The first expression to compare.</param>
        /// <param name="right">The first expression to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Expression? left, Expression? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (right is null || left is null)
            {
                return false;
            }

            return left.SequenceEqual(right);
        }

        /// <summary>
        /// Determines whether two specified expressions don't have the same value.
        /// </summary>
        /// <param name="left">The first expression to compare.</param>
        /// <param name="right">The first expression to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(Expression? left, Expression? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines if an expression is null or has an empty string value.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(Expression? exp)
        {
            if (Expression.IsNull(exp))
            {
                return true;
            }

            if (exp.IsLiteral)
            {
                var val = exp.GetLiteralValue();

                if (val != null && val is string && string.IsNullOrWhiteSpace((string)val))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if an expression is null. Checks the instance of the object, the expression and the value.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool IsNull(Expression? exp)
        {
            if (exp == null || (exp.Count == 0 || (exp.IsLiteral && exp.GetLiteralValue() == null)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Specifies if the expression is an undefined literal type: ['literal', 'undefined']
        /// </summary>
        /// <returns></returns>
        public bool IsUndefined()
        {
            return this.Count == 2 && this[0]?.Equals("literal") == true && this[1]?.Equals("undefined") == true;
        }

        /// <summary>
        /// Checks to see if the value of an expression is greater than or equal to a specified value.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGreaterOrEqualTo(Expression? exp, double value)
        {
            if (Expression.IsNull(exp))
            {
                return false;
            }

            if (exp != null)
            {
                if (!exp.IsLiteral)
                {
                    return true;
                }

                var val = exp.GetLiteralValue();

                if (val is double)
                {
                    double value1 = (double)val;

                    if (value1 >= value)
                    {
                        return true;
                    }
                }
                else if (val is int)
                {
                    int value1 = (int)val;

                    if (value1 >= value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the value of an expression is greater than a specified value.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsGreaterThan(Expression? exp, double value)
        {
            if (Expression.IsNull(exp))
            {
                return false;
            }

            if (exp != null)
            {

                if (!exp.IsLiteral)
                {
                    return true;
                }

                var val = exp.GetLiteralValue();

                if (val is double)
                {
                    double value1 = (double)val;

                    if (value1 > value)
                    {
                        return true;
                    }
                }
                else if (val is int)
                {
                    int value1 = (int)val;

                    if (value1 > value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the value of an expression is positive (greater or equal to 0).
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool IsPositive(Expression? exp)
        {
            if (Expression.IsNull(exp))
            {
                return false;
            }

            if (exp != null)
            {

                if (!exp.IsLiteral)
                {
                    return true;
                }

                var val = exp.GetLiteralValue();

                if (val is double)
                {
                    double value = (double)val;

                    if (value >= 0)
                    {
                        return true;
                    }
                }
                else if (val is int)
                {
                    int value = (int)val;

                    if (value >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the value of an expression is within a specified value.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsValueInRange(Expression? exp, double min, double max)
        {
            if (Expression.IsNull(exp))
            {
                return false;
            }

            if (exp != null)
            {
                if (!exp.IsLiteral)
                {
                    return true;
                }

                var val = exp.GetLiteralValue();

                if (val is double)
                {
                    double value = (double)val;

                    if (value >= min && value <= max)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

#endregion
    }

    /// <summary>
    /// A class for data driven expressions types.
    /// </summary>
    /// <typeparam name="T">The value type of the expression output.</typeparam>
    public class Expression<T>: Expression, IEquatable<Expression<T>>, IDeepCloneable<Expression<T>>
    {
        #region Constructor

        /// <summary>
        /// A class for data driven expressions types.
        /// </summary>
        public Expression()
        {

        }

        /// <summary>
        /// A class for data driven expressions types.
        /// </summary>
        /// <param name="values"></param>
        public Expression(params object[]? values)
        {
            if (values != null)
            {
                this.AddRange(values);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A data driven expression, defined as a literal value expression. Passes value as is.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression<T> Literal(T? value)
        {
            return new Expression<T>()
            {
                "literal",
                value
            };
        }

        /// <summary>
        /// A data driven expression that represents a "undefined" value. This is needed to disable some options, such as fillPattern.
        /// </summary>
        /// <returns></returns>
        public static new Expression<T> Undefined()
        {
            return new Expression<T>()
            {
                "literal",
                "undefined"
            };
        }

        /// <summary>
        /// A data driven expression that "get"s a property value by name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A "get" expression for the property.</returns>
        public static Expression<T> Get(string propertyName)
        {
            return new Expression<T>()
            {
                "get",
                propertyName
            };
        }

        /// <summary>
        /// Tries to create a deep clone of the expression.
        /// </summary>
        /// <returns></returns>
        public new Expression<T> DeepClone()
        {
            //Clone the contents of the expression.
            var clone = new Expression<T>();

            foreach (var item in this)
            {
                if (item is IDeepCloneable<object> deepCloneable)
                {
                    clone.Add(deepCloneable.DeepClone());
                }
                else if (item is ICloneable cloneable)
                {
                    clone.Add(cloneable.Clone());
                }
                else
                {
                    clone.Add(item);
                }
            }

            return clone;
        }
        
        /// <inheritdoc />
        public bool Equals(Expression<T>? other)
        {
            return (this == other);
        }

        /// <inheritdoc />
        public override bool Equals(object? other)
        {
            return (this == (other as Expression<T>));
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(Expression<T>? left, Expression<T>? right)
        {
            return (left == right);
        }

        /// <summary>
        /// Determines whether two specified expressions have the same value.
        /// </summary>
        /// <param name="left">The first expression to compare.</param>
        /// <param name="right">The first expression to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Expression<T>? left, Expression<T>? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (right is null || left is null)
            {
                return false;
            }

            return left.SequenceEqual(right);
        }

        /// <summary>
        /// Determines whether two specified expressions don't have the same value.
        /// </summary>
        /// <param name="left">The first expression to compare.</param>
        /// <param name="right">The first expression to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(Expression<T>? left, Expression<T>? right)
        {
            return !(left == right);
        }

        #endregion
    }
}

