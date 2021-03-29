using System;

namespace SeguroCaixa.Helpers
{
    public static class JsonHelper
    {
        public static string ToJson(this Object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(this string json) where T : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
