using System.Linq;

public static class StringHelper {

	public static System.String TrimToNull( this System.String @string ) {
		if ( System.String.IsNullOrEmpty( @string ) ) {
			return null;
		}
		@string = @string.Trim();
		if ( System.String.IsNullOrEmpty( @string ) ) {
			return null;
		}
		return @string;
	}

}