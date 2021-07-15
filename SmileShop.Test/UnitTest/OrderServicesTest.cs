using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Models;
using SmileShop.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmileShop.Test.UnitTest
{
    /**
     * Test Name : OrderServiceTest
     * Created by : AkiAkira
     * Version 1.0 : 21 Feb 2021.
     * 
     * Test on OrderService
     */

    [TestClass]
    public class OrderServicesTest : TestBase
    {
        private protected string _dbname { get; private set; }
        private protected AppDBContext _context { get; private set; }
        private protected IMapper _mapper { get; private set; }
        private protected Mock<IHttpContextAccessor> _http { get; private set; }

        public OrderServicesTest()
        {
            _dbname = Guid.NewGuid().ToString();
            _context = BuildContext(_dbname);
            _mapper = BuildMap();
            _http = new Mock<IHttpContextAccessor>();
            var http = new DefaultHttpContext();
            _http.Setup(_ => _.HttpContext).Returns(http);
        }

        public async Task<ServiceResponseWithPagination<List<OrderDTO>>> GetAll_Test(PaginationDto pagination = null, OrderFilterDTO OrderFilter = null, DataOrderDTO ordering = null)
        {
            var stockService = new StockServices(_context, _mapper, _http.Object);
            var service = new OrderServices(_context, _mapper, _http.Object, stockService);
            var result = await service.GetAll(pagination, OrderFilter, ordering);

            return result;
        }

        [TestMethod, TestCategory("No data"), TestCategory("GetAll")]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetAll_IsNoData_ReturnError()
        {

            // ===== Arrange =====
            // No Needed

            //Console.WriteLine(_dbname);
            // ===== Act ======
            await GetAll_Test(new PaginationDto(), new OrderFilterDTO(), new DataOrderDTO());

            // ===== Assert ======
            // Expect Exception
        }

        [TestMethod, TestCategory("Have data"), TestCategory("GetAll")]
        public async Task GetAll_HaveData_ReturnData()
        {

            // ===== Arrange =====
            var user = await Generate_Order_Data(_context, _mapper, _http.Object);

            //Console.WriteLine(_dbname);
            // ===== Act ======
            var result = await GetAll_Test(new PaginationDto(), new OrderFilterDTO(), new DataOrderDTO());

            // ===== Assert ======
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(3, result.Data.Count);
        }
        /*[TestMethod()]
        public void Get_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetOrderDetails_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Add_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Delete_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Filter_Test()
        {
            Assert.Fail();
        }*/
    }
}