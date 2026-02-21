using Newtonsoft.Json;
using System;

namespace Silksong.PurenailUtil.Collections.Json;

internal class AbstractJsonConvertibleConverter : JsonConverter<AbstractJsonConvertible>
{
    public override AbstractJsonConvertible? ReadJson(JsonReader reader, Type objectType, AbstractJsonConvertible? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        AbstractJsonConvertible result = hasExistingValue ? existingValue! : (AbstractJsonConvertible)Activator.CreateInstance(objectType);
        var rep = serializer.Deserialize(reader, result.GetRepType());
        result.ReadRepRaw(rep!);
        return result;
    }

    public override void WriteJson(JsonWriter writer, AbstractJsonConvertible? value, JsonSerializer serializer)
    {
        if (value != null) serializer.Serialize(writer, value.ConvertToRepRaw(), value.GetRepType());
        else serializer.Serialize(writer, null);
    }
}
