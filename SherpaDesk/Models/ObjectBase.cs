using System;
using System.Collections;
using System.Collections.Generic;
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
            StringBuilder result = new StringBuilder();

            var type = this.GetType();

            result.AppendFormat(TEMPLATE_START_TEXT, type.Name, Environment.NewLine);

            foreach (PropertyInfo prop in type.GetRuntimeProperties())
            {
                if (!prop.IsDefined(typeof(DetailsAttribute))) continue;

                var details = prop.GetCustomAttributes<DetailsAttribute>(true).ToList();
                if (details != null
                    && details.Count > 0
                    && !string.IsNullOrEmpty(((DetailsAttribute)details[0]).Text))
                {
                    result.AppendFormat(TEMPLATE_PROPERTY_TEXT,
                       prop.Name,
                       ((DetailsAttribute)details[0]).Text,
                       Environment.NewLine);
                }
                else
                {
                    if (!prop.PropertyType.Equals(typeof(string))
                        && (prop.PropertyType.IsArray
                            || prop.IsDefined(typeof(IEnumerable), true)))
                    {
                        IEnumerable list = prop.GetValue(this, null) as IEnumerable;

                        if (list != null)
                        {
                            IList<string> parameters = new List<string>();
                            foreach (object obj in list)
                            {
                                parameters.Add(
                                    string.Format(TEMPLATE_ARRAY_ITEM_TEXT,
                                        obj != null ? obj.ToString() : NULL));
                            }
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
            this.Text = text;

        }
    }
}
