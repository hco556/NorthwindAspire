using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class EmployeeTerritoryMapper : IMapper<EmployeeTerritory, EmployeeTerritoryViewModel>
{
    public EmployeeTerritoryViewModel MapToViewModel(EmployeeTerritory model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new EmployeeTerritoryViewModel
        {
            EmployeeId = model.EmployeeId,
            EmployeeName = GetEmployeeName(model.Employee),
            TerritoryId = model.TerritoryId,
            TerritoryDescription = model.Territory?.TerritoryDescription ?? string.Empty
        };
    }

    public EmployeeTerritory MapToModel(EmployeeTerritoryViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new EmployeeTerritory
        {
            EmployeeId = viewModel.EmployeeId,
            TerritoryId = viewModel.TerritoryId
        };
    }

    public IEnumerable<EmployeeTerritoryViewModel> MapToViewModelList(IEnumerable<EmployeeTerritory> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<EmployeeTerritoryViewModel>();
    }

    public IEnumerable<EmployeeTerritory> MapToModelList(IEnumerable<EmployeeTerritoryViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<EmployeeTerritory>();
    }

    private static string GetEmployeeName(Employee? employee)
    {
        if (employee == null)
            return string.Empty;
        return $"{employee.FirstName} {employee.LastName}".Trim();
    }
}
