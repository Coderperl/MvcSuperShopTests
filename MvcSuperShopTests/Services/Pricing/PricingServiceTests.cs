using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;
using MvcSuperShop.Services;

namespace MvcSuperShopTests.Services
{
    [TestClass]
    public class PricingServiceTests
    {
        private PricingService _sut;
        [TestInitialize]
        public void Initialize()
        {
            _sut = new PricingService();
        }

        [TestMethod]
        public void when_no_agreement_exists_product_baseprice_is_assigned()
        {
            //Arrange
            var productList = new List<ProductServiceModel>()
            {
                new ProductServiceModel{BasePrice = 909110}
            };

            var customerContext = new CurrentCustomerContext()
            {
                Agreements = new List<Agreement>()
            };
            //Act
            var products = _sut.CalculatePrices(productList, customerContext);

            //Assert
            Assert.AreEqual(909110,products.First().Price);
        }

        [TestMethod]
        public void When_agreement_exists_product_price_is_reduced_by_6_percent()
        {
            //Arrange
            var productList = new List<ProductServiceModel>()
            {
                new ProductServiceModel{BasePrice = 100000}
            };

            var customerContext = new CurrentCustomerContext()
            {
                Agreements = new List<Agreement>()
                {
                    new Agreement
                    {
                        AgreementRows = new List<AgreementRow>()
                        {
                            new AgreementRow()
                            {
                                PercentageDiscount = 6.0m
                            }
                        }
                    }
                }
            };
            //Act
            var products = _sut.CalculatePrices(productList, customerContext);

            //Assert
            Assert.AreEqual(94000.0m, products.First().Price);
        }
        [TestMethod]
        public void When_agreement_exists_and_categoryMatch_is_volvo_price_is_reduced_by_6_percent()
        {
            //Arrange
            var productList = new List<ProductServiceModel>()
            {
                new ProductServiceModel{BasePrice = 100000,CategoryName = "Volvo"}
            };

            var customerContext = new CurrentCustomerContext()
            {
                Agreements = new List<Agreement>()
                {
                    new Agreement
                    {
                        
                        AgreementRows = new List<AgreementRow>()
                        {
                            new AgreementRow()
                            {
                                PercentageDiscount = 6.0m,
                                CategoryMatch = "Volvo",
                            }
                        }
                    }
                }
            };
            //Act
            var products = _sut.CalculatePrices(productList, customerContext);

            //Assert
            Assert.AreEqual(94000.0m, products.First().Price);
        }
        [TestMethod]
        public void When_agreement_exists_and_ManufacturerMatch_is_Toyota_price_is_reduced_by_10_percent()
        {
            //Arrange
            var productList = new List<ProductServiceModel>()
            {
                new ProductServiceModel{BasePrice = 100000, ManufacturerName = "Toyota"}
            };

            var customerContext = new CurrentCustomerContext()
            {
                Agreements = new List<Agreement>()
                {
                    new Agreement
                    {
                        AgreementRows = new List<AgreementRow>()
                        {
                            new AgreementRow()
                            {
                                PercentageDiscount = 10.0m,
                                ManufacturerMatch = "Toyota"
                            }
                        }
                    }
                }
            };
            //Act
            var products = _sut.CalculatePrices(productList, customerContext);

            //Assert
            Assert.AreEqual(90000.0m, products.First().Price);
        }
        [TestMethod]
        public void When_agreement_exists_and_Manufacturername_is_Toyota_and_categoryname_is_van_and_productname_is_Rav_4_and_price_is_reduced_by_10_percent()
        {
            //Arrange
            var productList = new List<ProductServiceModel>()
            {
                new ProductServiceModel{BasePrice = 100000, ManufacturerName = "Toyota", CategoryName = "Van", Name = "Rav-4"}
            };

            var customerContext = new CurrentCustomerContext()
            {
                Agreements = new List<Agreement>()
                {
                    new Agreement
                    {
                        AgreementRows = new List<AgreementRow>()
                        {
                            new AgreementRow()
                            {
                                PercentageDiscount = 10.0m,
                                ManufacturerMatch = "Toyota",
                                CategoryMatch = "Van",
                                ProductMatch = "Rav-4"
                            }
                        }
                    }
                }
            };
            //Act
            var products = _sut.CalculatePrices(productList, customerContext);

            //Assert
            Assert.AreEqual(90000.0m, products.First().Price);
        }
    }
    
}
