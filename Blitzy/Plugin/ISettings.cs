using Blitzy.Model;

namespace Blitzy.Plugin
{
	public interface ISettings
	{
		T GetSystemSetting<T>( SystemSetting setting );

		T GetValue<T>( IPlugin plugin, string key );

		void RemoveValue( IPlugin plugin, string key );

		void SetValue( IPlugin plugin, string key, object value );
	}
}