namespace Appy.Configuration.Validation;

public static class ValidationExtensions
{
    public static void ValidateAndThrow<TModel>(this IValidator validator, TModel model)
    {
        var result = validator.Validate(model);

        if (!result.IsValid)
        {
            throw new ValidationException(typeof(TModel).Name, result);
        }
    }
}