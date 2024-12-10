using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SimpleHelpDeskAPI.DbContexts.ValueComparers
{
    public class DateOnlyValueComparer : ValueComparer<DateOnly>
    {
        public DateOnlyValueComparer() : base((v1, v2) => v2.DayNumber == v1.DayNumber, v => v.GetHashCode())
        {
        }
    }
}