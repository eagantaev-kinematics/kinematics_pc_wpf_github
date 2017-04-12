using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public partial class FormMain : Form
	{
		private void tr( string msg = "ok" ) => Debug.WriteLine( $"> { this.GetType().Name}: {msg}" );
		public FormMain()
		{
			InitializeComponent();
		}
		private void button1_Click( object sender , EventArgs e )
		{
			///		GET TIMELINE DATA
			var research = new TimelineResearch();
            /*
			try
			{

				research.getTimelineData();
				tr( "researchName: " + research.researchName );
				foreach ( var tso_id in research.tso_id_active )
					tr( "active: " + tso_id );
                
			}
			catch ( Exception ex )
			{
				tr( "} getTimelineData exception: " + ex.Message );
				return;
			}
            */
			///			WRITE TO DB
			//DBPostgreSQL.init( "127.0.0.1" , "postgres" , "postgres" , "0000" );
            DBPostgreSQL.init( "192.168.0.177" , "postgres" , "postgres" , "0000" );
			if ( !DBPostgreSQL.create_schema() || !DBPostgreSQL.create_table() )
			{
				MessageBox.Show( "ERROR" );
				return;
			}
			if ( !DBPostgreSQL.is_table_exists() )
			{
				MessageBox.Show( "ERROR" );
				return;
			}
			tr( "OK TABLE " );
			var buf = File.ReadAllBytes(@"c:\temp\2017_04_03__20_19_47__kinematics.txt");      // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   file for db saving
			//if ( !DBPostgreSQL.insert_bytea( buf , research.researchID , research.tso_id_active.ToArray() ) )
            if (!DBPostgreSQL.insert_bytea(buf, "12345", new int[] {1, 2}))
            {
			    MessageBox.Show( "ERROR" );
			    return;
		    }
			tr( "OK INSERT " );
			if ( !DBPostgreSQL.select_bytea() )
			{
				MessageBox.Show( "ERROR" );
				return;
			}
			tr( "OK SELECT " );
		}
		private void button2_Click( object sender , EventArgs e )
		{
		}
		private void button3_Click( object sender , EventArgs e )
		{
		}
		private void btnExit_Click( object sender , EventArgs e )
		{
			this.Close();
		}
	} // class
}
