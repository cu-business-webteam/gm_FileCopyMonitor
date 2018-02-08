using System.Linq;

namespace Johnson.FileCopyMonitor {

	public static class Program {

		public static void Main( System.String[] args ) {
#if DEBUG
			if ( ( null != args ) && args.Any(
				x => System.String.Equals( x, "/run", System.StringComparison.OrdinalIgnoreCase )
			) ) {
				System.Console.Error.WriteLine( "(Strike any key to quit)" );
				using ( var m = new MonitorService() ) {
					m.Start( null );
					System.Console.ReadKey( true );
				}
				return;
			}
#endif
			System.ServiceProcess.ServiceBase.Run( new MonitorService() );
		}

	}

}