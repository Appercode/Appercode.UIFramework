namespace Appercode.UI.Internals
{
    internal enum BaseValueSourceInternal : short
    {
        Unknown,
        Default,
        Inherited,
        ThemeStyle,
        ThemeStyleTrigger,
        Style,
        TemplateTrigger,
        StyleTrigger,
        ImplicitReference,
        ParentTemplate,
        ParentTemplateTrigger,
        Local
    }
}
