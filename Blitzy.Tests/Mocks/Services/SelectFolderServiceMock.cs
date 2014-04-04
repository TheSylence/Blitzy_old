﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	internal class SelectFolderServiceMock : IDialogService
	{
		public string Folder;

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Folder;
		}
	}
}