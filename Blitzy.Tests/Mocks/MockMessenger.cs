// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Tests.Mocks
{
	internal class MockMessenger : IMessenger
	{
		public void Register<TMessage>( object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action )
		{
			M.Register<TMessage>( recipient, receiveDerivedMessagesToo, action );
		}

		public void Register<TMessage>( object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action )
		{
			M.Register<TMessage>( recipient, token, receiveDerivedMessagesToo, action );
		}

		public void Register<TMessage>( object recipient, object token, Action<TMessage> action )
		{
			M.Register<TMessage>( recipient, token, action );
		}

		public void Register<TMessage>( object recipient, Action<TMessage> action )
		{
			M.Register<TMessage>( recipient, action );
		}

		public void Send<TMessage>( TMessage message, object token )
		{
			M.Send<TMessage>( message, token );
		}

		public void Send<TMessage, TTarget>( TMessage message )
		{
			M.Send<TMessage, TTarget>( message );
		}

		public void Send<TMessage>( TMessage message )
		{
			M.Send<TMessage>( message );
		}

		public void Unregister<TMessage>( object recipient, object token, Action<TMessage> action )
		{
			M.Unregister<TMessage>( recipient, token, action );
		}

		public void Unregister<TMessage>( object recipient, Action<TMessage> action )
		{
			M.Unregister<TMessage>( recipient, action );
		}

		public void Unregister<TMessage>( object recipient, object token )
		{
			M.Unregister<TMessage>( recipient, token );
		}

		public void Unregister<TMessage>( object recipient )
		{
			M.Unregister<TMessage>( recipient );
		}

		public void Unregister( object recipient )
		{
			M.Unregister( recipient );
		}

		private Messenger M = new Messenger();
	}
}