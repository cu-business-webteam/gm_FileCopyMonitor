using System.Linq;

namespace Johnson.FileCopyMonitor.Configuration {

	[System.Serializable]
	public abstract class NamedConfigurationElementCollectionBase<T> : ConfigurationElementCollectionBase<T> where T : System.Configuration.ConfigurationElement, INamedConfigurationElement, new() {

		#region .ctor
		protected NamedConfigurationElementCollectionBase() : base() {
		}
		protected NamedConfigurationElementCollectionBase( System.Collections.IComparer comparer ) : base( comparer ) {
		}
		#endregion .ctor


		#region properties
		public override System.Configuration.ConfigurationElementCollectionType CollectionType {
			get {
				return System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		public virtual T this[ System.Int32 index ] {
			get {
				return (T)this.BaseGet( index );
			}
			set {
				if ( null != this.BaseGet( index ) ) {
					this.BaseRemoveAt( index );
				}
				this.BaseAdd( index, value );
			}
		}
		new public virtual T this[ System.String name ] {
			get {
				return (T)this.BaseGet( name );
			}
		}
		#endregion properties


		#region methods
		protected override System.Configuration.ConfigurationElement CreateNewElement( System.String elementName ) {
			T output = new T();
			output.Name = elementName;
			return output;
		}
		protected override System.Object GetElementKey( System.Configuration.ConfigurationElement element ) {
			return ( (T)element ).Name;
		}

		public virtual void Remove( T element ) {
			if ( null == element ) {
				return;
			}
			if ( 0 <= this.IndexOf( element ) ) {
				this.BaseRemove( element.Name );
			}
		}
		#endregion methods

	}

}