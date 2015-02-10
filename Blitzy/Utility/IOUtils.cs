using System.IO;

namespace Blitzy.Utility
{
	internal static class IOUtils
	{
		#region Methods

		public static string GetTempFileName( string extension )
		{
			string fileName;
			int attempt = 0;
			bool exit = false;
			do
			{
				fileName = Path.GetRandomFileName();
				fileName = Path.ChangeExtension( fileName, extension );
				fileName = Path.Combine( Path.GetTempPath(), fileName );

				try
				{
					using( new FileStream( fileName, FileMode.CreateNew ) ) { }

					exit = true;
				}
				catch( IOException ex )
				{
					if( ++attempt == 10 )
						throw new IOException( "No unique temporary file name is available.", ex );
				}
			} while( !exit );

			return fileName;
		}

		#endregion Methods
	}
}