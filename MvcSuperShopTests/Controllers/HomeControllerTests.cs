﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcSuperShop.Controllers;
using MvcSuperShop.Data;
using MvcSuperShop.Services;

namespace MvcSuperShopTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<IProductService> _productServiceMock;
        private Mock<IMapper> _mapperMock;
        private HomeController _sut;
        private ApplicationDbContext _context;
        [TestInitialize]
        public void TestInitialize()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _productServiceMock = new Mock<IProductService>();
            _mapperMock = new Mock<IMapper>();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("test")
                .Options;

            _context = new ApplicationDbContext(contextOptions);
            _context.Database.EnsureCreated();

            _sut = new HomeController(_categoryServiceMock.Object
                ,_productServiceMock.Object
                ,_mapperMock.Object, _context);
        }

        [TestMethod]
        public void Index_should_return_correct_view()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, "CoderPerl@github.com")
            }, "TestAuthentication"));

            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = user
            };

            var ua = new UserAgreements()
            {
                Email = "CoderPerl@github.com",
                Agreement = new Agreement()
                {
                    Name = "Huddinge Kommun",
                    AgreementRows = new List<AgreementRow>()
                    {
                        new AgreementRow()
                        {
                            CategoryMatch = "van",
                            ManufacturerMatch = "",
                            ProductMatch = "",
                            PercentageDiscount = 4
                        }
                    }
                }
            };
            _context.UserAgreements.Add(ua);
            _context.SaveChanges();
            //Act
            var result = _sut.Index() as ViewResult;
            var viewName = result.ViewName;

            Assert.IsTrue(string.IsNullOrEmpty(viewName)|| viewName == "Index");
        }
    }
}
