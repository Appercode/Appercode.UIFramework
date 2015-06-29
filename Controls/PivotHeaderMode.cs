using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appercode.UI.Data;

namespace Appercode.UI.Controls
{
    public static class PivotHeaderModeHelper
    {
        private static DataTemplate _DotsHeaderTemplate;
        /// <summary>
        /// Usage example:
        ///     pivot.HeaderMode = PivotHeaderMode.BottomCentered;
        ///     pivot.HeaderTemplate = PivotHeaderModeHelper.DotsHeaderTemplate;
        /// </summary>
        public static DataTemplate DotsHeaderTemplate
        {
            get
            {
                if (_DotsHeaderTemplate == null)
                {
                    var radioButtonTemplate = new FrameworkElementFactory(typeof (RadioButton));
                    radioButtonTemplate.SetBinding(RadioButton.IsCheckedProperty, new Binding("Parent.IsSelected")
                                                                                    {
                                                                                        RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                                                                        Mode = BindingMode.TwoWay,
                                                                                    });

                    _DotsHeaderTemplate = new DataTemplate()
                                               {
                                                   VisualTree = radioButtonTemplate
                                               };
                }
                return _DotsHeaderTemplate;
            }
        }
    }

    public enum PivotHeaderMode
    {
        Top,
        BottomCentered,
        None,
        BottomCircles,
        TopTabs,
    }
}