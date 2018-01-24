using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	public sealed class ExecuteElement : System.Configuration.ConfigurationElement {

		#region .ctor
		public ExecuteElement() : base() {
		}
		#endregion .ctor


		#region properties
		[System.IO.IODescription( "Specifies the executable that is to run" )]
		[System.Configuration.ConfigurationProperty( "executable", IsRequired = true, IsKey = false )]
		public System.String Executable {
			get {
				return (System.String)this[ "executable" ];
			}
			set {
				this[ "executable" ] = value;
			}
		}

		[System.IO.IODescription( "Specifies the arguments passed to the executable" )]
		[System.Configuration.ConfigurationProperty( "arguments", DefaultValue = "", IsRequired = false, IsKey = false )]
		public System.String Arguments {
			get {
				return (System.String)this[ "arguments" ];
			}
			set {
				this[ "arguments" ] = value;
			}
		}

		[System.IO.IODescription( "Specifies the working directory for the executable" )]
		[System.Configuration.ConfigurationProperty( "workingDirectory", DefaultValue = "", IsRequired = false, IsKey = false )]
		public System.String WorkingDirectory {
			get {
				return (System.String)this[ "workingDirectory" ];
			}
			set {
				this[ "workingDirectory" ] = value;
			}
		}
		#endregion properties

	}

}