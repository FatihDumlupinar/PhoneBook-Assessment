using Microsoft.EntityFrameworkCore;
using PhoneBook.Contact.Dtos.Enums;
using PhoneBook.Contact.Entities.Db;
using PhoneBook.Contact.Repositories.Contexts;
using PhoneBook.Shared.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Repositories.Seed
{
    public static class SeedData
    {
        public static async Task EnsurePopulatedAsync(ContactDbContext context)
        {
            context.Database.Migrate();

            var datetimenow = DateTime.Now;

            if (!await context.StaticContactTypes.AnyAsync())
            {
                await context.StaticContactTypes.AddRangeAsync(new List<StaticContactType>()
                {
                    new StaticContactType()
                    {
                        CreateDate=datetimenow,
                        Id=(int)StaticContactTypeEnm.PhoneNumber,
                        Name=EnumHelper<StaticContactTypeEnm>.GetDisplayValue(StaticContactTypeEnm.PhoneNumber)
                    },
                    new StaticContactType()
                    {
                        CreateDate=datetimenow,
                        Id=(int)StaticContactTypeEnm.Email,
                        Name=EnumHelper<StaticContactTypeEnm>.GetDisplayValue(StaticContactTypeEnm.Email)
                    },
                    new StaticContactType()
                    {
                        CreateDate=datetimenow,
                        Id=(int)StaticContactTypeEnm.Location,
                        Name=EnumHelper<StaticContactTypeEnm>.GetDisplayValue(StaticContactTypeEnm.Location)
                    },

                });

                await context.SaveChangesAsync();
            }


        }
    }
}
