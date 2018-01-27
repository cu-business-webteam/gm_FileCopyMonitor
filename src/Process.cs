using System.Linq;

namespace Johnson.FileCopyMonitor {

	public sealed class Process : System.IDisposable {

		#region fields
		private System.Collections.Immutable.IImmutableSet<System.IO.FileSystemWatcher> myFileSystemWatcher;
		private System.Threading.Timer myTimer;
		private System.Collections.Immutable.IImmutableSet<System.String> myFilePathName;
		private System.Diagnostics.Process myExecutable;
		private System.Int32 myState;
		#endregion fields


		#region .ctor
		public Process( Configuration.MonitorElement configuration ) : base() {
			if ( null == configuration ) {
				throw new System.ArgumentNullException( "configuration" );
			}
			this.Interval = configuration.Interval * 1000;
			myTimer = new System.Threading.Timer( Process.OnAlarm, this, -1, 0 );
			myFilePathName = System.Collections.Immutable.ImmutableHashSet<System.String>.Empty;
			myState = 0;
			myFileSystemWatcher = new System.Collections.Immutable.ImmutableHashSet<System.IO.FileSystemWatcher>.Empty;
			var exec = configuration.Execute;
			if ( null != exec ) {
				this.Arguments = exec.Arguments;
				this.PathName = exec.Executable;
				this.WorkingDirectory = exec.WorkingDirectory;
			}
			System.IO.FileSystemWatcher fsw;
			foreach ( var path in configuration.Paths.OfType<Configuration.PathElement>() ) {
				foreach ( var filter  in path.Filters.OfType<Configuration.FilterElement>() ) {
					fsw = new System.IO.FileSystemWatcher( path.Path, filter.Filter );
					fsw.IncludeSubdirectories = false;
					fsw.Created += this.OnCreated;
					fsw.Renamed += this.OnRename;
					fsw.NotifyFilter = ( System.IO.NotifyFilters.CreationTime | System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size );
					myFileSystemWatcher = myFileSystemWatcher.Add( fsw );
				}
			}
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

		public System.Int32 Interval {
			get;
			set;
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
					fsw.EnableRaisingEvents = false;
					fsw.Dispose();
				}
				this.SetTimer( -1 );
				myTimer.Dispose();
				try {
					myExecutable.Kill();
				} catch ( System.Exception ) {
					;
				}
			}
		}

		public void Stop() {
			foreach ( var fsw in myFileSystemWatcher ) {
				fsw.EnableRaisingEvents = false;
			}
			this.SetTimer( -1 );
			try {
				myExecutable.Kill();
			} catch ( System.Exception ) {
				;
			}
		}
		public void Pause() {
			foreach ( var fsw in myFileSystemWatcher ) {
				fsw.EnableRaisingEvents = false;
			}
		}
		public void Continue() {
			foreach ( var fsw in myFileSystemWatcher.Where(
				x => !x.EnableRaisingEvents
			) ) {
				foreach ( var file in System.IO.Directory.GetFiles( fsw.Path, fsw.Filter ) ) {
					this.AddFileToList( file );
				}
				fsw.EnableRaisingEvents = true;
			}
			if ( 0 < myFilePathName.Count ) {
				this.SetTimer( this.Interval );
			}
		}
		public void Start() {
			this.Continue();
		}

		private void OnChanged( System.Object sender, System.IO.FileSystemEventArgs e ) {
			var fp = e.FullPath;
			if ( !myFilePathName.Contains( fp ) && System.IO.File.Exists( fp ) ) {
				this.AddFileToList( fp );
			}
			if ( 0 < myFilePathName.Count ) {
				this.SetTimer( this.Interval );
			}
		}
		private void OnCreated( System.Object sender, System.IO.FileSystemEventArgs e ) {
			var fp = e.FullPath;
			if ( !myFilePathName.Contains( fp ) && System.IO.File.Exists( fp ) ) {
				this.AddFileToList( fp );
			}
			if ( 0 < myFilePathName.Count ) {
				this.SetTimer( this.Interval );
			}
		}
		private void OnRename( System.Object sender, System.IO.RenamedEventArgs e ) {
			if ( myFilePathName.Contains( e.OldFullPath ) ) {
				this.RemoveFileFromList( e.OldFullPath );
			}
			var fp = e.FullPath;
			if ( !myFilePathName.Contains( fp ) && System.IO.File.Exists( fp ) ) {
				this.AddFileToList( fp );
			}
			if ( 0 < myFilePathName.Count ) {
				this.SetTimer( this.Interval );
			}
		}

		private void OnProcessComplete( System.Object sender, System.EventArgs e ) {
			System.Threading.Volatile.Write( ref myState, 0 );
		}

		private void AddFileToList( System.String file ) {
			System.Collections.Immutable.IImmutableSet<System.String> res;
			System.Collections.Immutable.IImmutableSet<System.String> original;
			while ( !myFilePathName.Contains( file ) ) {
				do {
					original = myFilePathName;
					res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref myFilePathName, myFilePathName.Add( file ), original );
				} while ( !myFilePathName.Contains( file ) );
			}
		}
		private void RemoveFileFromList( System.String file ) {
			System.Collections.Immutable.IImmutableSet<System.String> res;
			System.Collections.Immutable.IImmutableSet<System.String> original;
			while ( myFilePathName.Contains( file ) ) {
				do {
					original = myFilePathName;
					res = System.Threading.Interlocked.CompareExchange<System.Collections.Immutable.IImmutableSet<System.String>>( ref myFilePathName, myFilePathName.Remove( file ), original );
				} while ( myFilePathName.Contains( file ) );
			}
		}

		private void SetTimer( System.Int32 interval ) {
			myTimer.Change( interval, 0 );
		}
		private void SetTimer( System.Int32 interval, System.Int32 dueTime ) {
			myTimer.Change( interval, dueTime );
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
				proc.SetTimer( proc.Interval );
				return;
			}
			System.Boolean movedAny = false;
			System.Boolean someLocked = false;
			var list = proc.FilePathName;
			foreach ( var file in list ) {
				if ( !System.IO.File.Exists( file ) ) {
					proc.RemoveFileFromList( file );
				} else if ( Process.MoveFile( file, System.IO.Path.Combine( proc.DestinationPath, System.IO.Path.GetFileName( file ) ) ) ) {
					movedAny = true;
					proc.RemoveFileFromList( file );
				} else {
					someLocked = true;
				}
			}
			if ( movedAny && !someLocked ) {
				System.Threading.Volatile.Write( ref proc.myState, 1 );
				proc.myExecutable = new System.Diagnostics.Process {
					StartInfo = new System.Diagnostics.ProcessStartInfo {
						Arguments = proc.Arguments,
						CreateNoWindow = false,
						FileName = proc.PathName,
						UseShellExecute = false,
						WorkingDirectory = proc.WorkingDirectory
					}
				};
				proc.myExecutable.Exited += proc.OnProcessComplete;
				proc.myExecutable.Start();
			} else if ( 0 < proc.FilePathName.Count ) {
				proc.SetTimer( proc.Interval );
			}
		}
		private static System.Boolean MoveFile( System.String source, System.String destination ) {
			var output = false;
			try {
				System.IO.File.Move( source, destination );
				output = true;
			} catch ( System.Exception ) {
				;
			}
			return output;
		}
		#endregion static methods

	}

}