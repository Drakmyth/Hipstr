namespace Hipstr.Client.Dialogs
{
	public class DialogResult<T> where T : class
	{
		public bool Cancelled => Result == null;

		public T Result { get; }

		public DialogResult(T result)
		{
			Result = result;
		}

		public static DialogResult<T> CancelledResult()
		{
			return new DialogResult<T>(null);
		}
	}
}