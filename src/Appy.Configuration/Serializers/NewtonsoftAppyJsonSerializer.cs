using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Appy.Configuration.Serializers
{
    public class NewtonsoftAppyJsonSerializer : IAppyJsonSerializer
    {
        readonly JsonSerializerSettings _defaultSettings;

        public NewtonsoftAppyJsonSerializer()
            : this(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            })
        { }

        public NewtonsoftAppyJsonSerializer(JsonSerializerSettings settings)
        {
            _defaultSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string Serialize(object value) => JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, _defaultSettings);

        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _defaultSettings)!;
    }
}