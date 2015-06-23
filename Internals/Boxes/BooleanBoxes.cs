namespace Appercode.UI.Internals.Boxes
{
    internal static class BooleanBoxes
    {
        internal static object TrueBox;

        internal static object FalseBox;

        static BooleanBoxes()
        {
            BooleanBoxes.TrueBox = true;
            BooleanBoxes.FalseBox = false;
        }

        internal static object Box(bool value)
        {
            if (value)
            {
                return BooleanBoxes.TrueBox;
            }
            return BooleanBoxes.FalseBox;
        }
    }
}
