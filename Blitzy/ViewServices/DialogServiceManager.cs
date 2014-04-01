// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	internal static class DialogServiceManager
	{
		#region Methods

		static public object Show<T>( object parameter = null ) where T : class, IDialogService
		{
			IDialogService service;
			if( Services.TryGetValue( typeof( T ), out service ) )
			{
				return service.Show( ActiveWindow, parameter );
			}

			throw new ArgumentException( "Kein DialogService für typ gefunden" );
		}

		/// <summary>
		/// Ruf einen DialogService auf.
		/// </summary>
		/// <typeparam name="TService">Service, der aufgerufen werden soll.</typeparam>
		/// <typeparam name="TReturn">Typ, in den der Rückgabewert des
		/// aufgerufenen Services convertiert werden soll.</typeparam>
		/// <param name="parameter">Parameter, die dem Service übergeben werden.</param>
		/// <returns>Rückgabewert des Services.</returns>
		public static TReturn Show<TService, TReturn>( object parameter = null ) where TService : class, IDialogService
		{
			return (TReturn)Show<TService>( parameter );
		}

		/// <summary>
		/// NÜR FÜR TESTS!
		/// </summary>
		internal static void Clear()
		{
			Debug.Assert( App.Current == null );

			Services.Clear();
		}

		/// <summary>
		/// Nur für Testzwecke
		/// </summary>
		/// <param name="type"></param>
		/// <param name="service"></param>
		internal static void RegisterService( Type type, IDialogService service )
		{
			Debug.Assert( App.Current == null );

			Services.Add( type, service );
		}

		internal static void RegisterServices()
		{
			try
			{
				Type baseType = typeof( IDialogService );

				foreach( Type type in Assembly.GetExecutingAssembly().GetTypes().Where( t => !t.IsAbstract && baseType.IsAssignableFrom( t ) ) )
				{
					LogHelper.LogDebug( MethodInfo.GetCurrentMethod().DeclaringType, "Registering DialogService {0}...", type );
					Services.Add( type, (IDialogService)Activator.CreateInstance( type ) );
				}
			}
			catch( ReflectionTypeLoadException ex )
			{
				MessageBox.Show( ex.ToString() );

				foreach( var exp in ex.LoaderExceptions )
				{
					MessageBox.Show( exp.ToString() );
				}
			}

			LogHelper.LogDebug( MethodInfo.GetCurrentMethod().DeclaringType, "{0} DialogServices registered", Services.Count );
		}

		internal static void UnregisterService( Type type )
		{
			Services.Remove( type );
		}

		#endregion Methods

		#region Properties

		private static Window ActiveWindow
		{
			get
			{
				// Trifft nur während Tests zu... hoffentlich
				if( App.Current == null )
					return null;

				return App.Current.Windows.Cast<Window>().Where( x => x.IsActive ).FirstOrDefault();
			}
		}

		#endregion Properties

		#region Attributes

		private static Dictionary<Type, IDialogService> Services = new Dictionary<Type, IDialogService>();

		#endregion Attributes
	}
}