using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using System;
using AutoMapper;
using SmileShop.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using SmileShop.Services;
using System.Threading.Tasks;

namespace SmileShop.Test
{
    [TestClass]
    public class ProductGroupServiceTest
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
        /// <summary>
        /// If Product Group method GetAll has no data in database
        /// it must return suscess with error message
        /// </summary>
        [TestMethod]
        public async Task ProductGroupGetAllHasNoData()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = BuildContext(dbName);
            var mapper = BuildMap();
            var httpContext = new Mock<IHttpContextAccessor>();
            
            var service = new ProductGroupServices(context, mapper, httpContext.Object);

            var response = await service.GetAll(null, null, null);

            var result = response.IsSuccess;

            Assert.AreEqual(result, false);
        }
    }
}
