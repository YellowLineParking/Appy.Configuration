using System;
using Microsoft.Extensions.DependencyInjection;

namespace Appy.Configuration.Validation;

public class DefaultValidationProvider : IValidationProvider
{
    readonly IServiceProvider _serviceProvider;
    public DefaultValidationProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IValidator<TModel> GetValidator<TModel>() =>
        _serviceProvider.GetRequiredService<IValidator<TModel>>();
}