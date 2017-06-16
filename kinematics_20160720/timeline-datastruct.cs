using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace kinematics_20160720
{
	public class TimelineResearch
	{
		public string researchID = null;
		public string researchName = null;
		public string researchTime = null;

		public List<TesterOnCheckOut> testerOnCheckOut_ls = new List<TesterOnCheckOut>();
		public List<int> tso_id_active = new List<int>();

		static readonly string[] research_keys = { "researchID" , "researchName" , "researchTime" };
		static readonly string[] testerOnCheckOut_keys = {
			"testerOnCheckOutID"
			,"techniqueName"
			,"testerID"
			,"firstName"
			,"secondName"
			,"lastName"
		};
		static readonly string[] testerStepsOrder_keys = { "testerStepsOrderID" , "shortName" , "duration" };

		Queue<string> sdata = null;
		List<string> values = new List<string>();
		///  CTOR
		public TimelineResearch() { }
		public TimelineResearch( string[] sa )
		{
			int i = 0;
			researchID = sa[i++];
			researchName = sa[i++];
			researchTime = sa[i++];
			for ( int k = i; k < sa.Length; k++ )
			{
				tr( "tso_id_active: " + sa[k] );
				int tso_id = 0;
				if ( int.TryParse( sa[k] , out tso_id ) )
					tso_id_active.Add( tso_id );
				//tso_id_active.Add( s );
				//else throw new Exception( "timeline tso_id_active wrong format: " + s );
			}
		}

		void getResearchInfo( List<string> sa )
		{
			if ( sa.Count != 3 )
				throw new Exception( this.GetType() + ": wrong input strings number: " + sa.Count );
			int i = 0;
			researchID = sa[i++];
			researchName = sa[i++];
			researchTime = sa[i++];
			researchTime = DateTime.Parse( researchTime , CultureInfo.InvariantCulture ).ToString( "yyyy-MM-dd" );
			//DateTime dt;
			//if ( DateTime.TryParse( researchTime , out dt ) )
			////if ( DateTime.TryParse( researchTime , CultureInfo.InvariantCulture , DateTimeStyles.None , out dt ) )
			//	researchTime = dt.ToString();
			//else
			//	throw new Exception( this.GetType() + ": wrong researchTime string: " + researchTime );
		}

		public void getTimelineData()
		{
			try
			{
				///			get research info
				sdata = TimelineConnector.get_data( "research" );
				if ( sdata == null ) throw new Exception( "timeline client failed" ); // return false;
				if ( get_vals_by_keys( research_keys ) == false )
					throw new Exception( "research parsing failed" );  // return false;
				getResearchInfo( values );
				tr( $"research: {researchID} [{researchName}] {researchTime}" );
				/////			get testers info
				//for ( ; sdata.Count != 0; )
				//{
				//	if ( get_vals_by_keys( testerOnCheckOut_keys ) == false ) break;
				//	var toc = new TesterOnCheckOut( values );
				//	if ( toc != null ) testerOnCheckOut_ls.Add( toc );
				//}
				//if ( testerOnCheckOut_ls.Count < 1 )
				//	throw new Exception( "TesterOnCheckOut parsing failed" );
				/////			get test order per tester
				//foreach ( var toc in testerOnCheckOut_ls )
				//{
				//	var id = toc.testerOnCheckOutID;
				//	sdata = TimelineConnector.get_data( "tester_tocid " + id );
				//	if ( sdata == null ) throw new Exception( "timeline client failed" ); // return false;
				//	for ( ; sdata.Count != 0; )
				//	{
				//		if ( get_vals_by_keys( testerStepsOrder_keys ) == false ) break;
				//		var tso = new TesterStepsOrder( values );
				//		toc.testerStepsOrder_ls.Add( tso );
				//	}
				//	if ( toc.testerStepsOrder_ls.Count < 1 )
				//		throw new Exception( "TesterStepsOrder parsing failed" ); // return false;
				//}
				///			get active testers
				sdata = TimelineConnector.get_data( "get_active" );
				//dbg.tr( "sdata: " + sdata );
				if ( sdata == null ) throw new Exception( "timeline client failed" ); // return false;
				foreach ( var s in sdata )
				{
					tr( "tso_id_active: " + s );
					int tso_id = 0;
					if ( int.TryParse( s , out tso_id ) )
						tso_id_active.Add( tso_id );
					//tso_id_active.Add( s );
					else throw new Exception( "timeline tso_id_active wrong format: " + s );
				}
			}
			catch ( Exception ) { throw; }
			//return true;
		}

		bool get_vals_by_keys( string[] keys )
		{
			values?.Clear();
			foreach ( var key in keys )
			{
				var value = get_val_by_key( key );
				//dbg.tr( "key: " + key + " : " + ( string.IsNullOrEmpty( value ) ? "failed" : value ) );
				//if ( string.IsNullOrEmpty( value ) ) return false;
				//values.Add( value );
				if ( value == null ) return false;
				values.Add( value.Length == 0 ? "-" : value );
			}
			if ( keys.Length != values.Count ) return false;
			return true;
		}
		string get_val_by_key( string key )
		{
			if ( sdata.Count < 1 ) return null;
			var kv = sdata.Dequeue().Split( new char[] { ':' } , 2 );
			if ( kv.Length != 2 ) return null;
			if ( kv[0].Trim().Equals( key ) ) return kv[1].Trim();
			return null;
		}
		static void tr( string msg = "ok" ) => Debug.WriteLine( $"> { MethodBase.GetCurrentMethod().DeclaringType }: {msg}" );

	} // class

	public class TesterStepsOrder
	{
		public string testerStepsOrderID = null;
		public string shortName = null;
		public string duration = null;
		public TesterStepsOrder( List<string> sa )
		{
			if ( sa.Count != 3 )
				throw new Exception( this.GetType() + ": wrong input strings number: " + sa.Count );
			int i = 0;
			testerStepsOrderID = sa[i++];
			shortName = sa[i++];
			duration = sa[i++];
		}
	} // class
	public class TesterOnCheckOut
	{
		public string testerOnCheckOutID = null;
		public string techniqueName = null;
		public string testerID = null;
		public string firstName = null;
		public string secondName = null;
		public string lastName = null;
		public List<TesterStepsOrder> testerStepsOrder_ls = new List<TesterStepsOrder>();
		public TesterOnCheckOut( List<string> sa )
		{
			if ( sa.Count != 6 )
				throw new Exception( this.GetType() + ": wrong input strings number: " + sa.Count );
			int i = 0;
			testerOnCheckOutID = sa[i++];
			techniqueName = sa[i++];
			testerID = sa[i++];
			firstName = sa[i++];
			secondName = sa[i++];
			lastName = sa[i++];
		}

	} // lcass
}
