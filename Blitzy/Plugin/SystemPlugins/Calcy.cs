using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Calcy : InternalPlugin
	{
		public override void ClearCache()
		{
			// Nothing to do
		}

		public override bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			message = null;
			if( input.Count >= 2 )
			{
				string res = Calculator.Calculate( input[1] );
				STAThread.QueueAction( () => Clipboard.SetText( res, TextDataFormat.Text ) );
			}
			return true;
		}

		public override IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "calcy", "CalcyDescription".Localize(), this, "Calcy.png", null, null, null, true );
		}

		public override string GetInfo( IList<string> data, CommandItem item )
		{
			if( data.Count <= 1 )
			{
				return null;
			}

			return Calculator.Calculate( data[1] );
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
			yield break;
		}

		public override bool Load( IPluginHost host, string oldVersion = null )
		{
			Calculator = new ShuntingYard();
			return true;
		}

		public override void Unload( PluginUnloadReason reason )
		{
			// Nothing to do
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
			get { return "Math calculator plugin for Blitzy"; }
		}

		public override bool HasSettings { get { return false; } }

		public override string Name
		{
			get { return "Calcy"; }
		}

		public override Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( "C0E02745-BBAB-4D6F-85A9-DB9879C75A4D" );
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

		private ShuntingYard Calculator;
		private Guid? Guid;
	}
}