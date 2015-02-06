using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Plugin;
using Blitzy.Plugin.SystemPlugins;

using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	public class PluginInformation : ObservableObject
	{
		public bool Enabled
		{
			get
			{
				return _Enabled;
			}

			set
			{
				if( _Enabled == value )
				{
					return;
				}

				RaisePropertyChanging( () => Enabled );
				_Enabled = value;
				RaisePropertyChanged( () => Enabled );
			}
		}

		public Guid Id { get; set; }

		public IPlugin Instance { get; set; }

		public string Name { get; set; }

		private bool _Enabled;
	}

	public class PluginsDialogViewModel : ViewModelBaseEx
	{
		public PluginsDialogViewModel( DbConnectionFactory factory, ViewServiceManager serviceManager = null )
			: base( factory, serviceManager )
		{
		}

		public override void Reset()
		{
			base.Reset();

			Plugins = new ObservableCollection<PluginInformation>( PluginManager.Plugins.Concat( PluginManager.DisabledPlugins ).Where( plug => !( plug is ISystemPlugin ) )
				.Select( p => new PluginInformation() { Id = p.PluginID, Name = p.Name, Instance = p, Enabled = !PluginManager.DisabledPlugins.Contains( p ) } ) );
		}

		private bool CanExecuteDisableCommand()
		{
			return SelectedPlugin != null && SelectedPlugin.Enabled;
		}

		private bool CanExecuteEnableCommand()
		{
			return SelectedPlugin != null && !SelectedPlugin.Enabled;
		}

		private bool CanExecuteInstallCommand()
		{
			return true;
		}

		private void ExecuteDisableCommand()
		{
			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "UPDATE plugins SET disabled = 1 WHERE PluginID = @pluginID";

					DbParameter param = cmd.CreateParameter();
					param.ParameterName = "pluginID";
					param.Value = SelectedPlugin.Id;
					cmd.Parameters.Add( param );

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}
			}

			SelectedPlugin.Enabled = false;
			MessengerInstance.Send( new PluginMessage( SelectedPlugin.Instance, PluginAction.Disabled ) );
		}

		private void ExecuteEnableCommand()
		{
			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "UPDATE plugins SET disabled = 0 WHERE PluginID = @pluginID";

					DbParameter param = cmd.CreateParameter();
					param.ParameterName = "pluginID";
					param.Value = SelectedPlugin.Id;
					cmd.Parameters.Add( param );

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}
			}

			SelectedPlugin.Enabled = true;
			MessengerInstance.Send( new PluginMessage( SelectedPlugin.Instance, PluginAction.Enabled ) );
		}

		private void ExecuteInstallCommand()
		{
			FileDialogParameters param = new FileDialogParameters( "ZipArchive".Localize( null, "|*.zip" ) + "|" + "DLLFiles".Localize( null, "|*.dll" ) );
			string file = ServiceManagerInstance.Show<OpenFileService, string>( param );
			if( File.Exists( file ) )
			{
				Process proc = new Process();
				proc.StartInfo.UseShellExecute = true;
				proc.StartInfo.Verb = "runas";
				proc.StartInfo.Arguments = string.Format( "{0} \"{1}\"", Constants.CommandLine.InstallPlugin, file );
				proc.StartInfo.FileName = Assembly.GetExecutingAssembly().Location;
				proc.EnableRaisingEvents = true;

				proc.Start();
				proc.WaitForExit();

				// TODO: Read new plugin from database
			}
		}

		public RelayCommand DisableCommand
		{
			get
			{
				return _DisableCommand ??
					( _DisableCommand = new RelayCommand( ExecuteDisableCommand, CanExecuteDisableCommand ) );
			}
		}

		public RelayCommand EnableCommand
		{
			get
			{
				return _EnableCommand ??
					( _EnableCommand = new RelayCommand( ExecuteEnableCommand, CanExecuteEnableCommand ) );
			}
		}

		public RelayCommand InstallCommand
		{
			get
			{
				return _InstallCommand ??
					( _InstallCommand = new RelayCommand( ExecuteInstallCommand, CanExecuteInstallCommand ) );
			}
		}

		public ObservableCollection<PluginInformation> Plugins
		{
			get
			{
				return _Plugins;
			}

			set
			{
				if( _Plugins == value )
				{
					return;
				}

				RaisePropertyChanging( () => Plugins );
				_Plugins = value;
				RaisePropertyChanged( () => Plugins );
			}
		}

		public PluginInformation SelectedPlugin
		{
			get
			{
				return _SelectedPlugin;
			}

			set
			{
				if( _SelectedPlugin == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedPlugin );
				_SelectedPlugin = value;
				RaisePropertyChanged( () => SelectedPlugin );
			}
		}

		internal Plugin.PluginManager PluginManager { get; set; }

		private RelayCommand _DisableCommand;
		private RelayCommand _EnableCommand;

		private RelayCommand _InstallCommand;
		private ObservableCollection<PluginInformation> _Plugins;
		private PluginInformation _SelectedPlugin;
	}
}