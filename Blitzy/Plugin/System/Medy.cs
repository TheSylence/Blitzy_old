// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.System
{
	internal class Medy : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			RootItem = CommandItem.Create( "medy", "ControlMediaPlayer".Localize(), this, "Medy.png" );
		}

		public bool ExecuteCommand( Model.CommandItem command, IList<string> input, out string message )
		{
			int lp = 0;
			switch( command.Name )
			{
				case "play":
					lp = APPCOMMAND_MEDIA_PLAY;
					break;

				case "pause":
					lp = APPCOMMAND_MEDIA_PAUSE;
					break;

				case "next":
					lp = APPCOMMAND_MEDIA_NEXTTRACK;
					break;

				case "prev":
					lp = APPCOMMAND_MEDIA_PREVIOUSTRACK;
					break;

				case "volup":
					lp = APPCOMMAND_VOLUME_UP;
					break;

				case "voldn":
					lp = APPCOMMAND_VOLUME_DOWN;
					break;

				default:
					message = "UnknownCommand".Localize();
					return false;
			}

			INativeMethods.Instance.SendMessage_Wrapper( HWND_BROADCAST, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)( lp * 65536 ) );
			message = null;
			return true;
		}

		public IEnumerable<Model.CommandItem> GetCommands( IList<string> input )
		{
			yield return RootItem;
		}

		public string GetInfo( IList<string> data, Model.CommandItem item )
		{
			return null;
		}

		public IEnumerable<CommandItem> GetSubCommands( Model.CommandItem parent, IList<string> input )
		{
			if( parent == RootItem )
			{
				yield return CommandItem.Create( "play", "MedyPlay".Localize(), this, "MedyPlay.png", null, RootItem );
				yield return CommandItem.Create( "pause", "MedyPause".Localize(), this, "MedyPause.png", null, RootItem );
				yield return CommandItem.Create( "next", "MedyNext".Localize(), this, "MedyNext.png", null, RootItem );
				yield return CommandItem.Create( "prev", "MedyPrev".Localize(), this, "MedyPrev.png", null, RootItem );
				yield return CommandItem.Create( "volup", "MedyVolup".Localize(), this, "MedyVolup.png", null, RootItem );
				yield return CommandItem.Create( "voldn", "MedyVoldn".Localize(), this, "MedyVoldn.png", null, RootItem );
			}
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			ClearCache();
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
			// Do nothing
		}

		#endregion Methods

		#region Attributes

		private Guid? GUID;

		private CommandItem RootItem;

		public int ApiVersion
		{
			get { return Constants.APIVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Control Media Players with with Blitzy"; }
		}

		public string Name
		{
			get { return "Medy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( "C3ED721C-AD70-47EB-B867-680B9DBAE8EA" );
				}

				return GUID.Value;
			}
		}

		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		#endregion Attributes

		#region Constants

		internal const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
		internal const int APPCOMMAND_MEDIA_PAUSE = 47;
		internal const int APPCOMMAND_MEDIA_PLAY = 46;
		internal const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
		internal const int APPCOMMAND_VOLUME_DOWN = 9;
		internal const int APPCOMMAND_VOLUME_UP = 10;
		internal const int WM_APPCOMMAND = 0x0319;
		private IntPtr HWND_BROADCAST = (IntPtr)0xffff;

		#endregion Constants
	}
}