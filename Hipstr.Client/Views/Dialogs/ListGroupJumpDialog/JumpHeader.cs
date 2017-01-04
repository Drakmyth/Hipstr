namespace Hipstr.Client.Views.Dialogs.ListGroupJumpDialog
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