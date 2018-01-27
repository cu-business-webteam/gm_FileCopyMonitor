using System.Linq;

namespace Johnson.FileCopyMonitor {

	[System.Serializable]
	public sealed class Process : System.IDisposable {

		#region fields
		[System.NonSerialized]
		private System.Collections.Immutable.IImmutableSet<System.IO.FileSystemWatcher> myFileSystemWatcher;
		private System.Threading.Timer myTimer;
		private System.Collections.Immutable.IImmutableSet<System.String> myFilePathName;
		private System.Diagnostics.Process myExecutable;
		private System.Int32 myState;
		#endregion fields


		#region .ctor
		public Process() : base() {
			myFileSystemWatcher = System.Collections.Immutable.ImmutableHashSet<System.IO.FileSystemWatcher>.Empty;
			myTimer = new System.Threading.Timer( Process.OnAlarm, this, -1, 0 );
			myFilePathName = System.Collections.Immutable.ImmutableHashSet<System.String>.Empty;
			myState = 0;
		}

		~Process() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region properties
		[System.IO.IODescription( "Specifies the executable that is to run" )]
		public System.String PathName {
			get;
			internal set;
		}
		[System.IO.IODescription( "Specifies the arguments passed to the executable" )]
		public System.String Arguments {
			get;
			internal set;
		}
		[System.IO.IODescription( "Specifies the working directory for the executable" )]
		public System.String WorkingDirectory {
			get;
			internal set;
		}

		[System.IO.IODescription( "Specifies the target directory to copy the files for processing" )]
		public System.String DestinationPath {
			get;
			internal set;
		}

		internal System.Threading.Timer Timer {
			get {
				return myTimer;
			}
		}

		public System.Collections.Generic.IReadOnlyCollection<System.IO.FileSystemWatcher> FileSystemWatcher {
			get {
				return myFileSystemWatcher;
			}
		}

		public System.Collections.Generic.IReadOnlyCollection<System.String> FilePathName {
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
				System.Collections.Generic.IEnumerable<System.IO.FileSystemWatcher> list = myFileSystemWatcher;
				foreach ( var fsw in list ) {
					fsw.Dispose();
				}
				var probe = ( myTimer as System.IDisposable );
				if ( null != probe ) {
					probe.Dispose();
				}
			}
		}

		public void Stop() {
			this.Dispose();
			try {
				myExecutable.Kill();
			} catch ( System.Exception ) {
				;
			}
		}

		private void OnCreated( System.Object sender, System.IO.FileSystemEventArgs e ) {
			System.Collections.Immutable.IImmutableSet<System.String> res;
			System.Collections.Immutable.IImmutableSet<System.String> original;
			while ( !myFilePathName.Contains( e.FullPath ) ) {
				do {
					original = myFilePathName;
					res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref myFilePathName, myFilePathName.Add( e.FullPath ), original );
				} while ( !myFilePathName.Contains( e.FullPath ) && System.Object.Equals( res, original ) );
			}
			this.Timer.Change( 30000, 0 );
		}
		private void OnRename( System.Object sender, System.IO.RenamedEventArgs e ) {
			System.Collections.Immutable.IImmutableSet<System.String> res;
			System.Collections.Immutable.IImmutableSet<System.String> original;
			while ( myFilePathName.Contains( e.OldFullPath ) ) {
				do {
					original = myFilePathName;
					res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref myFilePathName, myFilePathName.Remove( e.OldFullPath ), original );
				} while ( myFilePathName.Contains( e.OldFullPath ) && System.Object.Equals( res, original ) );
			}
			while ( !myFilePathName.Contains( e.FullPath ) ) {
				do {
					original = myFilePathName;
					res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref myFilePathName, myFilePathName.Add( e.FullPath ), original );
				} while ( !myFilePathName.Contains( e.FullPath ) && System.Object.Equals( res, original ) );
			}
			this.Timer.Change( 30000, 0 );
		}
		#endregion methods


		#region static methods
		private static void OnAlarm( System.Object state ) {
			if ( null == state ) {
				return;
			}
			var proc = ( state as Process );
			if ( null == proc ) {
				return;
			}
			if ( 0 != System.Threading.Volatile.Read( ref proc.myState ) ) {
				proc.Timer.Change( 30000, 0 );
				return;
			}
			System.Boolean didMove = false;
			System.Boolean movedAny = false;
			System.Collections.Immutable.IImmutableSet<System.String> res;
			System.Collections.Immutable.IImmutableSet<System.String> original;
			var list = proc.myFilePathName;
			foreach ( var file in list ) {
				try {
					System.IO.File.Move( file, System.IO.Path.Combine( proc.DestinationPath, System.IO.Path.GetFileName( file ) ) );
					didMove =true;
					movedAny = true;
				} catch ( System.IO.IOException ) {
					didMove = false;
				} catch ( System.Exception ) {
					throw;
				}
				if ( didMove && proc.myFilePathName.Contains( file ) ) {
					do {
						original = proc.myFilePathName;
						res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref proc.myFilePathName, original.Remove( file ), original );
					} while ( original.Contains( file ) && System.Object.ReferenceEquals( res, original ) );
				}
			}
			if (
				( movedAny || !proc.myFilePathName.Any() )
				&& ( 0 == System.Threading.Interlocked.CompareExchange( ref proc.myState, 1, 0 ) )
			) {
				proc.myExecutable = new System.Diagnostics.Process {
					StartInfo = new System.Diagnostics.ProcessStartInfo {
						Arguments = proc.Arguments,
						CreateNoWindow = false,
						FileName = proc.PathName,
						UseShellExecute = false,
						WorkingDirectory = proc.WorkingDirectory
					}
				};
				proc.myExecutable.Start();
			} else {
				proc.myTimer.Change( 30000, 0 );
			}
		}
		#endregion static methods


	}

}