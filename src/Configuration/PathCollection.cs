using System;
using System.Configuration;
using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	public sealed class PathCollection : ConfigurationElementCollectionBase<PathElement> {

		#region .ctor
		public PathCollection() : base() {
		}
		public PathCollection( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region methods
		protected override Object GetElementKey( ConfigurationElement element ) {
			if ( null == element ) {
				throw new System.ArgumentNullException( "element" );
			}
			var probe = ( element as PathElement );
			if ( null == probe ) {
				throw new System.ArgumentException();
			}
			return probe.Path;
		}
		#endregion methods

	}

}