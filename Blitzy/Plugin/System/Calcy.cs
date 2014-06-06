// $Id$

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.System
{
	internal class Calcy : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			// Nothing to do
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			message = null;
			if( input.Count >= 2 )
			{
				string res = Calculator.Calculate( input[1] );
				STAThread.QueueAction( () => Clipboard.SetText( res, TextDataFormat.Text ) );
			}
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "calcy", "CalcyDescription".Localize(), this, "Calcy.png", null, null, null, true );
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			if( data.Count <= 1 )
			{
				return null;
			}

			return Calculator.Calculate( data[1] );
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield break;
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Calculator = new ShuntingYard();
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
			// Nothing to do
		}

		#endregion Methods

		#region Properties

		private Guid? GUID;

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
			get { return "Math calculator plugin for Blitzy"; }
		}

		public string Name
		{
			get { return "Calcy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( "C0E02745-BBAB-4D6F-85A9-DB9879C75A4D" );
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

		#endregion Properties

		#region Attributes

		private ShuntingYard Calculator;

		#endregion Attributes
	}
}