using System.Linq;

namespace Johnson.FileCopyMonitor {

	public sealed class Watcher : System.IDisposable {

		#region fields
		private readonly System.IO.FileSystemWatcher myFsw;
		private System.Collections.Generic.ICollection<FileAlarm> myAlarm;
		#endregion fields


		#region .ctor
		public Watcher( System.IO.FileSystemWatcher fileSystemWatcher ) : base() {
			if ( null == fileSystemWatcher ) {
				throw new System.ArgumentNullException( "fileSystemWatcher" );
			}
			myFsw = fileSystemWatcher;
			myAlarm = new System.Collections.Generic.List<FileAlarm>();
		}

		~Watcher() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region properties
		public System.IO.FileSystemWatcher FileSystemWatcher {
			get {
				return myFsw;
			}
		}

		public System.Collections.Generic.ICollection<FileAlarm> Alarm {
			get {
				return myAlarm;
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
				var probe = ( myFsw as System.IDisposable );
				if ( null != probe ) {
					probe.Dispose();
				}
			}
		}
		#endregion methods

	}

}