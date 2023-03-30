namespace Appy.Configuration.Validation;

public interface IValidationProvider
{
    IValidator<TModel> GetValidator<TModel>();
}