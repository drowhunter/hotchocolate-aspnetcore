using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace HotChocolate.AspNetCore
{
    internal static class QueryMiddlewareUtilities
    {
        public static Dictionary<string, object> DeserializeVariables(
            JObject input)
        {
            if (input == null)
            {
                return null;
            }

            return DeserializeVariables(
                input.ToObject<Dictionary<string, JToken>>());
        }

        private static Dictionary<string, object> DeserializeVariables(
            Dictionary<string, JToken> input)
        {
            if (input == null)
            {
                return null;
            }

            var values = new Dictionary<string, object>();

            foreach (string key in input.Keys.ToArray())
            {
                values[key] = DeserializeVariableValue(input[key]);
            }

            return values;
        }

        private static Dictionary<string, object> DeserializeObjectValue(
            Dictionary<string, JToken> input)
        {
            if (input == null)
            {
                return null;
            }

            var fields = new Dictionary<string, object>();

            foreach (string key in input.Keys.ToArray())
            {
                fields[key] = DeserializeVariableValue(input[key]);
            }

            return fields;
        }

        private static object DeserializeVariableValue(object value)
        {
            if (value is JObject jo)
            {
                return DeserializeObjectValue(
                    jo.ToObject<Dictionary<string, JToken>>());
            }

            if (value is JArray ja)
            {
                return DeserializeVariableListValue(ja);
            }

            if (value is JValue jv)
            {
                return DeserializeVariableScalarValue(jv);
            }

            throw new NotSupportedException();
        }

        private static List<object> DeserializeVariableListValue(JArray array)
        {
            var list = new List<object>();

            foreach (JToken token in array.Children())
            {
                list.Add(DeserializeVariableValue(token));
            }

            return list;
        }

        private static object DeserializeVariableScalarValue(JValue value)
        {
            switch (value.Type)
            {
                case JTokenType.Boolean:
                    return value.Value<bool>();
                case JTokenType.Integer:
                    return value.Value<int>();
                case JTokenType.Float:
                    return value.Value<double>();
                default:
                    return value.Value<string>();
            }
        }

        public static IServiceProvider CreateRequestServices(
            HttpContext context)
        {
            Dictionary<Type, object> services = new Dictionary<Type, object>
            {
                { typeof(HttpContext), context }
            };

            return new RequestServiceProvider(
                context.RequestServices, services);
        }
    }
}
