namespace Appy.Configuration.Serializers;

public interface IAppyJsonSerializer
{
    string Serialize(object value);
    T Deserialize<T>(string value);
}