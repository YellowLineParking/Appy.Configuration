namespace Appy.Infrastructure.OnePassword.Tooling
{
    public static class OnePasswordToolExceptionExtensions
    {
        public static TContent GetContentAs<TContent>(this OnePasswordToolException exception)
        {
            return (TContent)exception.Result;
        }
    }
}