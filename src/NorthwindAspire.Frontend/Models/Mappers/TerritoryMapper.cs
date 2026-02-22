using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class TerritoryMapper : IMapper<Territory, TerritoryViewModel>
{
    public TerritoryViewModel MapToViewModel(Territory model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new TerritoryViewModel
        {
            TerritoryId = model.TerritoryId,
            TerritoryDescription = model.TerritoryDescription,
            RegionId = model.RegionId,
            RegionDescription = model.Region?.RegionDescription ?? string.Empty,
            EmployeeIds = model.Employees?.Select(e => e.EmployeeId).ToList() ?? new()
        };
    }

    public Territory MapToModel(TerritoryViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Territory
        {
            TerritoryId = viewModel.TerritoryId,
            TerritoryDescription = viewModel.TerritoryDescription,
            RegionId = viewModel.RegionId
        };
    }

    public IEnumerable<TerritoryViewModel> MapToViewModelList(IEnumerable<Territory> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<TerritoryViewModel>();
    }

    public IEnumerable<Territory> MapToModelList(IEnumerable<TerritoryViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Territory>();
    }
}
