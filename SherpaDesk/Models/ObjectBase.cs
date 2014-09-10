using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace SherpaDesk.Models
{
    [DataContract]
    public abstract class ObjectBase : object
    {
        #region Constants

        private const string TEMPLATE_PROPERTY_TEXT = "    {0} = {1}; {2}";
        private const string TEMPLATE_ARRAY_ITEM_TEXT = "{0}";
        private const string TEMPLATE_START_ARRAY = " [";
        private const string TEMPLATE_END_ARRAY = "] ";
        private const string TEMPLATE_START_TEXT = "{0} {{{1}";
        private const string TEMPLATE_END_TEXT = "}";
        private const string COMMA = ", ";
        private const string NULL = "NULL";

        #endregion

        #region Methods

        public override string ToString()
        {
            var result = new StringBuilder();

            var type = GetType();

            result.AppendFormat(TEMPLATE_START_TEXT, type.Name, Environment.NewLine);

            foreach (PropertyInfo prop in type.GetRuntimeProperties())
            {
                var details = prop.GetCustomAttribute<DetailsAttribute>(true);
                if(details == null)
                    continue;
                if (!string.IsNullOrEmpty(details.Text))
                {
                    result.AppendFormat(TEMPLATE_PROPERTY_TEXT,
                       prop.Name,
                       details.Text,
                       Environment.NewLine);
                }
                else
                {
                    if (prop.PropertyType != typeof(string)
                        && (prop.PropertyType.IsArray
                        || prop.PropertyType.GetTypeInfo().ImplementedInterfaces.Any(i => typeof(IEnumerable) == i)))
                    {
                        var list = prop.GetValue(this, null) as IEnumerable;

                        if (list != null)
                        {
                            var parameters = (from object obj in list 
                                                            select string.Format(TEMPLATE_ARRAY_ITEM_TEXT, obj != null ? obj.ToString() : NULL))
                                                        .ToList();
                            result.AppendFormat(TEMPLATE_PROPERTY_TEXT,
                               prop.Name,
                               string.Concat(
                                   TEMPLATE_START_ARRAY,
                                   string.Join(COMMA, parameters),
                                   TEMPLATE_END_ARRAY),
                               Environment.NewLine);
                        }
                        else
                        {
                            result.AppendFormat(TEMPLATE_PROPERTY_TEXT,
                                prop.Name,
                                NULL,
                                Environment.NewLine);
                        }
                    }
                    else
                    {
                        object obj = prop.GetValue(this, null);

                        result.AppendFormat(TEMPLATE_PROPERTY_TEXT,
                            prop.Name,
                            obj != null ? obj.ToString() : NULL,
                            Environment.NewLine);
                    }
                }
            }
            result.Append(TEMPLATE_END_TEXT);

            return result.ToString();

        }
        #endregion
    }

    /// <summary>
    /// The attribute signs mark for tracking in properties and during reflection process 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DetailsAttribute : Attribute
    {
        /// <summary>
        /// Defines the text that will replaced on real data. Uses when need hide some value.
        /// </summary>
        public string Text { get; private set; }

        public DetailsAttribute()
        {
        }
        public DetailsAttribute(string text)
        {
            Text = text;

        }
    }
}
