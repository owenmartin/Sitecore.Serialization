using System;
using System.Collections;
using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Serialization;
using Sitecore.Data.Serialization.Exceptions;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Serialization.Data;
using Version = Sitecore.Data.Version;

namespace Sitecore.Serialization
{
    /// <summary>
    ///     Implements Item &lt;-&gt; File synchronization
    /// </summary>
    public class ItemSynchronization
    {
        /// <summary>
        ///     Builds the dump item object from item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     The dump item.
        /// </returns>
        public SerializedItem BuildSyncItem(Item item)
        {
            var syncItem = new SerializedItem
            {
                ID = item.ID.ToString(),
                DatabaseName = item.Database.Name,
                ParentID = item.ParentID.ToString(),
                Name = item.Name,
                BranchId = item.BranchId.ToString(),
                TemplateID = item.TemplateID.ToString(),
                TemplateName = item.TemplateName,
                ItemPath = item.Paths.Path
            };
            item.Fields.ReadAll();
            item.Fields.Sort();
            foreach (Field field in item.Fields)
            {
                if (TemplateManager.GetTemplate(item).GetField(field.ID) == null || !field.Shared) continue;
                var fieldValue = GetFieldValue(field);
                if (fieldValue != null)
                    AddSharedField(syncItem, field.ID.ToString(), field.Name, field.Key, fieldValue, true);
            }
            var versions = item.Versions.GetVersions(true);
            Array.Sort(versions, CompareVersions);
            foreach (var version in versions)
                BuildVersion(syncItem, version);
            return syncItem;
        }

        /// <summary>
        /// Adds the shared field.
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldID">The field ID.</param><param name="fieldName">Name of the field.</param><param name="fieldKey">The field key.</param><param name="fieldValue">The field value.</param><param name="hasValue">if set to <c>true</c> this instance is has value.</param>
        /// <returns>
        /// The shared field.
        /// </returns>
        private void AddSharedField(SerializedItem item, string fieldID, string fieldName, string fieldKey, string fieldValue, bool hasValue)
        {
            SerializedField syncField = BuildField(fieldID, fieldName, fieldKey, fieldValue, hasValue);
            if (syncField != null)
                item.SharedFields.Add(syncField);
        }

        /// <summary>
        /// Builds the field.
        /// 
        /// </summary>
        /// <param name="fieldID">The field ID.</param><param name="fieldName">Name of the field.</param><param name="fieldKey">The field key.</param><param name="fieldValue">The field value.</param><param name="hasValue">if set to <c>true</c> this instance is has value.</param>
        /// <returns>
        /// The field.
        /// </returns>
        private SerializedField BuildField(string fieldID, string fieldName, string fieldKey, string fieldValue, bool hasValue)
        {
            if (!hasValue)
                return null;
            return new SerializedField
            {
                FieldID = fieldID,
                FieldName = fieldName,
                FieldKey = fieldKey,
                FieldValue = fieldValue
            };
        }

        private static int CompareVersions(Item left, Item right)
        {
            int num = String.Compare(left.Language.Name, right.Language.Name, StringComparison.Ordinal);
            if (num == 0)
                num = left.Version.Number.CompareTo(right.Version.Number);
            return num;
        }

        /// <summary>
        ///     Builds the version.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="version">The version.</param>
        private void BuildVersion(SerializedItem item, Item version)
        {
            var syncVersion = AddVersion(item, version.Language.ToString(), version.Version.ToString(),
                version.Statistics.Revision);
            if (syncVersion == null)
                return;
            version.Fields.ReadAll();
            version.Fields.Sort();
            foreach (Field field in version.Fields)
            {
                if (TemplateManager.GetTemplate(version).GetField(field.ID) == null || field.Shared) continue;
                var fieldValue = GetFieldValue(field);
                if (fieldValue != null)
                    AddField(syncVersion,field.ID.ToString(), field.Name, field.Key, fieldValue, true);
            }
        }

        /// <summary>
        /// Adds the field.
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="fieldID">The field ID.</param><param name="fieldName">Name of the field.</param><param name="fieldKey">The field key.</param><param name="fieldValue">The field value.</param><param name="hasValue">if set to <c>true</c> this instance is has value.</param>
        /// <returns>
        /// The field.
        /// </returns>
        private void AddField(SerializedVersion version, string fieldID, string fieldName, string fieldKey, string fieldValue, bool hasValue)
        {
            SerializedField field = BuildField(fieldID, fieldName, fieldKey, fieldValue, hasValue);
            if (field != null)
            {
                version.Fields.Add(field);
                if (version.Values != null)
                    version.Values.Add(field);
            }
        }

        /// <summary>
        /// Adds the version.
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="language">The language.</param><param name="version">The version.</param><param name="revision">The revision.</param>
        /// <returns>
        /// The version.
        /// </returns>
        private SerializedVersion AddVersion(SerializedItem item, string language, string version, string revision)
        {
            var syncVersion = new SerializedVersion
            {
                Language = language,

                Revision = revision,
                Version = version
            };
            item.Versions.Add(syncVersion);
            return syncVersion;
        }

        private string GetFieldValue(Field field)
        {
            string str = field.GetValue(false, false);
            if (str == null)
                return null;
            if (!field.IsBlobField)
                return str;
            var blobStream = field.GetBlobStream();
            if (blobStream == null)
                return null;
            using (blobStream)
            {
                var numArray = new byte[blobStream.Length];
                blobStream.Read(numArray, 0, numArray.Length);
                return System.Convert.ToBase64String(numArray, Base64FormattingOptions.InsertLineBreaks);
            }
        }

        /// <summary>
        ///     Pastes SyncItem into the database.
        /// </summary>
        /// <param name="syncItem">The sync item.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        ///     The sync item.
        /// </returns>
        public Item PasteSyncItem(SerializedItem syncItem, LoadOptions options)
        {
            return PasteSyncItem(syncItem, options, false);
        }

        /// <summary>
        ///     Pastes SyncItem into the database.
        /// </summary>
        /// <param name="syncItem">The sync item.</param>
        /// <param name="options">The options.</param>
        /// <param name="failOnDataInconsistency">
        ///     if set to <c>true</c> the method will fail with exception when inconsisntecy
        ///     detected.
        /// </param>
        /// <returns>
        ///     The sync item.
        /// </returns>
        /// <exception cref="T:Sitecore.Data.Serialization.Exceptions.ParentItemNotFoundException">
        ///     <c>ParentItemNotFoundException</c>.
        /// </exception>
        /// <exception cref="T:System.Exception"><c>Exception</c>.</exception>
        /// <exception cref="T:Sitecore.Data.Serialization.Exceptions.ParentForMovedItemNotFoundException">
        ///     <c>ParentForMovedItemNotFoundException</c>.
        /// </exception>
        public Item PasteSyncItem(SerializedItem syncItem, LoadOptions options, bool failOnDataInconsistency)
        {
            if (syncItem == null)
                return null;
            Exception exception = null;
            var database = options.Database ?? Factory.GetDatabase(syncItem.DatabaseName);
            var destination = database.GetItem(syncItem.ParentID);
            var id1 = options.UseNewID ? ID.NewID : ID.Parse(syncItem.ID);
            var target = database.GetItem(id1);
            var options1 = new LoadOptions(options);
            var flag = false;
            if (target == null)
            {
                if (destination == null)
                {
                    if (!failOnDataInconsistency)
                        return null;
                    throw new ParentItemNotFoundException
                    {
                        ParentID = syncItem.ParentID,
                        ItemID = syncItem.ID
                    };
                }
                var id2 = ID.Parse(syncItem.TemplateID);
                AssertTemplate(database, id2);
                target = ItemManager.AddFromTemplate(syncItem.Name, id2, destination, id1);
                target.Versions.RemoveAll(true);
                options1.ForceUpdate = true;
                flag = true;
            }
            else
            {
                if (!options1.ForceUpdate)
                    options1.ForceUpdate = NeedUpdate(target, syncItem);
                if (options1.ForceUpdate)
                {
                    if (destination == null && failOnDataInconsistency)
                        exception = new ParentForMovedItemNotFoundException
                        {
                            ParentID = syncItem.ParentID,
                            Item = target
                        };
                    if (destination != null && destination.ID != target.ParentID)
                        target.MoveTo(destination);
                }
            }
            try
            {
                if (options1.ForceUpdate)
                {
                    //If templates do not match
                    if (target.TemplateID.ToString() != syncItem.TemplateID)
                    {
                        using (new EditContext(target))
                        {
                            target.RuntimeSettings.ReadOnlyStatistics = true;
                            target.ChangeTemplate(target.Database.Templates[ID.Parse(syncItem.TemplateID)]);
                        }
                        if (EventDisabler.IsActive)
                            RemoveItemFromCaches(database, target.ID);
                        target.Reload();
                    }
                    //If the item's name or branch Id has changed
                    if (target.Name != syncItem.Name || target.BranchId.ToString() != syncItem.BranchId)
                    {
                        using (new EditContext(target))
                        {
                            target.RuntimeSettings.ReadOnlyStatistics = true;
                            target.Name = syncItem.Name;
                            target.BranchId = ID.Parse(syncItem.BranchId);
                        }
                        ClearCaches(target.Database, target.ID);
                        target.Reload();
                    }
                    ResetTemplateEngine(target);
                    using (new EditContext(target))
                    {
                        target.RuntimeSettings.ReadOnlyStatistics = true;
                        target.RuntimeSettings.SaveAll = true;
                        if (options.ForceUpdate)
                        {
                            foreach (Field field in target.Fields)
                            {
                                if (field.Shared)
                                    field.Reset();
                            }
                        }
                        foreach (SerializedField field in syncItem.SharedFields)
                            PasteField(target, field);
                    }
                    ClearCaches(database, id1);
                    target.Reload();
                    ResetTemplateEngine(target);
                }
                Hashtable ciHashtable = CommonUtils.CreateCIHashtable();
                if (options1.ForceUpdate)
                {
                    foreach (Item obj in target.Versions.GetVersions(true))
                        ciHashtable[obj.Uri] = null;
                }
                foreach (SerializedVersion syncVersion in syncItem.Versions)
                    PasteVersion(target, syncVersion, ciHashtable, options1);
                if (options1.ForceUpdate)
                {
                    foreach (ItemUri uri in ciHashtable.Keys)
                    {
                        if (options.Database != null)
                            options.Database.GetItem(uri.ToDataUri()).Versions.RemoveVersion();
                        else
                            Database.GetItem(uri).Versions.RemoveVersion();
                    }
                }
                ClearCaches(target.Database, target.ID);
                if (failOnDataInconsistency && exception != null)
                    throw exception;
                return target;
            }
            catch (ParentForMovedItemNotFoundException)
            {
                throw;
            }
            catch (ParentItemNotFoundException)
            {
                throw;
            }
            catch (FieldIsMissingFromTemplateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (flag)
                {
                    target.Delete();
                    ClearCaches(database, id1);
                }
                throw new Exception("Failed to paste item: " + syncItem.ItemPath, ex);
            }
        }

        /// <summary>
        ///     Checks whether an item should be updated
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="syncItem">The sync item.</param>
        /// <returns>
        ///     The update.
        /// </returns>
        private bool NeedUpdate(Item item, SerializedItem syncItem)
        {
            return
                syncItem.Versions.Any(
                    version =>
                        NeedUpdate(
                            item.Database.GetItem(item.ID, Language.Parse(version.Language),
                                Version.Parse(version.Version)), version));
        }

        /// <summary>
        ///     Needs the update.
        /// </summary>
        /// <param name="localVersion">The local version.</param>
        /// <param name="version">The version.</param>
        /// <returns>
        ///     The update.
        /// </returns>
        private bool NeedUpdate(Item localVersion, SerializedVersion version)
        {
            if (localVersion == null)
                return true;
            return
                string.Compare(version.Values[FieldIDs.Updated.ToString()], localVersion[FieldIDs.Updated.ToString()],
                    StringComparison.InvariantCulture) > 0;
        }

        /// <summary>
        ///     Pastes single version from ItemDom into the item
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="syncVersion">The sync version.</param>
        /// <param name="versions">The versions.</param>
        /// <param name="options">The loading options.</param>
        private void PasteVersion(Item item, SerializedVersion syncVersion, Hashtable versions, LoadOptions options)
        {
            Language language = Language.Parse(syncVersion.Language);
            Version index = Version.Parse(syncVersion.Version);
            Item obj1 = item.Database.GetItem(item.ID, language);
            Item obj2 = obj1.Versions[index];
            if (!options.ForceUpdate && !NeedUpdate(obj2, syncVersion))
                return;
            if (obj2 == null)
                obj2 = obj1.Versions.AddVersion();
            else
                versions.Remove(obj2.Uri);
            if (!options.ForceUpdate && obj2.Statistics.Revision == syncVersion.Revision)
                return;
            using (new EditContext(obj2))
            {
                obj2.RuntimeSettings.ReadOnlyStatistics = true;
                if (options.ForceUpdate)
                {
                    if (obj2.Versions.Count == 0)
                        obj2.Fields.ReadAll();
                    foreach (Field field in obj2.Fields)
                    {
                        if (!field.Shared)
                            field.Reset();
                    }
                }
                bool flag = false;
                foreach (SerializedField field in syncVersion.Fields)
                {
                    ID result;
                    if (ID.TryParse(field.FieldID, out result) && result == FieldIDs.Owner)
                        flag = true;
                    PasteField(obj2, field);
                }
                if (!flag)
                    obj2.Fields[FieldIDs.Owner].Reset();
            }
            ClearCaches(obj2.Database, obj2.ID);
            ResetTemplateEngine(obj2);
        }

        /// <summary>
        ///     Resets the template engine.
        /// </summary>
        /// <param name="target">The target.</param>
        private void ResetTemplateEngine(Item target)
        {
            if (!target.Database.Engines.TemplateEngine.IsTemplatePart(target))
                return;
            target.Database.Engines.TemplateEngine.Reset();
        }

        /// <summary>
        ///     Inserts field value into item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="field">The field.</param>
        /// <exception cref="T:Sitecore.Data.Serialization.Exceptions.FieldIsMissingFromTemplateException" />
        private void PasteField(Item item, SerializedField field)
        {
            Template template = AssertTemplate(item.Database, item.TemplateID);
            if (template.GetField(field.FieldID) == null)
            {
                item.Database.Engines.TemplateEngine.Reset();
                template = AssertTemplate(item.Database, item.TemplateID);
            }
            if (template.GetField(field.FieldID) == null)
            {
                throw new FieldIsMissingFromTemplateException(
                    "Field '" + field.FieldName + "' does not exist in template '" + template.Name + "'",
                    FileUtil.MakePath(item.Template.InnerItem.Database.Name, item.Template.InnerItem.Paths.FullPath),
                    FileUtil.MakePath(item.Database.Name, item.Paths.FullPath), item.ID);
            }
            Field field1 = item.Fields[ID.Parse(field.FieldID)];
            if (field1.IsBlobField && !MainUtil.IsID(field.FieldValue))
            {
                byte[] buffer = System.Convert.FromBase64String(field.FieldValue);
                field1.SetBlobStream(new MemoryStream(buffer, false));
            }
            else
                field1.SetValue(field.FieldValue, true);
        }

        /// <summary>
        ///     Asserts that the template is present in the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="template">The template.</param>
        /// <returns>
        ///     The template being asserted
        /// </returns>
        private Template AssertTemplate(Database database, ID template)
        {
            Template template1 = database.Engines.TemplateEngine.GetTemplate(template);
            if (template1 == null)
            {
                database.Engines.TemplateEngine.Reset();
                template1 = database.Engines.TemplateEngine.GetTemplate(template);
            }
            Assert.IsNotNull(template1, "Template: " + template + " not found");
            return template1;
        }

        /// <summary>
        /// Removes information about a specific item from database caches. This compensates
        ///             cache functionality that depends on database events (which are disabled when loading).
        /// 
        /// </summary>
        /// <param name="database">Database to clear caches for.
        ///             </param><param name="itemID">Item ID to remove
        ///             </param>
        private void ClearCaches(Database database, ID itemID)
        {
            if (!EventDisabler.IsActive)
                return;
            database.Caches.ItemCache.RemoveItem(itemID);
            database.Caches.DataCache.RemoveItemInformation(itemID);
        }

        /// <summary>
        /// Removes the item from caches.
        /// 
        /// </summary>
        /// <param name="database">The database.
        ///             </param><param name="itemId">The item ID.
        ///             </param>
        private void RemoveItemFromCaches(Database database, ID itemId)
        {
            database.Caches.ItemCache.RemoveItem(itemId);
            database.Caches.DataCache.RemoveItemInformation(itemId);
        }
    }
}