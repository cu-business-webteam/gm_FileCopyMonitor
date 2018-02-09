namespace Johnson.FileCopyMonitor {
	partial class ProjectInstaller {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ) {
			if ( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.FileCopyMonitorProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.FileCopyMonitorServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// FileCopyMonitorProcessInstaller
			// 
			this.FileCopyMonitorProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
			this.FileCopyMonitorProcessInstaller.Password = null;
			this.FileCopyMonitorProcessInstaller.Username = null;
			// 
			// FileCopyMonitorServiceInstaller
			// 
			this.FileCopyMonitorServiceInstaller.Description = "Monitors multiple drop-off directories for pick-up and copy to elsewhere.";
			this.FileCopyMonitorServiceInstaller.DisplayName = "File Copy Monitor Service";
			this.FileCopyMonitorServiceInstaller.ServiceName = "gm_FileCopyMonitor";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FileCopyMonitorProcessInstaller,
            this.FileCopyMonitorServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller FileCopyMonitorProcessInstaller;
		private System.ServiceProcess.ServiceInstaller FileCopyMonitorServiceInstaller;
	}
}