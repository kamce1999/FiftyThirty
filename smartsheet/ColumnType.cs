using System.Diagnostics.CodeAnalysis;

namespace Fifty.Smartsheet
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]

    public enum ColumnType
    {
        ServerId,
        EmployeeName,
        Position,
        PayRate,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
    }
}
