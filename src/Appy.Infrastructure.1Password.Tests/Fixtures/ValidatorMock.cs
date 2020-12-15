using Appy.Configuration.Validation;
using Appy.TestTools;
using Moq;

namespace Appy.Infrastructure.OnePassword.Tests.Fixtures
{
    public class ValidatorMock : Mock<IValidator>
    {
        public ValidatorMock SetupAndReturns<TModel>(ValidationResult result)
        {
            this.Setup(x => x.Validate(
                    It.IsAny<TModel>()))
                .Returns(result);
            return this;
        }

        public ValidatorMock VerifyCalledWith<TModel>(TModel expected)
        {
            Verify(x => x.Validate(
                It.Is<TModel>(q => q.IsEquivalentTo(expected))),
                Times.Once);
            return this;
        }
    }
}