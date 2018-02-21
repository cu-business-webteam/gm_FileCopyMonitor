using System.Linq;

namespace Johnson.FileCopyMonitor {

	public sealed class Process : System.IDisposable {

		#region fields
		private readonly System.Action<System.String> myLog;
		private System.Collections.Immutable.IImmutableSet<System.IO.FileSystemWatcher> myFileSystemWatcher;
		private System.Threading.Timer myTimer;
		private System.Collections.Immutable.IImmutableSet<System.String> myFilePathName;
		private System.Diagnostics.Process myExecutable;
		private System.Int32 myState;
		#endregion fields


		#region .ctor
		public Process( Configuration.MonitorElement configuration, System.Action<System.String> log ) : base() {
			if ( null == configuration ) {
				throw new System.ArgumentNullException( "configuration" );
			}
			myLog = log;
			this.DestinationPath = configuration.Destination;
			this.Interval = configuration.Interval * 1000;
			myTimer = new System.Threading.Timer( this.OnAlarm, this, -1, 0 );
			myFilePathName = System.Collections.Immutable.ImmutableHashSet<System.String>.Empty;
			myState = 0;
			myFileSystemWatcher = System.Collections.Immutable.ImmutableHashSet<System.IO.FileSystemWatcher>.Empty;
			var exec = configuration.Execute;
			if ( null != exec ) {
				this.Arguments = exec.Arguments;
				this.PathName = exec.Executable;
				this.WorkingDirectory = exec.WorkingDirectory;
			}
			System.IO.FileSystemWatcher fsw;
			foreach ( var path in configuration.Paths.OfType<Configuration.PathElement>() ) {
				foreach ( var filter  in path.Filters.OfType<Configuration.FilterElement>() ) {
					try {
						fsw = new System.IO.FileSystemWatcher( System.Environment.ExpandEnvironmentVariables( path.Path ), System.Environment.ExpandEnvironmentVariables( filter.Filter ) );
					} catch ( System.Exception e ) {
						myLog( System.String.Format( "Exception: {0}\r\n{1}\r\nStack Trace follows:\r\n{2}", e.GetType().ToString(), e.Message, e.StackTrace ) );
						throw;
					}
					fsw.IncludeSubdirectories = false;
					fsw.Created += this.OnCreated;
					fsw.Renamed += this.OnRename;
					fsw.Deleted += this.OnDeleted;
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
				this.SetTimer( System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite );
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
			this.SetTimer( System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite );
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
#if TRACE
					myLog( System.String.Format( "Watching file {0}", file ) );
#endif
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
			myLog( System.String.Format( "OnChanged: {0}", fp ) );
			this.AddFileToList( fp );
			this.SetTimer( this.Interval );
		}
		private void OnDeleted( System.Object sender, System.IO.FileSystemEventArgs e ) {
			var fp = e.FullPath;
			this.RemoveFileFromList( fp );
			this.SetTimer( this.Interval );
		}
		private void OnCreated( System.Object sender, System.IO.FileSystemEventArgs e ) {
			var fp = e.FullPath;
			myLog( System.String.Format( "OnCreated: {0}", fp ) );
			this.AddFileToList( fp );
			this.SetTimer( this.Interval );
		}
		private void OnRename( System.Object sender, System.IO.RenamedEventArgs e ) {
			var ofp = e.OldFullPath;
			if ( this.FilePathName.Contains( ofp ) ) {
				this.RemoveFileFromList( ofp );
			}
			var fp = e.FullPath;
#if TRACE
			myLog( System.String.Format( "OnRename: {0} => {1}", ofp, fp ) );
#endif
			if ( !this.FilePathName.Contains( fp ) && System.IO.File.Exists( fp ) ) {
				this.AddFileToList( fp );
			}
			this.SetTimer( this.Interval );
		}

		private void OnProcessComplete( System.Object sender, System.EventArgs e ) {
#if TRACE
			myLog( "Execution complete" );
#endif
			System.Threading.Volatile.Write( ref myState, 0 );
			this.SetTimer( this.Interval );
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

		private void OnAlarm( System.Object state ) {
#if TRACE
			myLog( "OnAlarm" );
#endif
			if ( 0 != System.Threading.Volatile.Read( ref myState ) ) {
				this.SetTimer( this.Interval );
				return;
			}
			System.Boolean movedAny = false;
			System.Boolean someLocked = false;
			var list = this.FilePathName;
			foreach ( var file in list ) {
				myLog( System.String.Format( "Attempting to process file {0}", file ) );
				if ( !System.IO.File.Exists( file ) ) {
#if TRACE
					myLog( System.String.Format( "File not found: {0}", file ) );
#endif
					this.RemoveFileFromList( file );
				} else if ( Process.MoveFile( this.myLog, file, System.IO.Path.Combine( System.Environment.ExpandEnvironmentVariables( this.DestinationPath ), System.IO.Path.GetFileName( file ) ) ) ) {
#if TRACE
					myLog( System.String.Format( "Moved: {0}", file ) );
#endif
					movedAny = true;
					this.RemoveFileFromList( file );
				} else {
#if TRACE
					myLog( System.String.Format( "Locked File: {0}", file ) );
#endif
					someLocked = true;
				}
			}
			if ( movedAny && !someLocked ) {
				System.Threading.Volatile.Write( ref myState, 1 );
				var exec = new System.Diagnostics.Process {
					StartInfo = new System.Diagnostics.ProcessStartInfo {
						Arguments = System.Environment.ExpandEnvironmentVariables( this.Arguments ),
						CreateNoWindow = false,
						FileName = System.Environment.ExpandEnvironmentVariables( this.PathName ),
						UseShellExecute = false,
						WorkingDirectory = System.Environment.ExpandEnvironmentVariables( this.WorkingDirectory )
					}
				};
				exec.EnableRaisingEvents = true;
				exec.Exited += this.OnProcessComplete;
				exec.Start();
				this.myExecutable = exec;
#if TRACE
				this.myLog( System.String.Format( "Launching executable: {0}", exec.StartInfo.FileName ) );
#endif
			} else if ( someLocked ) {
				this.SetTimer( this.Interval );
			}
		}
		#endregion methods


		#region static methods
		private static System.Boolean MoveFile( System.Action<System.String> log, System.String source, System.String destination ) {
			var output = false;
			try {
				System.IO.File.Move( source, destination );
				output = true;
			} catch ( System.Exception e ) {
				log( System.String.Format( "Exception moving file: {0}", e.Message ) );
			}
			return output;
		}
		#endregion static methods

	}

}