using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Internals
{
    internal enum ExpressionMode
    {
        None,
        NonSharable,
        ForwardsInvalidations,
        SupportsUnboundSources
    }
}
