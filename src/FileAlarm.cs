using System.Linq;

namespace Johnson.FileCopyMonitor {

	public sealed class FileAlarm : System.IDisposable {

		#region fields
		private readonly System.Threading.Timer myTimer;
		private readonly System.String myFilePathName;
		#endregion fields


		#region .ctor
		public FileAlarm( System.Threading.Timer timer, System.String filePathName ) : base() {
			myFilePathName = filePathName;
			myTimer = timer;
		}

		~FileAlarm() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region properties
		public System.Threading.Timer Timer {
			get {
				return myTimer;
			}
		}
		public System.String FilePathName {
			get {
				return myFilePathName;
			}
		}
		#endregion properties


		#region methods
		public void Dispose() {
			this.Dispose( true );
			System.GC.SuppressFinalize( this );
		}
		private void Dispose( System.Boolean disposing ) {
			if ( disposing ) {
				var probe = ( myTimer as System.IDisposable );
				if ( null != probe ) {
					probe.Dispose();
				}
			}
		}
		#endregion methods

	}

}