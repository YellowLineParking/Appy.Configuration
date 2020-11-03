using System;
using System.Threading;
using System.Threading.Tasks;
using Appy.Configuration.Validation;
using Appy.Infrastructure.OnePassword.Commands;
using Appy.Infrastructure.OnePassword.Queries;

namespace Appy.Infrastructure.OnePassword.Tooling
{
    public class OnePasswordToolValidationDecorator: IOnePasswordTool
    {
        readonly IOnePasswordTool _innerTool;
        readonly IValidator _validator;
        
        public OnePasswordToolValidationDecorator(
            IOnePasswordTool innerTool,
            IValidator validator)
        {
            _innerTool = innerTool ?? throw new ArgumentNullException(nameof(innerTool));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<GetOnePasswordNoteQueryResult> Execute(GetOnePasswordNoteQuery query, CancellationToken cancellationToken = default)
        {
            _validator.ValidateAndThrow(query);
            return await _innerTool.Execute(query, cancellationToken);
        }

        public Task<GetOnePasswordVaultsQueryResult> Execute(GetOnePasswordVaultsQuery query, CancellationToken cancellationToken = default)
        {
            _validator.ValidateAndThrow(query);
            return _innerTool.Execute(query, cancellationToken);
        }

        public Task<SignInOnePasswordResult> Execute(SignInOnePasswordCommand command, CancellationToken cancellationToken = default)
        {
            _validator.ValidateAndThrow(command);
            return _innerTool.Execute(command, cancellationToken);
        }
    }
}