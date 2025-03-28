using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public static class EntitiesConfiguration
    {

        public static ModelBuilder ConfigureEntities(this ModelBuilder builder)
        {
            return builder;
        }

        #region[Date Localization methods and Params]

        public static DateTime FromLocalToUtc(DateTime date)
        {

            return date.ToUniversalTime();
        }

        public static DateTime? FromLocalToUtcNullable(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToUniversalTime();
            }
            return null;
        }

        #endregion[Date Localization methods and Params]

    }
}
