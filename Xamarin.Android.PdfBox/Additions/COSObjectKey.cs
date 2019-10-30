using Java.Lang;

namespace Com.Tom_roush.Pdfbox.Cos
{
	public partial class COSObjectKey : Object, IComparable
	{
		int IComparable.CompareTo(Object obj)
		{
			return CompareTo((COSObjectKey)obj);
		}
	}
}