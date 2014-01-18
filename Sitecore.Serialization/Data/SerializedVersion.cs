using System.Collections.Generic;

namespace Sitecore.Serialization.Data
{
    public class SerializedVersion
    {
        private ValuesDictionary _values;

        /// <summary>
        /// Gets or sets the language.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The language.
        /// </value>
        public string Language
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the version.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The version.
        /// </value>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the revision.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The revision.
        /// </value>
        public string Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the fields.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The fields.
        /// </value>
        public IList<SerializedField> Fields
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the values.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The values.
        /// </value>
        public ValuesDictionary Values
        {
            get { return _values ?? (_values = new ValuesDictionary(this)); }
        }

        public SerializedVersion()
        {
            Fields = new List<SerializedField>();
        }
    }
}
