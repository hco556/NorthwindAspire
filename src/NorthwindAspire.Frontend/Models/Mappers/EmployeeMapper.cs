using NorthwindAspire.Backend.Models;
using NorthwindAspire.Frontend.Models.ViewModels;

namespace NorthwindAspire.Frontend.Models.Mappers;

public class EmployeeMapper : IMapper<Employee, EmployeeViewModel>
{
    public EmployeeViewModel MapToViewModel(Employee model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        return new EmployeeViewModel
        {
            EmployeeId = model.EmployeeId,
            LastName = model.LastName,
            FirstName = model.FirstName,
            Title = model.Title,
            TitleOfCourtesy = model.TitleOfCourtesy,
            BirthDate = model.BirthDate,
            HireDate = model.HireDate,
            Address = model.Address,
            City = model.City,
            Region = model.Region,
            PostalCode = model.PostalCode,
            Country = model.Country,
            HomePhone = model.HomePhone,
            Extension = model.Extension,
            PhotoPath = model.PhotoPath,
            Notes = model.Notes,
            ReportsTo = model.ReportsTo,
            ManagerName = GetManagerName(model.Manager),
            SubordinateIds = model.Subordinates?.Select(s => s.EmployeeId).ToList() ?? new(),
            OrderIds = model.Orders?.Select(o => o.OrderId).ToList() ?? new(),
            TerritoryIds = model.Territories?.Select(t => t.TerritoryId).ToList() ?? new()
        };
    }

    public Employee MapToModel(EmployeeViewModel viewModel)
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        return new Employee
        {
            EmployeeId = viewModel.EmployeeId,
            LastName = viewModel.LastName,
            FirstName = viewModel.FirstName,
            Title = viewModel.Title,
            TitleOfCourtesy = viewModel.TitleOfCourtesy,
            BirthDate = viewModel.BirthDate,
            HireDate = viewModel.HireDate,
            Address = viewModel.Address,
            City = viewModel.City,
            Region = viewModel.Region,
            PostalCode = viewModel.PostalCode,
            Country = viewModel.Country,
            HomePhone = viewModel.HomePhone,
            Extension = viewModel.Extension,
            PhotoPath = viewModel.PhotoPath,
            Notes = viewModel.Notes,
            ReportsTo = viewModel.ReportsTo
        };
    }

    public IEnumerable<EmployeeViewModel> MapToViewModelList(IEnumerable<Employee> models)
    {
        return models?.Select(MapToViewModel) ?? Enumerable.Empty<EmployeeViewModel>();
    }

    public IEnumerable<Employee> MapToModelList(IEnumerable<EmployeeViewModel> viewModels)
    {
        return viewModels?.Select(MapToModel) ?? Enumerable.Empty<Employee>();
    }

    private static string GetManagerName(Employee? manager)
    {
        if (manager == null)
            return string.Empty;
        return $"{manager.FirstName} {manager.LastName}".Trim();
    }
}
