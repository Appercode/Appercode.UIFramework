using System.ComponentModel;

namespace Appercode.UI.Internals.CollectionView
{
    internal class ViewRecord
    {
        internal ViewRecord(ICollectionView view)
        {
            this.View = view;
            this.IsInitialized = false;
            this.Version = -1;
        }
        
        internal bool IsInitialized
        {
            get;
            set;
        }

        internal int Version
        {
            get;
            set;
        }

        internal ICollectionView View
        {
            get;
            private set;
        }
    }
}
