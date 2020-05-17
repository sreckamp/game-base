using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GameBase.WPF
{
    public class IfContainsConverter : MarkupExtension, IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (values[1] is IEnumerable e) && e.Cast<object>().Contains(values[0])
                ? (values.Length > 2 ? values[2] : DependencyProperty.UnsetValue)
                : (values.Length > 3 ? values[3] : DependencyProperty.UnsetValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class ArrayListConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            if (value is IEnumerable enumerable)
            {
                var comma = false;
                foreach (var val in enumerable)
                {
                    if (comma) sb.Append(',');
                    sb.Append(val);
                    comma = true;
                }
            }
            sb.Append(']');
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class PassThroughConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IfElseConverter : MarkupExtension, IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] is bool bv && bv || values[0] != DependencyProperty.UnsetValue
                ? (values.Length > 1 ? values[1] : DependencyProperty.UnsetValue)
                : (values.Length > 2 ? values[2] : DependencyProperty.UnsetValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class ChooseOneConverter : MarkupExtension, IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var foundNull = false;
            foreach (var t in values)
            {
                if (t == DependencyProperty.UnsetValue) continue;
                if (t != null) return t;
                foundNull = true;
            }

            return foundNull ? null : DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// Process formula of the format var1,var2:var1||var2
    /// Also processes var1,var2:var1&amp;&amp;var2?true:false
    /// </summary>
    public class BooleanFormulaConverter : AbstractFormulaConverter<bool>
    {
        private static readonly BooleanStringFormulaParser SParser = new BooleanStringFormulaParser();

        public BooleanFormulaConverter() : base(SParser) { }

        #region IValueConverter Members

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            parameter = parameter ?? "A:A";
            SplitParameter(parameter.ToString(), out var names, out var formula);

            var idx = formula.IndexOf("?");
            string trueName = null;
            object trueValue = null;
            string falseName = null;
            object falseValue = null;
            if (idx >= 0)
            {
                var results = formula.Substring(idx + 1).Split(':');
                if (results.Length != 2
                    || string.Empty.Equals(results[0]) || string.Empty.Equals(results[1]))
                {
                    throw new ArgumentException("Result syntax is ?TrueName:FalseName.");
                }
                formula = formula.Substring(0, idx);
                trueName = results[0];
                falseName = results[1];
            }

            var rawData = new Dictionary<string, object>();
            var data = PrepareValues(names, values, rawData);
            if (trueName != null && rawData.ContainsKey(trueName))
            {
                trueValue = rawData[trueName];
            }
            if (falseName != null && rawData.ContainsKey(falseName))
            {
                falseValue = rawData[falseName];
            }
            if (trueValue == null)
            {
                if (targetType == typeof(bool))
                {
                    trueValue = true;
                }
                else if (targetType == typeof(Visibility))
                {
                    trueValue = Visibility.Visible;
                }
                else
                {
                    trueValue = DependencyProperty.UnsetValue;
                }
            }
            if (falseValue == null)
            {
                if (targetType == typeof(bool))
                {
                    falseValue = false;
                }
                else if (targetType == typeof(Visibility))
                {
                    falseValue = Visibility.Hidden;
                }
                else
                {
                    falseValue = DependencyProperty.UnsetValue;
                }
            }
            var result = Calculate(formula, data);
            var retVal = result ? trueValue : falseValue;
            return retVal;
        }

        #endregion

        protected override bool ObjectToTypedValue(object value)
        {
            var val = false;
            if (value is Double)
            {
                val = !((Double)value).Equals(0);
            }
            else if (value is bool)
            {
                val = (bool)value;
            }
            else if (value is Visibility)
            {
                val = (Visibility)value == Visibility.Visible;
            }
            else if (value != null && value != DependencyProperty.UnsetValue)
            {
                val = true;
            }
            return val;
        }
    }

    public class DoubleFormulaConverter : AbstractFormulaConverter<double>
    {
        private static readonly MathStringFormulaParser<double> SParser = new MathStringFormulaParser<double>();

        public DoubleFormulaConverter() : base(SParser) { }

        protected override double ObjectToTypedValue(object value)
        {
            double val = 0;
            if (value is Double || value is int)
            {
                val = (double)value;
            }
            else if (value is string str)
            {
                double.TryParse(str, out val);
            }
            return val;
        }
    }

    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    public class IntegerFormulaConverter : AbstractFormulaConverter<int>
    {
        private static readonly MathStringFormulaParser<int> SParser = new MathStringFormulaParser<int>();

        public IntegerFormulaConverter() : base(SParser) { }

        //public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (parameter == null) parameter = "A=>A";
        //    string[] names;
        //    string formula;
        //    splitParameter(parameter.ToString(), out names, out formula);
        //    var data = prepareValues(names, values);
        //    var result = calculate(formula, data);
        //    Debug.WriteLine(formula + "=" + result.ToString());
        //    return result;
        //}

        protected override int ObjectToTypedValue(object value)
        {
            var val = 0;
            if (value is double)
            {
                val = (int)Math.Round((double)value,0);
            }
            else if (value is int)
            {
                val = (int)value;
            }
            else if (value is string)
            {
                try
                {
                    val = int.Parse((string)value);
                }
                catch (FormatException) { }
            }
            return val;
        }
    }

    public class ObjectVisibilityConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return Visibility.Visible;
                }
            }
            else if (value != null && value != DependencyProperty.UnsetValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class EqualityConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value?.Equals(parameter) ?? parameter == null;
            if (targetType == typeof(Visibility))
            {
                return result ? Visibility.Visible : Visibility.Hidden;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    /// <summary>
    /// Process formula of the format var1,var2=>var1||var2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractFormulaConverter<T> : MarkupExtension, IMultiValueConverter where T:struct
    {
        private readonly StringFormulaParser<T> m_parser;

        protected AbstractFormulaConverter(StringFormulaParser<T> parser)
        {
            m_parser = parser;
        }

        #region IValueConverter Members

        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) parameter = "A:A";
            SplitParameter(parameter.ToString(), out var names, out var formula);
            var data = PrepareValues(names, values);
            return Calculate(formula, data);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected void SplitParameter(string parameter, out string[] names, out string formula)
        {
            var temp = parameter.ToUpper().Replace(" ", "");

            if (!temp.Contains(":"))
            {
                throw new ArgumentException("Expected format is variableList=>formula.");
            }
            var idx = temp.IndexOf(":");
            formula = temp.Substring(idx + ":".Length);
            names = temp.Substring(0, idx).Split(',');
            if (names.Length == 1 && string.Empty.Equals(names[0]))
            {
                names = new string[0];
            }
        }

        protected Dictionary<string, T> PrepareValues(string[] names, object[] values)
        {
            return PrepareValues(names, values, null);
        }

        protected Dictionary<string, T> PrepareValues(string[] names, object[] values, Dictionary<string, object> rawData)
        {
            if (names.Length > values.Length)
            {
                throw new ArgumentException($"Expected {names.Length} arguments, received {values.Length}.");
            }
            var data = new Dictionary<string, T>();
            for (var i = 0; i < names.Length; i++)
            {
                if (string.Empty.Equals(names[i]))
                {
                    throw new ArgumentException("Invalid empty variable name.");
                }
                if (m_parser.IsConstant(names[i]))
                {
                    throw new ArgumentException("Variable cannot have same name as constant: " + names[i]);
                }
                if (data.ContainsKey(names[i]))
                {
                    throw new ArgumentException("Duplicate variable name: " + names[i]);
                }
                data[names[i]] = ObjectToTypedValue(values[i]);
                if (rawData != null)
                {
                    rawData[names[i]] = values[i];
                }
            }
            return data;
        }

        protected abstract T ObjectToTypedValue(object value);

        protected T Calculate(string formula, Dictionary<string, T> values)
        {
            var p = m_parser.BuildProcessor(formula);
            return p.Calculate(values);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
