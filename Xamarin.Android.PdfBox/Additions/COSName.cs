using Java.Lang;

namespace Com.Tom_roush.Pdfbox.Cos
{
	public partial class COSName : Com.Tom_roush.Pdfbox.Cos.COSBase, IComparable
	{
		int IComparable.CompareTo(Object obj)
		{
			return CompareTo((COSName)obj);
		}
	}
}