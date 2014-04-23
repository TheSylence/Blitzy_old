// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blitzy
{
	internal static class SingleInstance
	{
		#region Constructor

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline" )]
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
			string name;

#if DEBUG
			name = "Blitzy.Debug";
			if( RuntimeConfig.Tests )
			{
				name += ".Test";
			}
#else
			name = string.Format( System.Globalization.CultureInfo.InvariantCulture, "Local\\{0}", AssemblyGuid );
#endif

			bool onlyInstance = false;
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
				IEnumerable<System.Runtime.InteropServices.GuidAttribute> attributes = Assembly.GetExecutingAssembly().GetCustomAttributes<System.Runtime.InteropServices.GuidAttribute>();
				if( attributes == null || attributes.Count() == 0 )
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