using System;
using System.Configuration;
using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public sealed class FilterCollection : ConfigurationElementCollectionBase<FilterElement> {

		#region .ctor
		public FilterCollection() : base() {
		}
		public FilterCollection( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region methods
		protected override Object GetElementKey( ConfigurationElement element ) {
			if ( null == element ) {
				throw new System.ArgumentNullException( "element" );
			}
			var probe = ( element as FilterElement );
			if ( null == probe ) {
				throw new System.ArgumentException();
			}
			return probe.Filter;
		}
		#endregion methods

	}

}