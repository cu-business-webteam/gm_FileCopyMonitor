using System.Linq;

namespace Johnson.FileCopyMonitor {

	public static class Program {

		public static void Main( System.String[] args ) {
			System.ServiceProcess.ServiceBase.Run( new MonitorService() );
		}

	}

}