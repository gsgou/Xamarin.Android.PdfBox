﻿namespace Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form
{
	public partial class FieldUtils : Java.Lang.Object
	{
		public partial class KeyValueValueComparator : Java.Lang.Object, Java.IO.ISerializable, Java.Util.IComparator
		{
			public int Compare(Java.Lang.Object o1, Java.Lang.Object o2)
			{
				KeyValueValueComparator c1 = o1 as KeyValueValueComparator;
				KeyValueValueComparator c2 = o2 as KeyValueValueComparator;
				return string.Compare(c1.ToString(), c1.ToString(), true);
			}
		}
	}
}