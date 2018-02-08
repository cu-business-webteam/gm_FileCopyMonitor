using System.Linq;

namespace Johnson.FileCopyMonitor {

	public static class Program {

		public static void Main( System.String[] args ) {
			if ( ( null != args ) && args.Any(
				x => System.String.Equals( x, "/run", System.StringComparison.OrdinalIgnoreCase )
			) ) {
				System.Console.Error.WriteLine( "(Strike any key to quit)" );
				System.Console.ReadKey( true );
			} else {
				System.ServiceProcess.ServiceBase.Run( new MonitorService() );
			}
		}

	}

}