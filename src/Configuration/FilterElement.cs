using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	public sealed class FilterElement : System.Configuration.ConfigurationElement {

		#region .ctor
		public FilterElement() : base() {
		}
		#endregion .ctor


		#region properties
		[System.IO.IODescription( "File name filter pattern" )]
		[System.Configuration.ConfigurationProperty( "filter", IsRequired = true, IsKey = true )]
		public System.String Filter {
			get {
				return (System.String)this[ "filter" ];
			}
			set {
				this[ "filter" ] = value;
			}
		}
		#endregion properties

	}

}