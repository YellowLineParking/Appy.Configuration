using System;
using Newtonsoft.Json;

namespace Appy.Configuration.Serializers
{
    public class NewtonsoftConfigurationJsonSerializer : IConfigurationJsonSerializer
    {
        readonly JsonSerializerSettings _defaultSettings;

        public NewtonsoftConfigurationJsonSerializer()
            : this(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
        { }

        public NewtonsoftConfigurationJsonSerializer(JsonSerializerSettings settings)
        {
            _defaultSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public string Serialize(object value) => JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.Indented, _defaultSettings);

        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _defaultSettings)!;
    }
}