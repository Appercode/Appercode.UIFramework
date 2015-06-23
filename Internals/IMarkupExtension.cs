using System;

namespace Appercode.UI.Markup
{
    internal interface IMarkupExtension
    {
        bool IsValidTargetForExtension(object target, object dp);

        void SetupExtension(object target, object dp);
    }
}
