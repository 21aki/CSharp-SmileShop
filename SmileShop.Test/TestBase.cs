using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SmileShop.Test
{
    public class TestBase
    {
        /// <summary>
        /// Create new dbContext for each test.
        /// </summary>
        /// <param name="dbName"> Name new db with uniqe name for uniqe testing.</param>
        /// <returns>Project's DBContext</returns>
        protected AppDBContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(dbName).Options;

            var dbContext = new AppDBContext(options);
            return dbContext;
        }

        /// <summary>
        /// Create auto mapper from our project.
        /// </summary>
        /// <returns>AutoMapper</returns>
        protected IMapper BuildMap()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfile());
            });

            return config.CreateMapper();
        }

        protected string ClassToJsonString(object obj)
        {

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonString = JsonSerializer.Serialize(obj, options);

            return jsonString;
        }

    }
}
