using System.Windows;

namespace Appercode.UI.Controls
{
    public partial class Grid
    {
        /// <summary>
        /// This definition is only for XAML XSD schema generation
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.Register(
                "ColumnDefinitions",
                typeof(ColumnDefinitionCollection),
                typeof(Grid),
                new PropertyMetadata());

        /// <summary>
        /// This definition is only for XAML XSD schema generation
        /// </summary>
        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.Register(
                "RowDefinitions",
                typeof(RowDefinitionCollection),
                typeof(Grid),
                new PropertyMetadata());

        private void NativeChildrenCollectionChanged()
        {
        }
    }
}