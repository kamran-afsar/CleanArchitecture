using CleanArchitecture.Application.Interfaces.Utility;

namespace CleanArchitecture.Infrastructure.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
