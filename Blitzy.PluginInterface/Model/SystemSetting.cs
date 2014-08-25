// $Id$

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	public enum SystemSetting
	{
		[DefaultValue( 1 )]
		AutoUpdate,

		[DefaultValue( 1 )]
		CloseOnEscape,

		[DefaultValue( 1 )]
		CloseAfterCommand,

		[DefaultValue( 1 )]
		CloseOnFocusLost,

		[DefaultValue( "Control+Alt, Space" )]
		Shortcut,

		[DefaultValue( 1 )]
		TrayIcon,

		[DefaultValue( 0 )]
		KeepCommand,

		[DefaultValue( 0 )]
		StayOnTop,

		[DefaultValue( 20 )]
		MaxMatchingItems,

		[DefaultValue( "en" )]
		Language,

		[DefaultValue( "Default" )]
		Skin,

		[DefaultValue( 20 )]
		HistoryCount,

		[DefaultValue( 60 )]
		AutoCatalogRebuild,

		[DefaultValue( 1 )]
		BackupShortcuts,

		[DefaultValue( 0 )]
		RebuildCatalogOnChanges,

		[DefaultValue( "2000-01-01 00:00:00" )]
		LastCatalogBuild
	}
}