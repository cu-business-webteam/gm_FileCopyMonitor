using System.Linq;

namespace Johnson.FileCopyMonitor {

	public sealed class FileAlarm {

		#region fields
		private readonly Process myProcess;
		private readonly System.String myFilePathName;
		#endregion fields


		#region .ctor
		public FileAlarm( Process timer, System.String filePathName ) : base() {
			myFilePathName = filePathName;
			myProcess = timer;
		}
		#endregion .ctor


		#region properties
		public Process Timer {
			get {
				return myProcess;
			}
		}
		public System.String FilePathName {
			get {
				return myFilePathName;
			}
		}
		#endregion properties

	}

}