using System.Linq;

namespace Johnson.FileCopyMonitor {

	public class MonitorService : System.ServiceProcess.ServiceBase {

		#region fields
		private const System.String theServiceName = "gm_FileCopyMonitor";

		private System.Collections.Generic.ICollection<Process> myProcess;
		private readonly System.Action<System.String> myLog;
		#endregion fields


		#region .ctor
		public MonitorService() : this( false ) {
		}
		public MonitorService( System.Boolean logEvents ) : base() {
			this.AutoLog = true;

			this.CanPauseAndContinue = true;
			this.CanStop = true;
			this.CanHandlePowerEvent = false;
			this.CanHandleSessionChangeEvent = false;

			this.ServiceName = theServiceName;

			myProcess = new System.Collections.Generic.List<Process>();

			myLog = ( x ) => System.Console.Error.WriteLine( x );
			if ( logEvents ) {
				myLog = ( x ) => {
					System.Console.Error.WriteLine( x );
					this.EventLog.WriteEntry( x, System.Diagnostics.EventLogEntryType.Information );
				};
			}
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
					myProcess.Add( new Process( m, myLog ) );
				} catch ( System.Exception e ) {
					myLog( System.String.Format( "Exception: {0}\r\n{1}\r\nStack Trace follows:\r\n{2}", e.GetType().Name, e.Message, e.StackTrace ) );
					this.Stop();
					throw;
				}
#if TRACE
				name = m.Name;
				foreach ( var path in m.Paths.OfType<Configuration.PathElement>() ) {
					foreach ( var filter in path.Filters.OfType<Configuration.FilterElement>() ) {
						entry = System.String.Format( "Monitoring {0} : {1}\\{2}", name, path.Path, filter.Filter );
						myLog( entry );
					}
				}
#endif
			}
			System.Threading.Tasks.Parallel.ForEach( myProcess.AsParallel(), x => x.Start() );
#if TRACE
			entry = "All monitors activated";
			myLog( entry );
#endif
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