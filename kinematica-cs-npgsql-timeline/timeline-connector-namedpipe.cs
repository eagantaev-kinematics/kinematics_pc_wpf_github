using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;

namespace WindowsFormsApplication1
{

	static class TimelineConnector
	{
		const int timeout_connect = 1000;
		const string string_data_terminator = "end_data";
		public static Queue<string> get_data( string cmd )
		{
			Queue<string> sdata = new Queue<string>();
			using ( var pipeclient = new NamedPipeClientStream( "." , "timeline" , PipeDirection.InOut , PipeOptions.None ) )
			{
				try { pipeclient.Connect( timeout_connect ); }
				catch ( Exception ex )
				{
					tr( "exception:  " + ex.Message );
					return null;
				}
				if ( pipeclient.IsConnected == false )
				{
					tr( "connection failed" );
					return null;
				}
				tr( "ok, connected" );
				string s = "";
				try
				{
					tr( "try to writeline: " + cmd );
					byte[] data = Encoding.ASCII.GetBytes( cmd );
					pipeclient.Write( data , 0 , data.Length );
					pipeclient.Flush();
					tr( "write ok" );
					tr( "try to read" );
					using ( var sr = new StreamReader( pipeclient , true ) )
					{
						while ( sr.Peek() > -1 )
						{
							s = sr.ReadLine(); 
							//tr( "readline: " + s );
							if ( string.IsNullOrEmpty( s ) ) continue;
							if ( s.Equals( string_data_terminator ) )
								break;
							sdata.Enqueue( s ); // sdata.Add( s );
						}
					}
					tr( "end" );
				}
				catch ( Exception ex ) { tr( "exception: " + ex.Message ); sdata = null; }
				pipeclient.Close();
				tr( "pipeclient closed" );
			}
			return sdata;
		}
		static void tr( string msg = "ok" ) =>
			Debug.WriteLine( $"> { MethodBase.GetCurrentMethod().DeclaringType }: {msg}" );
	} // class
}



