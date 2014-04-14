﻿// $Id$

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	public class PropertyChangedListener
	{
		#region Constructor

		public PropertyChangedListener( INotifyPropertyChanged obj )
		{
			Obj = obj;
			Obj.PropertyChanged += Obj_PropertyChanged;
		}

		#endregion Constructor

		#region Methods

		public void Exclude<T>( Expression<Func<T, object>> exp )
		{
			Excludes.Add( exp.GetNameFromExpression() );
		}

		public void SetValue( string prop, object value )
		{
			ValueMap[prop] = value;
		}

		public bool TestProperties()
		{
			foreach( PropertyInfo info in Obj.GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ) )
			{
				BrowsableAttribute attr = info.GetCustomAttribute<BrowsableAttribute>();
				if( attr != null && attr.Browsable == false )
				{
					continue;
				}

				if( Excludes.Contains( info.Name ) )
				{
					continue;
				}

				if( !info.CanWrite )
				{
					continue;
				}

				if( info.SetMethod != null && info.SetMethod.IsPrivate )
				{
					continue;
				}

				if( ValueMap.ContainsKey( info.Name ) )
				{
					info.SetValue( Obj, ValueMap[info.Name] );
				}
				else
				{
					info.SetValue( Obj, info.GetValue( Obj ) );
					Assert.IsFalse( WasChanged( info.Name ) );
					info.SetValue( Obj, info.PropertyType.GetNonDefaultValue() );
				}
				Assert.IsTrue( WasChanged( info.Name ), info.Name );
			}

			return true;
		}

		private void Obj_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			ChangedProperties.Add( e.PropertyName );
		}

		/// <summary>
		/// Überprüft ob das PropertyChangedEvent für die Eigenschaft ausgelöst wurde
		/// </summary>
		/// <param name="propName">Name der Eigenschaft.</param>
		private bool WasChanged( string propName )
		{
			return ChangedProperties.Contains( propName );
		}

		#endregion Methods

		#region Attributes

		private HashSet<string> ChangedProperties = new HashSet<string>();
		private HashSet<string> Excludes = new HashSet<string>();
		private INotifyPropertyChanged Obj;
		private Dictionary<string, object> ValueMap = new Dictionary<string, object>();

		#endregion Attributes
	}
}