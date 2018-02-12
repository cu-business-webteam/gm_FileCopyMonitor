using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Johnson.FileCopyMonitor {
	[RunInstaller( true )]
	public partial class ProjectInstaller : System.Configuration.Install.Installer {
		public ProjectInstaller() {
			InitializeComponent();
#if DEBUG || TRACE
			this.FileCopyMonitorServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Manual;
#else
			this.FileCopyMonitorServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			this.FileCopyMonitorServiceInstaller.DelayedAutoStart = true;
#endif
		}
	}
}
