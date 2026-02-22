namespace NorthwindAspire.Frontend.Models.Mappers;

/// <summary>
/// Represents a bidirectional mapper between a ViewModel and a Data Model.
/// This interface extends IMapper and adds reverse mapping capability for Create and Update operations.
/// </summary>
public interface IReverseMapper<TViewModel, TModel> : IMapper<TModel, TViewModel>
    where TModel : class
    where TViewModel : class
{
    /// <summary>
    /// Maps a ViewModel to a Data Model.
    /// This is the reverse direction mapping used in Create and Update operations.
    /// </summary>
    TModel MapViewModelToModel(TViewModel viewModel);
}
