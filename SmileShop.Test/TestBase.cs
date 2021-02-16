using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Models;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);

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
                new Product {Id = 1, GroupId = 1, Name = "Test Product 1 Group A", Price = 10, StockCount = 40, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 2, GroupId = 1, Name = "Test Product 2 Group A", Price = 20, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 3, GroupId = 1, Name = "Test Product 3 Group A", Price = 30, StockCount = 10, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 4, GroupId = 2, Name = "Test Product 1 Group B", Price = 20, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 5, GroupId = 2, Name = "Test Product 2 Group B", Price = 30, StockCount = 25, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 6, GroupId = 2, Name = "Test Product 3 Group B", Price = 40, StockCount = 40, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 7, GroupId = 3, Name = "Test Product 1 Group C", Price = 30, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 8, GroupId = 3, Name = "Test Product 2 Group C", Price = 40, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 9, GroupId = 3, Name = "Test Product 3 Group C", Price = 50, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 10, GroupId = 4, Name = "Test Product 1 Group D", Price = 40, StockCount = 0, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 11, GroupId = 4, Name = "Test Product 2 Group D", Price = 50, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 12, GroupId = 4, Name = "Test Product 3 Group D", Price = 60, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 13, GroupId = 5, Name = "Test Product 1 Group E", Price = 50, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 14, GroupId = 5, Name = "Test Product 2 Group E", Price = 60, StockCount = 20, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
                new Product {Id = 15, GroupId = 5, Name = "Test Product 3 Group E", Price = 70, StockCount = 30, CreatedDate = DateTime.Now, Status = true, CreatedByUserId = user.Id},
            };

            // Add Product Group
            await context.Product.AddRangeAsync(product);
            await context.SaveChangesAsync();

            return user;
        }
        public async Task<User> Generate_Stock_Data(AppDBContext context, IMapper mapper, IHttpContextAccessor http)
        {
            var user = await Generate_Product_Data(context, mapper, http);

            // Stand in Stock
            var stock = new List<Stock>
            {
                new Stock {Id = 1,ProductId = 1, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 2,ProductId = 2, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 3,ProductId = 3, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 4,ProductId = 4, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 5,ProductId = 5, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 6,ProductId = 6, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 7,ProductId = 6, Debit = 20, Credit = 0, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 8,ProductId = 8, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 9,ProductId = 9, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 10,ProductId = 10, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 11,ProductId = 11, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 12,ProductId = 12, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 13,ProductId = 13, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 14,ProductId = 14, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 15,ProductId = 15, Debit = 20, Credit = 0, StockBefore = 0, Remark = "Inititial Stock", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 16,ProductId = 1, Debit = 20, Credit = 0, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 17,ProductId = 3, Debit = 0, Credit = 10, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 18,ProductId = 5, Debit = 5, Credit = 0, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 19,ProductId = 10, Debit = 0, Credit = 20, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                new Stock {Id = 20,ProductId = 15, Debit = 10, Credit = 0, StockBefore = 20, Remark = "Stock Set", CreatedByUserId = user.Id, CreatedDate = DateTime.Now},
                
            };

            // Add Product Group
            await context.Stock.AddRangeAsync(stock);
            await context.SaveChangesAsync();

            return user;
        }


        protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string databaseName)
        {
            var factory = new WebApplicationFactory<Startup>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptorDbContext = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDBContext>));


                    if (descriptorDbContext != null)
                    {
                        services.Remove(descriptorDbContext);
                    }

                    services.AddDbContext<AppDBContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });
            });

            return factory;
        }

        protected async Task<string> SimulateLogin(HttpClient client)
        {

            var loginUrl = "/api/Auth/login";

            var loginUser = new UserLoginDto { Username = "Test", Password = "Test" };
            var loginPayload = await Task.Run(() => JsonConvert.SerializeObject(loginUser));
            var loginContent = new StringContent(loginPayload, Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync(loginUrl, loginContent);
            var baerer = JsonConvert.DeserializeObject<ServiceResponse<string>>(await loginResponse.Content.ReadAsStringAsync());

            return baerer.Data;
        }

        protected async Task<HttpContent> ObjectToJsonContent(object obj)
        {
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(obj));
            return new StringContent(stringPayload, Encoding.UTF8, "application/json");
        }
    }
}
