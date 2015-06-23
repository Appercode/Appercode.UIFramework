using Appercode.UI.StylesAndResources;
using System.Collections.Generic;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal struct ResourcesChangeInfo
    {
        private List<ResourceDictionary> oldDictionaries;

        private List<ResourceDictionary> newDictionaries;

        private object key;

        private DependencyObject container;

        private ResourcesChangeInfo.PrivateFlags flags;

        internal ResourcesChangeInfo(object key)
        {
            this.oldDictionaries = null;
            this.newDictionaries = null;
            this.key = key;
            this.container = null;
            this.flags = (ResourcesChangeInfo.PrivateFlags)0;
        }

        internal ResourcesChangeInfo(ResourceDictionary oldDictionary, ResourceDictionary newDictionary)
        {
            this.oldDictionaries = null;
            if (oldDictionary != null)
            {
                this.oldDictionaries = new List<ResourceDictionary>(1);
                this.oldDictionaries.Add(oldDictionary);
            }
            this.newDictionaries = null;
            if (newDictionary != null)
            {
                this.newDictionaries = new List<ResourceDictionary>(1);
                this.newDictionaries.Add(newDictionary);
            }
            this.key = null;
            this.container = null;
            this.flags = (ResourcesChangeInfo.PrivateFlags)0;
        }

        internal ResourcesChangeInfo(List<ResourceDictionary> oldDictionaries, List<ResourceDictionary> newDictionaries, bool isStyleResourcesChange, bool isTemplateResourcesChange, DependencyObject container)
        {
            this.oldDictionaries = oldDictionaries;
            this.newDictionaries = newDictionaries;
            this.key = null;
            this.container = container;
            this.flags = (ResourcesChangeInfo.PrivateFlags)0;
            this.IsStyleResourcesChange = isStyleResourcesChange;
            this.IsTemplateResourcesChange = isTemplateResourcesChange;
        }

        private enum PrivateFlags : byte
        {
            IsThemeChange = 1,
            IsTreeChange = 2,
            IsStyleResourceChange = 4,
            IsTemplateResourceChange = 8,
            IsSysColorsOrSettingsChange = 16,
            IsCatastrophicDictionaryChange = 32
        }
        internal static ResourcesChangeInfo CatastrophicDictionaryChangeInfo
        {
            get
            {
                ResourcesChangeInfo resourcesChangeInfo = new ResourcesChangeInfo();
                resourcesChangeInfo.IsCatastrophicDictionaryChange = true;
                return resourcesChangeInfo;
            }
        }

        internal static ResourcesChangeInfo SysColorsOrSettingsChangeInfo
        {
            get
            {
                ResourcesChangeInfo resourcesChangeInfo = new ResourcesChangeInfo();
                resourcesChangeInfo.IsSysColorsOrSettingsChange = true;
                return resourcesChangeInfo;
            }
        }

        internal static ResourcesChangeInfo ThemeChangeInfo
        {
            get
            {
                ResourcesChangeInfo resourcesChangeInfo = new ResourcesChangeInfo();
                resourcesChangeInfo.IsThemeChange = true;
                return resourcesChangeInfo;
            }
        }

        internal static ResourcesChangeInfo TreeChangeInfo
        {
            get
            {
                ResourcesChangeInfo resourcesChangeInfo = new ResourcesChangeInfo();
                resourcesChangeInfo.IsTreeChange = true;
                return resourcesChangeInfo;
            }
        }

        internal DependencyObject Container
        {
            get
            {
                return this.container;
            }
        }

        internal bool IsCatastrophicDictionaryChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsCatastrophicDictionaryChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsCatastrophicDictionaryChange, value);
            }
        }

        internal bool IsResourceAddOperation
        {
            get
            {
                if (this.key != null)
                {
                    return true;
                }
                if (this.newDictionaries == null)
                {
                    return false;
                }
                return this.newDictionaries.Count > 0;
            }
        }

        internal bool IsStyleResourcesChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsStyleResourceChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsStyleResourceChange, value);
            }
        }

        internal bool IsSysColorsOrSettingsChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsSysColorsOrSettingsChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsSysColorsOrSettingsChange, value);
            }
        }

        internal bool IsTemplateResourcesChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsTemplateResourceChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsTemplateResourceChange, value);
            }
        }

        internal bool IsThemeChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsThemeChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsThemeChange, value);
            }
        }

        internal bool IsTreeChange
        {
            get
            {
                return this.ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags.IsTreeChange);
            }
            set
            {
                this.WritePrivateFlag(ResourcesChangeInfo.PrivateFlags.IsTreeChange, value);
            }
        }

        internal bool Contains(object key, bool isImplicitStyleKey)
        {
            if (this.IsTreeChange || this.IsCatastrophicDictionaryChange)
            {
                return true;
            }
            if (this.IsThemeChange || this.IsSysColorsOrSettingsChange)
            {
                return !isImplicitStyleKey;
            }
            if (this.key != null && object.Equals(this.key, key))
            {
                return true;
            }
            if (this.oldDictionaries != null)
            {
                for (int i = 0; i < this.oldDictionaries.Count; i++)
                {
                    if (this.oldDictionaries[i].Contains(key))
                    {
                        return true;
                    }
                }
            }
            if (this.newDictionaries != null)
            {
                for (int j = 0; j < this.newDictionaries.Count; j++)
                {
                    if (this.newDictionaries[j].Contains(key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ReadPrivateFlag(ResourcesChangeInfo.PrivateFlags bit)
        {
            return (byte)(this.flags & bit) != 0;
        }

        private void WritePrivateFlag(ResourcesChangeInfo.PrivateFlags bit, bool value)
        {
            if (value)
            {
                ResourcesChangeInfo resourcesChangeInfo = this;
                resourcesChangeInfo.flags = (ResourcesChangeInfo.PrivateFlags)((byte)(resourcesChangeInfo.flags | bit));
                return;
            }
            ResourcesChangeInfo resourcesChangeInfo1 = this;
            resourcesChangeInfo1.flags = (ResourcesChangeInfo.PrivateFlags)((byte)((byte)resourcesChangeInfo1.flags & (byte)(~bit)));
        }
    }
}
