using System;
using System.Configuration;
using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public sealed class MonitorCollection : NamedConfigurationElementCollectionBase<MonitorElement> {

		#region .ctor
		public MonitorCollection() : base() {
		}
		public MonitorCollection( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region methods
		protected override Object GetElementKey( ConfigurationElement element ) {
			if ( null == element ) {
				throw new System.ArgumentNullException( "element" );
			}
			var probe = ( element as MonitorElement );
			if ( null == probe ) {
				throw new System.ArgumentException();
			}
			return probe.Name;
		}
		#endregion methods

	}

}