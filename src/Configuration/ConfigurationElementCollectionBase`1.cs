using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public abstract class ConfigurationElementCollectionBase<T> : System.Configuration.ConfigurationElementCollection where T : System.Configuration.ConfigurationElement, new() {

		#region .ctor
		protected ConfigurationElementCollectionBase() : base() {
		}
		protected ConfigurationElementCollectionBase( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region properties
		public virtual System.Int32 IndexOf( T element ) {
			return this.BaseIndexOf( element );
		}
		#endregion properties


		#region methods
		protected override System.Configuration.ConfigurationElement CreateNewElement() {
			return new T();
		}

		public virtual void Add( T element ) {
			if ( null == element ) {
				throw new System.ArgumentNullException( "element" );
			}
			this.BaseAdd( element );
		}
		public virtual void RemoveAt( System.Int32 index ) {
			if ( index < 0 ) {
				throw new System.ArgumentOutOfRangeException( "index" );
			}
			this.BaseRemoveAt( index );
		}
		public virtual void Clear() {
			this.BaseClear();
		}
		#endregion methods

	}

}