using System.Linq;

namespace Johnson.FileCopyMonitor {

	public class MonitorService : System.ServiceProcess.ServiceBase {

		#region fields
		private System.Collections.Generic.ICollection<Process> myProcess;
		#endregion fields


		#region .ctor
		public MonitorService() : base() {
			this.AutoLog = true;

			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;
			this.CanPauseAndContinue = false;
			this.CanStop = true;

			this.ServiceName = "File Copy Monitor";

			myProcess = new System.Collections.Generic.List<Process>();
		}
		#endregion .ctor


		#region properties
		#endregion properties


		#region methods
		protected sealed override void OnStart( System.String[] args ) {
			var section = Configuration.FileCopyMonitorSection.GetSection();
			if ( null == section ) {
				return;
			}
			var monitors = section.Monitors.OfType<Configuration.MonitorElement>();
			if ( ( null == monitors ) || !monitors.Any() ) {
				return;
			}
			throw new System.NotImplementedException();
		}
		protected sealed override void OnStop() {
			foreach ( var proc in myProcess ) {
				proc.Dispose();
			}
			base.OnStop();
		}
		#endregion methods


		#region static methods
		#endregion static methods

	}

}