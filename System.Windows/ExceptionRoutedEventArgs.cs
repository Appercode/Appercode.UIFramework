using Appercode.UI;

namespace System.Windows
{
    public class ExceptionRoutedEventArgs : RoutedEventArgs
    {
        internal string ErrorMessage = "";

        private Exception innerException;

        internal ExceptionRoutedEventArgs(Exception innerException)
        {
            this.innerException = innerException;
        }

        internal ExceptionRoutedEventArgs()
        {
        }

        public Exception ErrorException
        {
            get
            {
                return new Exception(this.ErrorMessage, this.innerException);
            }
        }
    }
}