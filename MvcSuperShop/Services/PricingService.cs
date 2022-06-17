using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;

namespace MvcSuperShop.Services;

public class PricingService : IPricingService
{
    public IEnumerable<ProductServiceModel> CalculatePrices(IEnumerable<ProductServiceModel> products,
        CurrentCustomerContext customerContext)
    {
        foreach (var product in products)
        {
            var lowestPriceSoFar = product.BasePrice;
            if (customerContext != null)
                foreach (var agreement in customerContext.Agreements)
                foreach (var agreementRow in agreement.AgreementRows)
                {
                    if (!AgreementMatches(agreementRow, product)) continue;
                    {
                        var discountPrice = CalculateDiscountPrices(product.BasePrice,
                            agreementRow.PercentageDiscount);
                        if (discountPrice < lowestPriceSoFar)
                            lowestPriceSoFar = Convert.ToInt32(Math.Round(discountPrice, 0));
                    }
                }
            product.Price = lowestPriceSoFar;
            yield return product;
        }
    }

    private decimal CalculateDiscountPrices(int productBasePrice, decimal percentageDiscount)
    {
        return (1.0m - percentageDiscount / 100.0m) * productBasePrice;
    }

    private bool AgreementMatches(AgreementRow agreementRow, ProductServiceModel product)
    {
        var productCheck = !string.IsNullOrEmpty(agreementRow.ProductMatch);
        var categoryCheck = !string.IsNullOrEmpty(agreementRow.CategoryMatch);
        var manufacturerCheck = !string.IsNullOrEmpty(agreementRow.ManufacturerMatch);
        if (productCheck && !product.Name.ToLower().Contains(agreementRow.ProductMatch.ToLower()))
            return false;
        if (categoryCheck && !product.CategoryName.ToLower().Contains(agreementRow.CategoryMatch.ToLower()))
            return false;
        if (manufacturerCheck && !product.ManufacturerName.ToLower().Contains(agreementRow.ManufacturerMatch.ToLower()))
            return false;

        return true;
    }
}