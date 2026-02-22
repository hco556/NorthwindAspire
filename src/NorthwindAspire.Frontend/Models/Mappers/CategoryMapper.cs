using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class CategoryMapper : IMapper<Category, CategoryViewModel>
{
    public CategoryViewModel MapToViewModel(Category model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new CategoryViewModel
        {
            CategoryId = model.CategoryId,
            CategoryName = model.CategoryName,
            Description = model.Description,
            ProductIds = model.Products?.Select(p => p.ProductId).ToList() ?? new()
        };
    }

    public Category MapToModel(CategoryViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Category
        {
            CategoryId = viewModel.CategoryId,
            CategoryName = viewModel.CategoryName,
            Description = viewModel.Description
        };
    }

    public IEnumerable<CategoryViewModel> MapToViewModelList(IEnumerable<Category> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<CategoryViewModel>();
    }

    public IEnumerable<Category> MapToModelList(IEnumerable<CategoryViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Category>();
    }
}
