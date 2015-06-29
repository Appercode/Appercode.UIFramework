using Appercode.UI.Controls;
using System;
using System.Windows;

namespace Appercode.UI.Internals
{
    internal static class StyleHelper
    {   
        internal static void SealIfSealable(object p)
        {
            var sealeble = p as ISealable;
            if (sealeble != null && sealeble.CanSeal)
            {
                sealeble.Seal();
            }
        }

        internal static int CreateChildIndexFromChildName(string p, FrameworkTemplate frameworkTemplate)
        {
            throw new NotImplementedException();
        }

        internal static bool IsStylingLogicalTree(DependencyProperty dp, object value)
        {
            throw new NotImplementedException();
        }

        internal static void DoTemplateInvalidations(ContentPresenter contentPresenter, DataTemplate template)
        {
            // throw new NotImplementedException();
        }

        internal static void UpdateTemplateCache(ContentPresenter contentPresenter, FrameworkTemplate frameworkTemplate1, FrameworkTemplate frameworkTemplate2, DependencyProperty dependencyProperty)
        {
            // throw new NotImplementedException();
        }

        internal static void AddCustomTemplateRoot(ContentPresenter container, TextBlock textBlock, bool p1, bool p2)
        {
            throw new NotImplementedException();
        }

        internal static void AddCustomTemplateRoot(Appercode.UI.Controls.UIElement container, Appercode.UI.Controls.UIElement uiElement)
        {
            throw new NotImplementedException();
        }
    }
}