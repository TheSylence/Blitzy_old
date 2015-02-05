

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Blitzy.Utility
{
	/// <summary>
	/// Represents an hotKey
	/// </summary>
	internal class HotKey : INotifyPropertyChanged
	{
		private bool _Enabled;

		private Key _Key;

		private ModifierKeys _Modifiers;

		/// <summary>
		/// Creates an HotKey object. This instance has to be registered in an HotKeyHost.
		/// </summary>
		public HotKey()
		{
		}

		/// <summary>
		/// Creates an HotKey object. This instance has to be registered in an HotKeyHost.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
		public HotKey( Key key, ModifierKeys modifiers )
			: this( key, modifiers, true )
		{
		}

		/// <summary>
		/// Creates an HotKey object. This instance has to be registered in an HotKeyHost.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="modifiers">The modifier. Multiple modifiers can be combined with or.</param>
		/// <param name="enabled">Specifies whether the HotKey will be enabled when registered to an HotKeyHost</param>
		[SuppressMessage( "Microsoft.Usage", "CA2214" )]
		public HotKey( Key key, ModifierKeys modifiers, bool enabled )
		{
			Key = key;
			Modifiers = modifiers;
			Enabled = enabled;
		}

		/// <summary>
		/// Will be raised if the hotkey is pressed (works only if registed in HotKeyHost)
		/// </summary>
		public event EventHandler<HotKeyEventArgs> HotKeyPressed;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Enabled
		{
			get
			{
				return _Enabled;
			}
			set
			{
				if( value != _Enabled )
				{
					_Enabled = value;
					OnPropertyChanged( "Enabled" );
				}
			}
		}

		/// <summary>
		/// The Key. Must not be null when registering to an HotKeyHost.
		/// </summary>
		public Key Key
		{
			get
			{
				return _Key;
			}
			set
			{
				if( _Key != value )
				{
					_Key = value;
					OnPropertyChanged( "Key" );
				}
			}
		}

		/// <summary>
		/// The modifier. Multiple modifiers can be combined with or.
		/// </summary>
		public ModifierKeys Modifiers
		{
			get
			{
				return _Modifiers;
			}
			set
			{
				if( _Modifiers != value )
				{
					_Modifiers = value;
					OnPropertyChanged( "Modifiers" );
				}
			}
		}

		public override string ToString()
		{
			return string.Format( CultureInfo.InvariantCulture, "{0} + {1} ({2}Enabled)", Modifiers, Key, Enabled ? "" : "Not " );
		}

		internal void RaiseOnHotKeyPressed()
		{
			OnHotKeyPress();
		}

		protected virtual void OnHotKeyPress()
		{
			if( HotKeyPressed != null )
				HotKeyPressed( this, new HotKeyEventArgs( this ) );
		}

		protected virtual void OnPropertyChanged( string propertyName )
		{
			if( PropertyChanged != null )
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
		}
	}

	[SuppressMessage( "Microsoft.Design", "CA1064:ExceptionsShouldBePublic" )]
	[Serializable]
	internal class HotKeyAlreadyRegisteredException : Exception
	{
		public HotKeyAlreadyRegisteredException( string message, HotKey hotKey )
			: base( message )
		{
			HotKey = hotKey;
		}

		public HotKeyAlreadyRegisteredException( string message, HotKey hotKey, Exception inner )
			: base( message, inner )
		{
			HotKey = hotKey;
		}

		public HotKey HotKey { get; private set; }
	}

	internal class HotKeyEventArgs : EventArgs
	{
		public HotKeyEventArgs( HotKey hotKey )
		{
			HotKey = hotKey;
		}

		public HotKey HotKey { get; private set; }
	}

	/// <summary>
	/// The HotKeyHost needed for working with hotKeys.
	/// </summary>
	internal sealed class HotKeyHost : IDisposable
	{
		private static readonly SerialCounter IDGen = new SerialCounter( 1 );

		private readonly Dictionary<int, HotKey> _HotKeys = new Dictionary<int, HotKey>();

		/// <summary>
		/// Creates a new HotKeyHost
		/// </summary>
		/// <param name="hwndSource">The handle of the window. Must not be null.</param>
		public HotKeyHost( HwndSource hwndSource )
		{
			if( hwndSource == null )
				throw new ArgumentNullException( "hwndSource" );

			Hook = WndProc;
			HwndSource = hwndSource;
			hwndSource.AddHook( Hook );
		}

		#region HotKey Interop

		internal const int WM_HOTKEY = 786;

		#endregion HotKey Interop

		#region Interop-Encapsulation

		private readonly HwndSourceHook Hook;
		private readonly HwndSource HwndSource;

		private void RegisterHotKey( int id, HotKey hotKey )
		{
			if( (int)HwndSource.Handle != 0 )
			{
				int error = INativeMethods.Instance.RegisterHotKey_Wrapper( HwndSource.Handle, id, (int)hotKey.Modifiers, KeyInterop.VirtualKeyFromKey( hotKey.Key ) );
				if( error == 0 )
				{
					error = Marshal.GetLastWin32Error();
					Exception e = new Win32Exception( error );

					if( error == 1409 )
						throw new HotKeyAlreadyRegisteredException( e.Message, hotKey, e );

					throw e;
				}
			}
			else
				throw new InvalidOperationException( "Handle is invalid" );
		}

		private void UnregisterHotKey( int id )
		{
			if( (int)HwndSource.Handle != 0 )
			{
				int error = INativeMethods.Instance.UnregisterHotKey_Wrapper( HwndSource.Handle, id );
				if( error == 0 )
				{
					throw new Win32Exception( Marshal.GetLastWin32Error() );
				}
			}
		}

		#endregion Interop-Encapsulation

		/// <summary>
		/// Will be raised if any registered hotKey is pressed
		/// </summary>
		public event EventHandler<HotKeyEventArgs> HotKeyPressed;

		/// <summary>
		/// All registered hotKeys
		/// </summary>
		public IEnumerable<HotKey> HotKeys { get { return _HotKeys.Values; } }

		/// <summary>
		/// Adds an hotKey.
		/// </summary>
		/// <param name="hotKey">The hotKey which will be added. Must not be null and can be registed only once.</param>
		public void AddHotKey( HotKey hotKey )
		{
			if( hotKey == null )
				throw new ArgumentNullException( "hotKey" );
			if( hotKey.Key == 0 )
				throw new ArgumentNullException( "hotKey", @"hotKey.Key == 0" );
			if( _HotKeys.ContainsValue( hotKey ) )
				throw new HotKeyAlreadyRegisteredException( "HotKey already registered!", hotKey );

			int id = IDGen.Next();
			if( hotKey.Enabled )
				RegisterHotKey( id, hotKey );
			hotKey.PropertyChanged += hotKey_PropertyChanged;
			_HotKeys[id] = hotKey;
		}

		/// <summary>
		/// Removes an hotKey
		/// </summary>
		/// <param name="hotKey">The hotKey to be removed</param>
		/// <returns>True if success, otherwise false</returns>
		public bool RemoveHotKey( HotKey hotKey )
		{
			var kvPair = _HotKeys.FirstOrDefault( h => h.Value == hotKey );
			if( kvPair.Value != null )
			{
				kvPair.Value.PropertyChanged -= hotKey_PropertyChanged;
				try
				{
					if( kvPair.Value.Enabled )
					{
						UnregisterHotKey( kvPair.Key );
					}
					return _HotKeys.Remove( kvPair.Key );
				}
				catch( Win32Exception )
				{
					return false;
				}
			}

			return false;
		}

		private void hotKey_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			var kvPair = _HotKeys.FirstOrDefault( h => h.Value == sender );
			if( kvPair.Value != null )
			{
				if( e.PropertyName == "Enabled" )
				{
					if( kvPair.Value.Enabled )
						RegisterHotKey( kvPair.Key, kvPair.Value );
					else
						UnregisterHotKey( kvPair.Key );
				}
				else if( e.PropertyName == "Key" || e.PropertyName == "Modifiers" )
				{
					if( kvPair.Value.Enabled )
					{
						UnregisterHotKey( kvPair.Key );
						RegisterHotKey( kvPair.Key, kvPair.Value );
					}
				}
			}
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == WM_HOTKEY )
			{
				if( _HotKeys.Count == 0 )
					Debugger.Break();

				if( _HotKeys.ContainsKey( (int)wParam ) )
				{
					HotKey h = _HotKeys[(int)wParam];
					h.RaiseOnHotKeyPressed();
					if( HotKeyPressed != null )
						HotKeyPressed( this, new HotKeyEventArgs( h ) );
				}
			}

			return new IntPtr( 0 );
		}

		public class SerialCounter
		{
			public SerialCounter( int start )
			{
				Current = start;
			}

			public int Current { get; private set; }

			public int Next()
			{
				return ++Current;
			}
		}

		#region Destructor

		private bool Disposed;

		~HotKeyHost()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing )
		{
			if( Disposed )
				return;

			if( disposing )
			{
				HwndSource.RemoveHook( Hook );
			}

			for( int i = _HotKeys.Count - 1; i >= 0; i-- )
			{
				RemoveHotKey( _HotKeys.Values.ElementAt( i ) );
			}

			Disposed = true;
		}

		#endregion Destructor
	}
}