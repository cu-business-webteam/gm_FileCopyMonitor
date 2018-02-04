﻿using System.Linq;

namespace Johnson.FileCopyMonitor {

	public class MonitorService : System.ServiceProcess.ServiceBase {

		#region fields
		private System.Collections.Generic.ICollection<Process> myProcess;
		#endregion fields


		#region .ctor
		public MonitorService() : base() {
			this.AutoLog = true;

			this.CanPauseAndContinue = true;
			this.CanStop = true;
			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;

			this.ServiceName = "gm_FileCopyMonitor";

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
		protected override sealed void OnPause() {
			foreach ( var proc in myProcess ) {
				proc.Pause();
			}
			base.OnPause();
		}
		protected override sealed void OnContinue() {
			foreach ( var proc in myProcess ) {
				proc.Continue();
			}
			base.OnPause();
		}
		#endregion methods

		private void InitializeComponent() {
			// 
			// MonitorService
			// 
			this.CanPauseAndContinue = true;
			this.ServiceName = "gm_FileCopyMonitor";

		}
	}

}