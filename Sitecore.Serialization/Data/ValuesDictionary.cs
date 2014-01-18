using System;
using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Serialization.Data
{

    public class ValuesDictionary
    {
        private readonly Hashtable _values = new Hashtable();

        /// <summary>
        /// Gets the <see cref="T:System.String"/> with the specified id.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The <see cref="T:System.String"/>.
        /// </value>
        public string this[string id]
        {
            get
            {
                return _values[id] as string ?? string.Empty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Serialization.ObjectModel.ValuesDictionary"/> class.
        /// 
        /// </summary>
        /// <param name="version">The version.</param>
        public ValuesDictionary(SerializedVersion version)
            : this(version.Fields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Serialization.ObjectModel.ValuesDictionary"/> class.
        /// 
        /// </summary>
        /// <param name="fields">List of fields to create ValuesDictionary from</param>
        public ValuesDictionary(IEnumerable<SerializedField> fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields", "fields argument must not be null");
            foreach (SerializedField field in fields)
                Add(field);
        }

        /// <summary>
        /// Adds the specified field.
        /// 
        /// </summary>
        /// <param name="field">The field.</param>
        public void Add(SerializedField field)
        {
            _values[field.FieldID] = field.FieldValue;
        }
    }
}
