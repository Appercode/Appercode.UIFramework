using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Appercode.UI.Controls.Navigation
{
    internal class PageFactory
    {
        internal static AppercodePage InstantiatePage(Type pageType, ref bool isNavigationInProgress)
        {
            var pageConstructorInfo = pageType.GetConstructor(new Type[] { });
            try
            {
                return (AppercodePage)pageConstructorInfo.Invoke(new object[] { });
            }
            catch (TargetInvocationException ex)
            {
                isNavigationInProgress = false;
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
