using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class ProductMapper : IMapper<Product, ProductViewModel>
{
    public ProductViewModel MapToViewModel(Product model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new ProductViewModel
        {
            ProductId = model.ProductId,
            ProductName = model.ProductName,
            SupplierId = model.SupplierId,
            SupplierName = model.Supplier?.CompanyName ?? string.Empty,
            CategoryId = model.CategoryId,
            CategoryName = model.Category?.CategoryName ?? string.Empty,
            QuantityPerUnit = model.QuantityPerUnit.ToString(),
            UnitPrice = model.UnitPrice,
            UnitsInStock = model.UnitsInStock,
            UnitsOnOrder = model.UnitsOnOrder,
            ReorderLevel = model.ReorderLevel,
            Discontinued = model.Discontinued == "1" || model.Discontinued.ToLower() == "true"
        };
    }

    public Product MapToModel(ProductViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Product
        {
            ProductId = viewModel.ProductId,
            ProductName = viewModel.ProductName,
            SupplierId = viewModel.SupplierId,
            CategoryId = viewModel.CategoryId,
            QuantityPerUnit = int.TryParse(viewModel.QuantityPerUnit, out var qty) ? qty : 0,
            UnitPrice = viewModel.UnitPrice,
            UnitsInStock = viewModel.UnitsInStock,
            UnitsOnOrder = viewModel.UnitsOnOrder,
            ReorderLevel = viewModel.ReorderLevel,
            Discontinued = viewModel.Discontinued ? "1" : "0"
        };
    }

    public IEnumerable<ProductViewModel> MapToViewModelList(IEnumerable<Product> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<ProductViewModel>();
    }

    public IEnumerable<Product> MapToModelList(IEnumerable<ProductViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Product>();
    }
}
