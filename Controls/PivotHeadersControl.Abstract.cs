using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appercode.UI.Controls
{
    public partial class PivotHeadersControl
    {
    }

    public partial class CirclePivotHeaderControl : Control, IPivotHeaderControl
    {
        public IEnumerable ItemsSource { get; set; }

        public int SelectedIndex { get; set; }
    }
}
