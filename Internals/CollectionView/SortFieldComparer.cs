using Appercode.UI.Internals.PathParser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Appercode.UI.Internals.CollectionView
{
    internal class SortFieldComparer : IComparer<object>
    {
        private SortFieldComparer.SortPropertyInfo[] fields;

        private SortDescriptionCollection sortFields;

        private CultureInfo culture;

        internal SortFieldComparer(SortDescriptionCollection sortFields, CultureInfo culture)
        {
            this.sortFields = sortFields;
            this.fields = this.CreatePropertyInfo(this.sortFields);
            this.culture = culture;
        }

        public int Compare(object o1, object o2)
        {
            int num = 0;
            for (int i = 0; i < (int)this.fields.Length; i++)
            {
                object value = this.fields[i].GetValue(o1);
                object obj = this.fields[i].GetValue(o2);
                num = this.CompareInternal(value, obj);
                if (this.fields[i].Descending)
                {
                    num = -num;
                }
                if (num != 0)
                {
                    break;
                }
            }
            return num;
        }

        private int CompareInternal(object obj1, object obj2)
        {
            if (obj1 is string && obj1 is string)
            {
                string.Compare((string)obj1, (string)obj2, this.culture, CompareOptions.None);
            }
            if (obj1 is IComparable)
            {
                return ((IComparable)obj1).CompareTo(obj2);
            }
            if (obj2 is IComparable)
            {
                return ((IComparable)obj1).CompareTo(obj2);
            }
            throw new ArgumentException("Both arguments is not IComparable");
        }

        private SortFieldComparer.SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortFields)
        {
            PropertyPath propertyPath;
            SortFieldComparer.SortPropertyInfo[] direction = new SortFieldComparer.SortPropertyInfo[sortFields.Count];
            for (int i = 0; i < sortFields.Count; i++)
            {
                if (!string.IsNullOrEmpty(sortFields[i].PropertyName))
                {
                    SortDescription item = sortFields[i];
                    propertyPath = new PropertyPath(item.PropertyName, new object[0]);
                    propertyPath.ParsePathInternal(false);
                }
                else
                {
                    propertyPath = null;
                }
                direction[i].Info = propertyPath;
                SortDescription sortDescription = sortFields[i];
                direction[i].Descending = sortDescription.Direction == ListSortDirection.Descending;
            }
            return direction;
        }

        private struct SortPropertyInfo
        {
            internal PropertyPath Info;

            internal bool Descending;

            internal object GetValue(object o)
            {
                object obj;
                if (this.Info != null)
                {
                    PropertyPathListener listener = this.Info.GetListener(o, false, null);
                    obj = !listener.FullPathExists ? null : listener.LeafValue;
                }
                else
                {
                    obj = o;
                }
                return obj;
            }
        }
    }
}