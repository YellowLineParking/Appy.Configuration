using Newtonsoft.Json;

namespace Appy.TestTools
{
    public static class AssertionExtensions
    {
        public static bool IsEquivalentTo<T>(this T obj, T other) =>
            JsonConvert.SerializeObject(obj) == JsonConvert.SerializeObject(other);
    }
}