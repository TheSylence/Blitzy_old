using System;
using System.Collections.Generic;
using System.Reflection;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Medy : InternalPlugin
	{
		public override void ClearCache()
		{
			CreateCommands();
		}

		public override bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			int lp;
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

		public override IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( RootItem == null )
			{
				CreateCommands();
			}

			yield return RootItem;
		}

		public override string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public override IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return null;
		}

		public override System.Windows.Controls.Control GetSettingsUI()
		{
			return null;
		}

		public override IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
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

		public override bool Load( IPluginHost host, string oldVersion = null )
		{
			return true;
		}

		public override void Unload( PluginUnloadReason reason )
		{
			// Do nothing
		}

		private void CreateCommands()
		{
			RootItem = CommandItem.Create( "medy", "ControlMediaPlayer".Localize(), this, "Medy.png" );
		}

		public override int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public override string Author
		{
			get { return "Matthias Specht"; }
		}

		public override string Description
		{
			get { return "Control Media Players with with Blitzy"; }
		}

		public override bool HasSettings { get { return false; } }

		public override string Name
		{
			get { return "Medy"; }
		}

		public override Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( "C3ED721C-AD70-47EB-B867-680B9DBAE8EA" );
				}

				return Guid.Value;
			}
		}

		public override string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public override Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		internal const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
		internal const int APPCOMMAND_MEDIA_PAUSE = 47;
		internal const int APPCOMMAND_MEDIA_PLAY = 46;
		internal const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
		internal const int APPCOMMAND_VOLUME_DOWN = 9;
		internal const int APPCOMMAND_VOLUME_UP = 10;
		internal const int WM_APPCOMMAND = 0x0319;
		private readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		private Guid? Guid;

		private CommandItem RootItem;
	}
}