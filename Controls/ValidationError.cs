using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appercode.UI.Controls
{
    public class ValidationError
    {
        internal ValidationError(Exception exception, bool useDefaultString)
        {
            this.Exception = exception;
            if (useDefaultString)
            {
                this.ErrorContent = "ValidationError";
                return;
            }
            this.ErrorContent = exception.Message;
        }

        internal ValidationError(object errorContent)
        {
            this.ErrorContent = errorContent;
        }

        public object ErrorContent
        {
            get;
            private set;
        }

        public Exception Exception
        {
            get;
            private set;
        }
    }
}
