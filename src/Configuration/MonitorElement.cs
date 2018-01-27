using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public sealed class MonitorElement : System.Configuration.ConfigurationElement, INamedConfigurationElement {

		#region .ctor
		public MonitorElement() : base() {
		}
		public MonitorElement( System.String name ) : this() {
			this.Name = name;
		}
		#endregion .ctor


		#region properties
		[System.Configuration.ConfigurationProperty( "name", IsRequired = true, IsKey = true )]
		public System.String Name {
			get {
				return (System.String)this[ "name" ];
			}
			set {
				this[ "name" ] = value;
			}
		}

		[System.Configuration.ConfigurationProperty( "interval", DefaultValue = 30, IsRequired = false, IsKey = false )]
		public System.Int32 Interval {
			get {
				return (System.Int32)this[ "interval" ];
			}
			set {
				this[ "interval" ] = value;
			}
		}

		[System.Configuration.ConfigurationProperty( "paths", IsDefaultCollection = false, IsRequired = false )]
		[System.Configuration.ConfigurationCollection( typeof( PathCollection ),
			AddItemName = "add",
			ClearItemsName = "clear",
			RemoveItemName = "remove"
		)]
		public PathCollection Paths {
			get {
				return (PathCollection)base[ "paths" ];
			}
		}

		[System.Configuration.ConfigurationProperty( "execute", IsRequired = true )]
		public ExecuteElement Execute {
			get {
				return (ExecuteElement)base[ "execute" ];
			}
			set {
				this[ "execute" ] = value;
			}
		}
		#endregion properties

	}

}