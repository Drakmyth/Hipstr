using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Controls
{
	public sealed partial class MDLIcon : UserControl
	{
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}

		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register(nameof(Code), typeof(string), typeof(MDLIcon), PropertyMetadata.Create("E001"));

		private string ToGlyph(string code)
		{
			int ucode = int.Parse(code, System.Globalization.NumberStyles.HexNumber);
			return char.ConvertFromUtf32(ucode);
		}

		public MDLIcon()
		{
			InitializeComponent();
		}
	}
}
