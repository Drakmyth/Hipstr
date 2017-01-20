using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Attachments
{
	public static class ImmediateSourceUpdate
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
			"Source",
			typeof(string),
			typeof(ImmediateSourceUpdate),
			new PropertyMetadata(default(string), OnSourceChanged));

		public static string GetSource(DependencyObject target)
		{
			return (string)target.GetValue(SourceProperty);
		}

		public static void SetSource(DependencyObject target, string value)
		{
			target.SetValue(SourceProperty, value);
		}

		private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBox = d as TextBox;
			if (textBox == null) return;

			textBox.TextChanged += TextBox_OnTextChanged;
		}

		private static void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			textBox?.SetValue(SourceProperty, textBox.Text);
		}
	}
}