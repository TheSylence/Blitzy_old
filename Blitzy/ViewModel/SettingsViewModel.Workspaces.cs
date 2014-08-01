﻿// $Id$

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using Blitzy.Model;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class WorkspaceSettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public WorkspaceSettingsViewModel( Settings settings )
			: base( settings )
		{
			Workspaces = new ObservableCollection<Workspace>();

			using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WorkspaceID FROM workspaces";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Workspace workspace = new Workspace { ID = reader.GetInt32( 0 ) };

						workspace.Load( Settings.Connection );
						Workspaces.Add( workspace );
					}
				}
			}
		}

		#endregion Constructor

		#region Methods

		public override void Save()
		{
			foreach( Workspace ws in Workspaces )
			{
				ws.Save( Settings.Connection );
			}
		}

		private void UpdateItemOrders()
		{
			for( int i = 0; i < SelectedWorkspace.Items.Count; ++i )
			{
				SelectedWorkspace.Items[i].ItemOrder = i + 1;
			}
		}

		#endregion Methods

		#region Commands

		private RelayCommand _AddItemCommand;
		private RelayCommand _AddWorkspaceCommand;
		private RelayCommand _DeleteWorkspaceCommand;
		private RelayCommand _MoveItemDownCommand;
		private RelayCommand _MoveItemUpCommand;
		private RelayCommand _RemoveItemCommand;

		public RelayCommand AddItemCommand
		{
			get
			{
				return _AddItemCommand ??
					( _AddItemCommand = new RelayCommand( ExecuteAddItemCommand, CanExecuteAddItemCommand ) );
			}
		}

		public RelayCommand AddWorkspaceCommand
		{
			get
			{
				return _AddWorkspaceCommand ??
					( _AddWorkspaceCommand = new RelayCommand( ExecuteAddWorkspaceCommand, CanExecuteAddWorkspaceCommand ) );
			}
		}

		public RelayCommand DeleteWorkspaceCommand
		{
			get
			{
				return _DeleteWorkspaceCommand ??
					( _DeleteWorkspaceCommand = new RelayCommand( ExecuteDeleteWorkspaceCommand, CanExecuteDeleteWorkspaceCommand ) );
			}
		}

		public RelayCommand MoveItemDownCommand
		{
			get
			{
				return _MoveItemDownCommand ??
					( _MoveItemDownCommand = new RelayCommand( ExecuteMoveItemDownCommand, CanExecuteMoveItemDownCommand ) );
			}
		}

		public RelayCommand MoveItemUpCommand
		{
			get
			{
				return _MoveItemUpCommand ??
					( _MoveItemUpCommand = new RelayCommand( ExecuteMoveItemUpCommand, CanExecuteMoveItemUpCommand ) );
			}
		}

		public RelayCommand RemoveItemCommand
		{
			get
			{
				return _RemoveItemCommand ??
					( _RemoveItemCommand = new RelayCommand( ExecuteRemoveItemCommand, CanExecuteRemoveItemCommand ) );
			}
		}

		private bool CanExecuteAddItemCommand()
		{
			return SelectedWorkspace != null;
		}

		private bool CanExecuteAddWorkspaceCommand()
		{
			return true;
		}

		private bool CanExecuteDeleteWorkspaceCommand()
		{
			return SelectedWorkspace != null;
		}

		private bool CanExecuteMoveItemDownCommand()
		{
			return SelectedWorkspace != null && SelectedItem != null && SelectedWorkspace.Items.LastOrDefault() != SelectedItem;
		}

		private bool CanExecuteMoveItemUpCommand()
		{
			return SelectedWorkspace != null && SelectedItem != null && SelectedWorkspace.Items.FirstOrDefault() != SelectedItem;
		}

		private bool CanExecuteRemoveItemCommand()
		{
			return SelectedWorkspace != null && SelectedItem != null;
		}

		private void ExecuteAddItemCommand()
		{
			//TextInputParameter args = new TextInputParameter( "EnterWorkspaceCommand".Localize(), "AddWorkspaceItem".Localize() );
			//string command = DialogServiceManager.Show<TextInputService, string>( args );

			string command = DialogServiceManager.Show<OpenFileService, string>();

			if( !string.IsNullOrWhiteSpace( command ) )
			{
				WorkspaceItem item = new WorkspaceItem
				{
					WorkspaceID = SelectedWorkspace.ID,
					ItemCommand = command,
					ItemOrder = 1
				};
				if( SelectedWorkspace.Items.Count > 0 )
				{
					item.ItemOrder = SelectedWorkspace.Items.Max( i => i.ItemOrder ) + 1;
				}

				item.ItemID = 1;
				IEnumerable<WorkspaceItem> allItems = Workspaces.SelectMany( ws => ws.Items ).ToArray();
				if( allItems.Any() )
				{
					item.ItemID = allItems.Max( i => i.ItemOrder ) + 1;
				}

				SelectedWorkspace.Items.Add( item );
			}
		}

		private void ExecuteAddWorkspaceCommand()
		{
			TextInputParameter args = new TextInputParameter( "EnterWorkspaceName".Localize(), "AddWorkspace".Localize() );
			string name = DialogServiceManager.Show<TextInputService, string>( args );

			if( !string.IsNullOrWhiteSpace( name ) )
			{
				Workspace ws = new Workspace { Name = name, ID = 1 };
				if( Workspaces.Count > 0 )
				{
					ws.ID = Workspaces.Max( w => w.ID ) + 1;
				}

				Workspaces.Add( ws );
				SelectedWorkspace = ws;
			}
		}

		private void ExecuteDeleteWorkspaceCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "ConfirmDeleteWorkspace".Localize(), "DeleteWorkspace".Localize() );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );
			if( result == MessageBoxResult.Yes )
			{
				SelectedWorkspace.Delete( Settings.Connection );
				Workspaces.Remove( SelectedWorkspace );
				SelectedWorkspace = null;
			}
		}

		private void ExecuteMoveItemDownCommand()
		{
			int idx = SelectedWorkspace.Items.IndexOf( SelectedItem );

			WorkspaceItem item = SelectedWorkspace.Items[idx];
			SelectedWorkspace.Items[idx] = SelectedWorkspace.Items[idx + 1];
			SelectedWorkspace.Items[idx + 1] = item;

			UpdateItemOrders();
			SelectedItem = SelectedWorkspace.Items[idx + 1];
		}

		private void ExecuteMoveItemUpCommand()
		{
			int idx = SelectedWorkspace.Items.IndexOf( SelectedItem );

			WorkspaceItem item = SelectedWorkspace.Items[idx];
			SelectedWorkspace.Items[idx] = SelectedWorkspace.Items[idx - 1];
			SelectedWorkspace.Items[idx - 1] = item;

			UpdateItemOrders();
			SelectedItem = SelectedWorkspace.Items[idx - 1];
		}

		private void ExecuteRemoveItemCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "ConfirmDeleteItem".Localize(), "DeleteItem".Localize() );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );
			if( result == MessageBoxResult.Yes )
			{
				SelectedItem.Delete( Settings.Connection );
				SelectedWorkspace.Items.Remove( SelectedItem );
				SelectedItem = null;

				UpdateItemOrders();
			}
		}

		#endregion Commands

		#region Properties

		private WorkspaceItem _SelectedItem;
		private Workspace _SelectedWorkspace;

		public WorkspaceItem SelectedItem
		{
			get
			{
				return _SelectedItem;
			}

			set
			{
				if( _SelectedItem == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedItem );
				_SelectedItem = value;
				RaisePropertyChanged( () => SelectedItem );
			}
		}

		public Workspace SelectedWorkspace
		{
			get
			{
				return _SelectedWorkspace;
			}

			set
			{
				if( _SelectedWorkspace == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedWorkspace );
				_SelectedWorkspace = value;
				RaisePropertyChanged( () => SelectedWorkspace );
			}
		}

		public ObservableCollection<Workspace> Workspaces { get; private set; }

		#endregion Properties
	}
}