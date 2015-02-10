namespace Blitzy.Plugin
{
	public enum ViewService
	{
		MessageBox
	}

	public interface IViewServiceManager
	{
		TResult Show<TResult>( ViewService service, object parameter = null );
	}
}