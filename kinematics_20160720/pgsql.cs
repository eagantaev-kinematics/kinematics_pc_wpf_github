using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace kinematics_20160720
{
	public class DBPostgreSQL
	{
		static string db_schema = "kinematics";
		static string db_table = "data";

		const int tio_connect = 5; // sec
		const int tio_cmd = 5; // sec
		static string db_name = null;
		static string conn_str = null;
		static void tr( string msg = "ok" ) => Debug.WriteLine( $"> { MethodBase.GetCurrentMethod().DeclaringType }: {msg}" );

		public static void init( string server_ip , string database , string user , string pwd )
		{
			db_name = database;
			conn_str = $"Server={server_ip};User Id={user};Password={pwd};Database={database};";  // + "Timeout={tio_connect};CommandTimeout={tio_cmd};";
		}

		public static NpgsqlConnection open_conn()
		{
			NpgsqlConnection conn = new NpgsqlConnection( conn_str );
			if ( conn.State != ConnectionState.Open )
				try { conn.Open(); }
				catch ( Exception ex )
				{
					Console.WriteLine( "exception: " + ex.Message ); return null;
				}
			return conn;
		}

		public static bool create_schema()
		{
			using ( var conn = open_conn() )
			{
				if ( conn == null ) { tr( "} ! conn failed" ); return false; }
				using ( var cmd = new NpgsqlCommand( "" , conn ) )
				{
					cmd.CommandText = $"CREATE SCHEMA IF NOT EXISTS {db_schema};";
					tr( "cmd:  " + cmd.CommandText );
					try
					{
						var ra = cmd.ExecuteNonQuery();
						tr( "rows:  " + ra );
					}
					catch ( Exception ex )
					{
						tr( "exception: " + ex.Message );
						return false;
					}
					finally { conn.Close(); }
				}
			}
			return true;
		}
		public static bool create_table()
		{
			var sw = Stopwatch.StartNew();
			bool result = false;
			using ( var conn = open_conn() )
			{
				if ( conn == null ) { tr( "} ! conn failed" ); return false; }
				using ( var cmd = new NpgsqlCommand( "" , conn ) )
				{
					cmd.CommandText = $"CREATE TABLE IF NOT EXISTS  {db_schema}.{db_table} ( ";
					cmd.CommandText += "data " + NpgsqlDbType.Bytea.ToString();
					cmd.CommandText += ", research_id " + NpgsqlDbType.Integer.ToString();
					cmd.CommandText += ", tso_ids_active integer[]";
					cmd.CommandText += ");";
					tr( "cmd:  " + cmd.CommandText );
					try
					{
						var ra = cmd.ExecuteNonQuery();
						tr( "rows:  " + ra );
						result = true;
					}
					catch ( Exception ex ) { tr( "exception: " + ex.Message ); result = false; }
				}
				conn.Close();
			}
			return result;
		}
		public static bool is_table_exists()
		{
			if ( string.IsNullOrEmpty( conn_str ) ) {/* tr( "not initialized" );*/ return false; }
			bool is_table_exists = false;
			using ( NpgsqlConnection conn = open_conn() )
			{
				if ( conn == null ) { /*dbg.tr( "conn failed" );*/ return false; }
				using ( NpgsqlCommand cmd = new NpgsqlCommand() )
				{
					cmd.Connection = conn;
					cmd.Parameters.Add( new NpgsqlParameter( "tbl_name" , NpgsqlDbType.Unknown ) ).Value = db_table;
					cmd.CommandText = $"SELECT 1 FROM pg_tables WHERE tablename=:tbl_name";
					try
					{
						is_table_exists = cmd.ExecuteScalar() != null;
					}
					catch ( Exception ex ) { tr( "exception: " + ex.Message ); is_table_exists = false; }
				}
				conn.Close();
			}
			return is_table_exists;
		}

		public static bool insert_bytea( byte[] ba , string research_id , int[] tso_ids_active )
		{
			using ( var conn = open_conn() )
			{
				if ( conn == null ) { Console.WriteLine( "! conn failed" ); return false; }
				using ( var cmd = new NpgsqlCommand( "" , conn ) )
				{
					cmd.CommandText = $"INSERT INTO {db_schema}.{db_table} Values(:data , :researchid , :tso_ids_active)";
					cmd.Parameters.Add( new NpgsqlParameter( "data" , NpgsqlDbType.Bytea ) ).Value = ba;
					cmd.Parameters.Add( new NpgsqlParameter( "researchid" , NpgsqlDbType.Integer ) ).Value = research_id;
					cmd.Parameters.Add( new NpgsqlParameter( "tso_ids_active" , NpgsqlDbType.Array | NpgsqlDbType.Integer ) ).Value = tso_ids_active;
					try
					{
						var ra = cmd.ExecuteNonQuery();
					}
					catch ( Exception ex ) { Console.WriteLine( "exception: " + ex.Message ); return false; }
					finally { conn.Close(); }
				}
			}
			return true;
		}

		public static bool select_bytea()
		{
			var conn = open_conn();
			if ( conn == null ) { Console.WriteLine( "! conn failed" ); return false; }
			using ( var cmd = new NpgsqlCommand() )
			{
				cmd.Connection = conn;
				cmd.CommandText = $"SELECT * FROM {db_schema}.{db_table}";
				try
				{
					using ( var reader = cmd.ExecuteReader() )
					{
						Console.WriteLine( "cols: " + reader.FieldCount );
						if ( reader.FieldCount != 3 )
							throw new Exception( "ERROR TABLE FORMAT" );
						while ( reader.Read() )
						{
							int i = 0;
							byte[] result = (byte[])reader[i++];
							int research_id = (int)reader[i++];
							var tso_ids_active = (int[])reader[i++];
							Console.WriteLine( Encoding.UTF8.GetString( result ) + "  research_id: " + research_id );
							foreach ( var tso_id in tso_ids_active )
								tr( "active: " + tso_id );
						}
					}
				}
				catch ( Exception ex ) { Console.WriteLine( "exception: " + ex.Message ); return false; }
				finally { conn.Close(); }
			}
			return true;
		}
	} // class
}
