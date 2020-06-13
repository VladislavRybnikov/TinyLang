using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.JSON
{
    public class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType != typeof(Expr))
                return null; 
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ExprConverter : JsonConverter<Expr>
    {
        public override Expr ReadJson(JsonReader reader, Type objectType, [AllowNull] Expr existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            Expr expr;
            var nodeType = jsonObject["NodeType"].Value<string>();

            var types = Assembly.GetExecutingAssembly().GetTypes();
            var type = types.FirstOrDefault(t => t.Name.Equals(nodeType + "Expr", StringComparison.InvariantCultureIgnoreCase));
            expr = JsonConvert.DeserializeObject(JObject.FromObject(jsonObject).ToString(), type,
                new JsonSerializerSettings
                {
                    ContractResolver = new CustomContractResolver()
                }) as Expr;

            return expr;
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Expr value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
