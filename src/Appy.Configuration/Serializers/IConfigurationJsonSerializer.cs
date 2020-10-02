namespace Appy.Configuration.Serializers
{
    public interface IConfigurationJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
    }
}
