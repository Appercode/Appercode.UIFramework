using System;

namespace Appercode.UI.Controls.Navigation
{
    public interface IFrameStyler
    {
        void StyleNavBar(StackNavigationFrame stackNavigationFrame);

        void StyleTabBar(TabsNavigationFrame tabsNavigationFrame);
    }
}