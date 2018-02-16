using System.Linq;

namespace Johnson.FileCopyMonitor {

	public class MonitorService : System.ServiceProcess.ServiceBase {

		#region fields
		private const System.String theServiceName = "gm_FileCopyMonitor";

		private System.Collections.Generic.ICollection<Process> myProcess;
		#endregion fields


		#region .ctor
		public MonitorService() : base() {
			this.AutoLog = true;

			this.CanPauseAndContinue = true;
			this.CanStop = true;
			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;

			this.ServiceName = theServiceName;

			myProcess = new System.Collections.Generic.List<Process>();
		}
		#endregion .ctor


		#region methods
		private void InitializeComponent() {
			// 
			// MonitorService
			// 
			this.AutoLog = true;

			this.CanPauseAndContinue = true;
			this.CanStop = true;
			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;

			this.ServiceName = theServiceName;
		}

		public void Start( System.String[ ] args ) {
			this.OnStart( args );
		}
		protected sealed override void OnStart( System.String[] args ) {
			var section = Configuration.FileCopyMonitorSection.GetSection();
			if ( null == section ) {
				return;
			}
			var monitors = section.Monitors.OfType<Configuration.MonitorElement>();
			if ( ( null == monitors ) || !monitors.Any() ) {
				return;
			}
			System.String name;
			System.String entry;
			foreach ( var m in monitors ) {
				try {
#if TRACE
				myProcess.Add( new Process( m, this.EventLog ) );
#else
				myProcess.Add( new Process( m, null ) );
#endif
				} catch ( System.Exception e ) {
					this.EventLog.WriteEntry( "Exception: {0}\r\n{1}\r\nStack Trace follows:\r\n{2}", System.Diagnostics.EventLogEntryType.Information );
					this.Stop();
					throw;
				}
				name = m.Name;
				foreach ( var path in m.Paths.OfType<Configuration.PathElement>() ) {
					foreach ( var filter in path.Filters.OfType<Configuration.FilterElement>() ) {
						entry = System.String.Format( "Monitoring {0} : {1}\\{2}", name, path.Path, filter.Filter );
						this.EventLog.WriteEntry( entry, System.Diagnostics.EventLogEntryType.Information );
						System.Console.Error.WriteLine( entry );
					}
				}
			}
			System.Threading.Tasks.Parallel.ForEach( myProcess.AsParallel(), x => x.Start() );
			entry = "All monitors activated";
			this.EventLog.WriteEntry( entry, System.Diagnostics.EventLogEntryType.Information );
			System.Console.Error.WriteLine( entry );
		}
		protected sealed override void OnStop() {
			foreach ( var proc in myProcess ) {
				proc.Dispose();
			}
			myProcess = new System.Collections.Generic.List<Process>();
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

	}

}