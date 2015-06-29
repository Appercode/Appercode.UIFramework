using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals.CollectionView
{
    internal class ViewTable
    {
        private Dictionary<WeakRefKey, ViewRecord> cvsToViewRecord = new Dictionary<WeakRefKey, ViewRecord>();

        public ViewTable()
        {
        }

        internal ViewRecord this[object o]
        {
            get
            {
                ViewRecord viewRecord = null;
                this.cvsToViewRecord.TryGetValue(new WeakRefKey(o), out viewRecord);
                return viewRecord;
            }
            set
            {
                this.cvsToViewRecord[new WeakRefKey(o)] = value;
            }
        }
    }
}
