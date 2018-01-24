using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public sealed class FileCopyMonitorSection : System.Configuration.ConfigurationSection {

		#region fields
		public const System.String DefaultSectionName = "johnson.fileCopyMonitor";
		#endregion fields


		#region .ctor
		public FileCopyMonitorSection() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Configuration.ConfigurationProperty( "monitors", IsDefaultCollection = false, IsRequired = false )]
		[System.Configuration.ConfigurationCollection( typeof( MonitorCollection ),
			AddItemName = "add",
			ClearItemsName = "clear",
			RemoveItemName = "remove"
		)]
		public MonitorCollection Monitors {
			get {
				return (MonitorCollection)base[ "monitors" ];
			}
		}
		#endregion properties


		#region static methods
		public static FileCopyMonitorSection GetSection() {
			return ( System.Configuration.ConfigurationManager.GetSection( FileCopyMonitorSection.DefaultSectionName ) as FileCopyMonitorSection );
		}
		#endregion static methods

	}

}