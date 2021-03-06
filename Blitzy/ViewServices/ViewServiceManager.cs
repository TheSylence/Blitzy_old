﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;
using Blitzy.Model;
using Blitzy.Plugin;

namespace Blitzy.ViewServices
{
	public class ViewServiceManager : IViewServiceManager
	{
		TResult IViewServiceManager.Show<TResult>( ViewService service, object parameter )
		{
			switch( service )
			{
				case ViewService.MessageBox:
					return Show<MessageBoxService, TResult>( parameter );

				default:
					throw new ArgumentException( "Unknown ViewService" );
			}
		}

		public object Show<T>( object parameter = null ) where T : class, IViewService
		{
			IViewService service;
			if( Services.TryGetValue( typeof( T ), out service ) )
			{
				return service.Show( ActiveWindow, parameter );
			}

			throw new ArgumentException( "No DialogService registered for type" );
		}

		/// <summary>
		/// Ruf einen DialogService auf.
		/// </summary>
		/// <typeparam name="TService">Service, der aufgerufen werden soll.</typeparam>
		/// <typeparam name="TReturn">Typ, in den der Rückgabewert des
		/// aufgerufenen Services convertiert werden soll.</typeparam>
		/// <param name="parameter">Parameter, die dem Service übergeben werden.</param>
		/// <returns>Rückgabewert des Services.</returns>
		public TReturn Show<TService, TReturn>( object parameter = null ) where TService : class, IViewService
		{
			return (TReturn)Show<TService>( parameter );
		}

		internal T Create<T>() where T : ModelBase
		{
			IDataManipulationService service;
			if( ManipServices.TryGetValue( typeof( T ), out service ) )
			{
				return service.Create( ActiveWindow ) as T;
			}

			throw new ArgumentException( "No service found for type" );
		}

		internal bool Edit<T>( T obj ) where T : ModelBase
		{
			IDataManipulationService service;
			if( ManipServices.TryGetValue( typeof( T ), out service ) )
			{
				return service.Edit( ActiveWindow, obj );
			}

			throw new ArgumentException( "No service found for type" );
		}

		/// <summary>
		/// ONLY FOR TESTS!
		/// </summary>
		internal void RegisterManipService( Type type, IDataManipulationService service )
		{
			Debug.Assert( RuntimeConfig.Tests );

			ManipServices.Add( type, service );
		}

		/// <summary>
		/// ONLY FOR TESTS!
		/// </summary>
		internal void RegisterService( Type type, IViewService service )
		{
			Debug.Assert( RuntimeConfig.Tests );

			Services.Add( type, service );
		}

		[ExcludeFromCodeCoverage]
		internal void RegisterServices()
		{
			try
			{
				Type[] baseType = { typeof( IViewService ) };
				foreach( Type type in Assembly.GetExecutingAssembly().GetTypes().Where( t => !t.IsAbstract && baseType[0].IsAssignableFrom( t ) ) )
				{
					LogHelper.LogDebug( MethodBase.GetCurrentMethod().DeclaringType, "Registering DialogService {0}...", type );
					Services.Add( type, (IViewService)Activator.CreateInstance( type ) );
				}

				baseType[0] = typeof( IDataManipulationService );
				foreach( Type type in Assembly.GetExecutingAssembly().GetTypes().Where( t => !t.IsAbstract && baseType[0].IsAssignableFrom( t ) ) )
				{
					LogHelper.LogDebug( MethodBase.GetCurrentMethod().DeclaringType, "Registering DataManipulationService {0}...", type );
					IDataManipulationService srv = (IDataManipulationService)Activator.CreateInstance( type );
					ManipServices.Add( srv.ModelType, srv );
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

			LogHelper.LogDebug( MethodBase.GetCurrentMethod().DeclaringType, "{0} DataManipulationService registered", ManipServices.Count );
			LogHelper.LogDebug( MethodBase.GetCurrentMethod().DeclaringType, "{0} DialogServices registered", Services.Count );
		}

		/// <summary>
		/// ONLY FOR TESTS!
		/// </summary>
		internal void UnregisterService( Type type )
		{
			Debug.Assert( RuntimeConfig.Tests );
			Services.Remove( type );
		}

		public static ViewServiceManager Default
		{
			get
			{
				return _Default ?? ( _Default = new ViewServiceManager() );
			}
		}

		[ExcludeFromCodeCoverage]
		private static Window ActiveWindow
		{
			get
			{
				// Trifft nur während Tests zu... hoffentlich
				if( Application.Current == null )
					return null;

				return Application.Current.Windows.Cast<Window>().FirstOrDefault( x => x.IsActive );
			}
		}

		private static ViewServiceManager _Default;
		private readonly Dictionary<Type, IDataManipulationService> ManipServices = new Dictionary<Type, IDataManipulationService>();
		private readonly Dictionary<Type, IViewService> Services = new Dictionary<Type, IViewService>();
	}
}