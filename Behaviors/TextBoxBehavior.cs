using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Behaviors
{
    /// <summary>
    /// Attached behavior to add placeholder text support to TextBox controls.
    /// This provides a visual hint for empty TextBoxes.
    /// </summary>
    public static class TextBoxBehavior
    {
        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }

        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(TextBoxBehavior),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(170, 170, 170));
                    textBox.Text = (string)e.NewValue;
                }

                textBox.GotFocus += (s, args) =>
                {
                    if (textBox.Text == (string)e.NewValue)
                    {
                        textBox.Text = string.Empty;
                        textBox.Foreground = System.Windows.SystemColors.WindowTextBrush;
                    }
                };

                textBox.LostFocus += (s, args) =>
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(170, 170, 170));
                        textBox.Text = (string)e.NewValue;
                    }
                };
            }
        }
    }
}
