using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SimpleHelpDeskAPI.DbContexts.ValueConverters
{
    public class DateOnlyValueConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyValueConverter() : base(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v))
        {
        }
    }
}