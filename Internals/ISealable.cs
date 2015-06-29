namespace Appercode.UI.Internals
{
    internal interface ISealable
    {
        bool CanSeal
        {
            get;
        }

        bool IsSealed
        {
            get;
        }

        void Seal();
    }
}
