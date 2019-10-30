using Java.Lang;

namespace Com.Tom_roush.Pdfbox.Text
{
	public partial class TextPositionComparator : Java.Util.IComparator
	{
		public int Compare(Object o1, Object o2)
		{
			TextPositionComparator c1 = o1 as TextPositionComparator;
			TextPositionComparator c2 = o2 as TextPositionComparator;
			return string.Compare(c1.ToString(), c1.ToString(), true);
		}
	}
}