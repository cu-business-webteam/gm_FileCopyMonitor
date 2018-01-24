using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	public sealed class PathElement : System.Configuration.ConfigurationElement {

		#region .ctor
		public PathElement() : base() {
		}
		#endregion .ctor


		#region properties
		[System.IO.IODescription( "The directory of interest" )]
		[System.Configuration.ConfigurationProperty( "path", IsRequired = true, IsKey = true )]
		public System.String Path {
			get {
				return (System.String)this[ "path" ];
			}
			set {
				this[ "path" ] = value;
			}
		}
		[System.Configuration.ConfigurationProperty( "", IsDefaultCollection = true, IsRequired = false )]
		[System.Configuration.ConfigurationCollection( typeof( FilterCollection ),
			AddItemName = "add",
			ClearItemsName = "clear",
			RemoveItemName = "remove"
		)]
		public FilterCollection Parameters {
			get {
				return (FilterCollection)base[ "" ];
			}
		}
		#endregion properties

	}

}