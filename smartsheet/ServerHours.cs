using System.Diagnostics.CodeAnalysis;

namespace Fifty.Smartsheet
{
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:ElementsMustBeSeparatedByBlankLine", Justification = "Reviewed. Suppression is OK here.")]
    public class ServerHours : WeeklyValues
    {
        public long ServerId { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public float PayRate { get; set; }
		public int RoleId { get; set; }
    }
}