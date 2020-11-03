namespace Appy.Infrastructure.OnePassword.Model
{
    public class OnePasswordInternalField
    {
        public string? T { get; set; }
        public string? V { get; set; }
        
        public static OnePasswordInternalField New(string name, string value)
        {
            return new OnePasswordInternalField
            {
                T = name,
                V = value
            };
        }
    }
}