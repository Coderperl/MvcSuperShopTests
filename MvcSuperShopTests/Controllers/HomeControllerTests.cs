using System;
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
using MvcSuperShop.Infrastructure.Context;
using MvcSuperShop.Services;
using MvcSuperShop.ViewModels;

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

            var agreements = new UserAgreements()
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
            _context.UserAgreements.Add(agreements);
            _context.SaveChanges();
            //Act
            var result = _sut.Index() as ViewResult;
            var viewName = result.ViewName;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(viewName)|| viewName == "Index");
        }
        [TestMethod]
        public void Index_should_return_5_trendingCategories()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "CoderPerl@github.com")
            }, "TestAuthentication"));

            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = user
            };

            _categoryServiceMock.Setup(e => e.GetTrendingCategories(3))
                .Returns(new List<Category>
                {
                new Category(),
                new Category(),
                new Category(),
                new Category(),
                new Category()
                });

            _mapperMock.Setup(m => m.Map<List<CategoryViewModel>>(It.IsAny<List<Category>>())).Returns(
                new List<CategoryViewModel>
                {
                new CategoryViewModel(),
                new CategoryViewModel(),
                new CategoryViewModel(),
                new CategoryViewModel(),
                new CategoryViewModel(),
                });

            //Act
            var result = _sut.Index() as ViewResult;
            var model = result.Model as HomeIndexViewModel;

            //Assert
            Assert.AreEqual(5, model.TrendingCategories.Count);
        }
        [TestMethod]
        public void Index_should_show_5_products()
        {
            //Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "CoderPerl@github.com")
            }, "TestAuthentication"));

            _sut.ControllerContext = new ControllerContext();
            _sut.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = user
            };

            _productServiceMock.Setup(e => e.GetNewProducts(5, new CurrentCustomerContext()))
                .Returns(new List<ProductServiceModel>
                {
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                new ProductServiceModel(),
                
                });


            _mapperMock.Setup(m => m.Map<IEnumerable<ProductBoxViewModel>>(It.IsAny<IEnumerable<ProductServiceModel>>())).Returns(
                new List<ProductBoxViewModel>
                {
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                new ProductBoxViewModel(),
                });

            //Act
            var result = _sut.Index() as ViewResult;
            var model = result.Model as HomeIndexViewModel;

            //Assert
            Assert.AreEqual(5, model.NewProducts.Count);
        }
    }
}
