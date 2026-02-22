namespace NorthwindAspire.Frontend.Models.Mappers;

public interface IMapper<TModel, TViewModel>
{
    TViewModel MapToViewModel(TModel model);
    TModel MapToModel(TViewModel viewModel);
    IEnumerable<TViewModel> MapToViewModelList(IEnumerable<TModel> models);
    IEnumerable<TModel> MapToModelList(IEnumerable<TViewModel> viewModels);
}
