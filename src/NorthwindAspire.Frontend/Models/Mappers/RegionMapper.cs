using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class RegionMapper : IMapper<Region, RegionViewModel>
{
    public RegionViewModel MapToViewModel(Region model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new RegionViewModel
        {
            RegionId = model.RegionId,
            RegionDescription = model.RegionDescription,
            TerritoryIds = model.Territories?.Select(t => t.TerritoryId).ToList() ?? new()
        };
    }

    public Region MapToModel(RegionViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Region
        {
            RegionId = viewModel.RegionId,
            RegionDescription = viewModel.RegionDescription
        };
    }

    public IEnumerable<RegionViewModel> MapToViewModelList(IEnumerable<Region> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<RegionViewModel>();
    }

    public IEnumerable<Region> MapToModelList(IEnumerable<RegionViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Region>();
    }
}
