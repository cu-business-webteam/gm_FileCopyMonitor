using System.Linq;

namespace Johnson.FileCopyMonitor {

	public class MonitorService : System.ServiceProcess.ServiceBase {

		#region fields
		#endregion fields


		#region .ctor
		public MonitorService() : base() {
			this.AutoLog = true;

			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;
			this.CanPauseAndContinue = true;
			this.CanStop = true;

			this.ServiceName = "File Copy Monitor";
		}
		#endregion .ctor


		#region properties
		#endregion properties


		#region methods
		protected sealed override void OnStart( System.String[] args ) {
			throw new System.NotImplementedException();
		}
		protected sealed override void OnContinue() {
			base.OnContinue();
		}
		protected sealed override void OnPause() {
			base.OnPause();
		}
		protected sealed override void OnStop() {
			base.OnStop();
		}
		#endregion methods

	}

}