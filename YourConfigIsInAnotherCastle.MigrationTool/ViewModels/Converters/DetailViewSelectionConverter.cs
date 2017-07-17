using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YourConfigIsInAnotherCastle.MigrationTool.ViewModels.Converters
{
    public enum ViewFormat
    {
        Json,
        XML,
        JsonSchema,
        XSD,
    }
    //TODO this could more generic in that the second parameter is not a enum, but instead a path?
    //There might be an existing way to do this without a converter.
    /// <summary>
    /// This is used to convert to and from properties of an object based on the View Format passed in as the second parameter.
    /// </summary>
    public class ViewFormatConverter : IMultiValueConverter
    {
     
        private SeletableSectionMigratedViewModel _previousValue;
        private ViewFormat _previousType;

        /// <summary>
        /// Converts SeletableSectionMigratedViewModel to one of its data property values
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] != null)
            {
                var actualValue = (SeletableSectionMigratedViewModel)values[0];
                var detailViewType = (ViewFormat)Enum.Parse(typeof(ViewFormat), values[1].ToString().Replace(" ", string.Empty), true);
                string results = null;
                switch (detailViewType)
                {
                    case ViewFormat.Json:
                        results = actualValue.JsonDetails.RawData;
                        break;
                    case ViewFormat.JsonSchema:
                        results = actualValue.JsonDetails.Schema;
                        break;
                    case ViewFormat.XML:
                        results = actualValue.XmlDetails.RawData;
                        break;
                    case ViewFormat.XSD:
                        results = actualValue.XmlDetails.Schema;
                        break;
                }
                _previousType = detailViewType;
                _previousValue = actualValue;
                return results;
            }
            return null;
        }
        /// <summary>
        /// Converts string to one of SeletableSectionMigratedViewModel's data property values
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            string actualValue = (string)value;
            if (_previousValue != null)
            {
                var detailViewType = _previousType;
                switch (detailViewType)
                {
                    case ViewFormat.Json:
                        _previousValue.JsonDetails.RawData = actualValue;
                        break;
                    case ViewFormat.JsonSchema:
                        _previousValue.JsonDetails.Schema = actualValue;
                        break;
                    case ViewFormat.XML:
                        _previousValue.XmlDetails.RawData = actualValue;
                        break;
                    case ViewFormat.XSD:
                        _previousValue.XmlDetails.Schema = actualValue;
                        break;
                }
            }
            return new object[] { _previousValue, _previousType };
        }

    }
}
