using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class PropertyChangedListener
	{
		public PropertyChangedListener( INotifyPropertyChanged obj )
		{
			Obj = obj;
			Obj.PropertyChanged += Obj_PropertyChanged;
		}

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
			ChangedProperties.Clear();

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
					object value = info.GetValue( Obj );
					info.SetValue( Obj, value );
					Assert.IsFalse( WasChanged( info.Name ), info.Name );

					value = info.PropertyType.GetNonDefaultValue();
					info.SetValue( Obj, value );
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

		private HashSet<string> ChangedProperties = new HashSet<string>();
		private HashSet<string> Excludes = new HashSet<string>();
		private INotifyPropertyChanged Obj;
		private Dictionary<string, object> ValueMap = new Dictionary<string, object>();
	}
}