using System.Collections.Generic;

namespace Sitecore.Serialization.Data
{
    public class SerializedItem
    {
        /// <summary>
        /// Gets or sets the item ID.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the database.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The name of the database.
        /// </value>
        public string DatabaseName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent ID.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The parent ID.
        /// </value>
        public string ParentID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the master ID.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The master ID.
        /// </value>
        public string MasterID
        {
            get
            {
                return BranchId;
            }
            set
            {
                BranchId = value;
            }
        }

        /// <summary>
        /// Gets or sets the branch id.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The branch id.
        /// </value>
        public string BranchId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the template ID.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The template ID.
        /// </value>
        public string TemplateID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the template.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The name of the template.
        /// </value>
        public string TemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the shared fields.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The shared fields.
        /// </value>
        public IList<SerializedField> SharedFields
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the versions.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The versions.
        /// </value>
        public IList<SerializedVersion> Versions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item path.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The item path.
        /// </value>
        public string ItemPath
        {
            get;
            set;
        }

        public SerializedItem()
        {
            Versions = new List<SerializedVersion>();
            SharedFields = new List<SerializedField>();
        }
    }
}
