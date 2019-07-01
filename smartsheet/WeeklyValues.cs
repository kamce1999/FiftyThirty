using System.Diagnostics.CodeAnalysis;

namespace Fifty.Smartsheet
{
	[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]

	public class WeeklyValues
	{
		public float Monday { get; set; }
		public float Tuesday { get; set; }
		public float Wednesday { get; set; }
		public float Thursday { get; set; }
		public float Friday { get; set; }
		public float Saturday { get; set; }
		public float Sunday { get; set; }
	}
}