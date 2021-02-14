using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Models;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<User> SetupUser(AppDBContext context, IMapper mapper, IHttpContextAccessor http, UserRegisterDto userdto)
        {
            // Stand in User
            var auth = new AuthService(http, context, mapper, SmileShop.Program.Configuration);
            await auth.Register(userdto);
            User user = await context.Users.SingleOrDefaultAsync(x => x.Username.ToLower().Equals(userdto.Username.ToLower()));

            return user;
        }

        public async Task<User> Generate_ProductGroup_Data(AppDBContext context, IMapper mapper, IHttpContextAccessor http)
        {
            var user = await SetupUser(context, mapper, http, new UserRegisterDto { Username = "Test", Password = "Test" });

            // Stand in Product Group
            List<ProductGroup> productGroups = new List<ProductGroup> {
                new ProductGroup { Name = "Test Product Group 1", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 2", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 3", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 4", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
                new ProductGroup { Name = "Test Product Group 5", CreatedByUserId = user.Id, CreatedDate = DateTime.Now },
            };

            // Add Product Group
            await context.ProductGroup.AddRangeAsync(productGroups);
            await context.SaveChangesAsync();

            return user;
        }


        public async Task<User> Generate_Product_Data(AppDBContext context, IMapper mapper, IHttpContextAccessor http)
        {
            var user = await Generate_ProductGroup_Data(context, mapper, http);

            // Stand in Product
            var product = new List<Product> {
                new Product {Id = 1, GroupId = 1, Name = "Test Product 1 Group A", Price = 10, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 2, GroupId = 1, Name = "Test Product 2 Group A", Price = 20, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 3, GroupId = 1, Name = "Test Product 3 Group A", Price = 30, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 4, GroupId = 2, Name = "Test Product 1 Group B", Price = 20, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 5, GroupId = 2, Name = "Test Product 2 Group B", Price = 30, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 6, GroupId = 2, Name = "Test Product 3 Group B", Price = 40, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 7, GroupId = 3, Name = "Test Product 1 Group C", Price = 30, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 8, GroupId = 3, Name = "Test Product 2 Group C", Price = 40, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 9, GroupId = 3, Name = "Test Product 3 Group C", Price = 50, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 10, GroupId = 4, Name = "Test Product 1 Group D", Price = 40, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 11, GroupId = 4, Name = "Test Product 2 Group D", Price = 50, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 12, GroupId = 4, Name = "Test Product 3 Group D", Price = 60, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 13, GroupId = 5, Name = "Test Product 1 Group E", Price = 50, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 14, GroupId = 5, Name = "Test Product 2 Group E", Price = 60, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 15, GroupId = 5, Name = "Test Product 3 Group E", Price = 70, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
            };

            // Add Product Group
            await context.Product.AddRangeAsync(product);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
