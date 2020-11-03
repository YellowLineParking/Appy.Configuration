namespace Appy.Configuration.Validation
{
    public interface IValidator<in TModel>
    {
        ValidationResult Validate(TModel model);
    }

    public interface IValidator
    {
        ValidationResult Validate<TModel>(TModel model);
    }
}