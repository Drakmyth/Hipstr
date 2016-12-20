namespace Hipstr.Client.Views
{
	public class JumpHeader
	{
		public string Header { get; }
		public bool Enabled { get; }

		public JumpHeader(string header, bool enabled)
		{
			Header = header;
			Enabled = enabled;
		}
	}
}