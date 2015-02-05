

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Blitzy.Utility
{
	internal static class SingleInstance
	{
		#region Constructor

		[SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline" )]
		static SingleInstance()
		{
			WM_SHOWFIRSTINSTANCE = INativeMethods.Instance.RegisterWindowMessage_Wrapper( "WM_SHOWFIRSTINSTANCE|{0}", AssemblyGuid );
		}

		#endregion Constructor

		#region Methods

		internal static void ShowFirstInstance()
		{
			INativeMethods.Instance.PostMessage_Wrapper( (IntPtr)INativeMethods.HWND_BROADCAST, WM_SHOWFIRSTINSTANCE, IntPtr.Zero, IntPtr.Zero );
		}

		internal static bool Start()
		{
#if DEBUG
			string name = "Blitzy.Debug";
			if( RuntimeConfig.Tests )
			{
				name += ".Test";
			}
#else
			string name = "Blitzy." + AssemblyGuid.ToString( System.Globalization.CultureInfo.InvariantCulture ).Replace( "-", "" );
#endif

			bool onlyInstance;
			AppMutex = new Mutex( true, name, out onlyInstance );
			return onlyInstance;
		}

		internal static void Stop()
		{
			AppMutex.ReleaseMutex();
			AppMutex.Dispose();
		}

		#endregion Methods

		#region Properties

		private static string AssemblyGuid
		{
			get
			{
				IEnumerable<GuidAttribute> attributes = Assembly.GetExecutingAssembly().GetCustomAttributes<GuidAttribute>().ToArray();
				if( !attributes.Any() )
				{
					return string.Empty;
				}

				return attributes.First().Value;
			}
		}

		#endregion Properties

		#region Attributes

		internal static readonly int WM_SHOWFIRSTINSTANCE;
		private static Mutex AppMutex;

		#endregion Attributes
	}
}