namespace Hipstr.Client.Views
{
	public class ModalResult<T> where T : class
	{
		public bool Cancelled => Result == null;

		public T Result { get; }

		public ModalResult(T result)
		{
			Result = result;
		}

		public static ModalResult<T> CancelledResult()
		{
			return new ModalResult<T>(null);
		}
	}
}