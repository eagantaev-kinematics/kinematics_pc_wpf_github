using System;
//using bzdbg;

namespace kinematics_20160720
{
	static class Timeline
	{
		public static TimelineResearch GetData( bool is_app_mode_standalone )
		{
			//dbg.tr( "{" );
			var research = new TimelineResearch();
			if ( is_app_mode_standalone )
			{
				research.researchID = "0";
				research.researchName = "Автономное исследование";
				var dt = DateTime.Now;
				research.researchTime = dt.Date.ToString(); // "2017.03.07 00:00:00";
				return research;
			}
#if TIMELINE_EMULATE
			research.researchID = "50013";
			research.researchName = "Исследование 7";
			research.researchTime = "2017.03.07 00:00:00";
#else
			try
			{
				research.getTimelineData();
			}
			catch ( Exception ex )
			{
				//dbg.tr( "} getTimelineData exception: " + ex.Message );
				return null;
			}
#endif
			//dbg.tr( $"research: {research.researchID}  {research.researchName}" );
			//dbg.tr( "} ok" );
			return research;
		}
	} // class
}



