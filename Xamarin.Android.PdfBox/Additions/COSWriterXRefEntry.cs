using Java.Lang;

namespace Com.Tom_roush.Pdfbox.Pdfwriter
{
	public partial class COSWriterXRefEntry : Object, IComparable
	{
		int IComparable.CompareTo(Object obj)
		{
			return CompareTo((COSWriterXRefEntry)obj);
		}
	}
}