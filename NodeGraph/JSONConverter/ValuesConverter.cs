using Newtonsoft.Json;
using NodeSharp.NodeGraph.NodeData;

namespace NodeSharp.JSONConverter;

public class ValuesConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Values) || (objectType.IsArray && objectType.GetElementType() == typeof(Values));
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {

        if (value is Values data)
        {
            writer.WriteStartArray();
            writer.WriteStartObject();
            
            writer.WritePropertyName("dataKey");
            writer.WriteValue(data.DataKey);
            
            writer.WritePropertyName("dataType");
            writer.WriteValue(data.DataType);

            writer.WritePropertyName("dataValue");
            WriteValue(writer, data.DataType, data.DataValue);
            
            writer.WriteEndObject();
            writer.WriteEndArray();

            writer.Flush();
            return;
        }
        else if (value is List<Values> items)
        {
            writer.WriteStartArray();
            foreach (var item in items)
            {
                if (item is not null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("dataKey");
                    writer.WriteValue(item.DataKey);

                    writer.WritePropertyName("dataType");
                    writer.WriteValue(item.DataType);

                    writer.WritePropertyName("dataValue");
                    WriteValue(writer, item.DataType, item.DataValue);
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteNull();
                }
            }
            writer.WriteEndArray();
            writer.Flush();
            return;
        }

        throw new NotImplementedException();
    }

    private void WriteValue(JsonWriter writer, string type, object value)
    {
        switch (type)
        {
            case DataType.Bool:
                writer.WriteValue((bool)value);
                break;
            
            case DataType.UShort: 
                writer.WriteValue(Convert.ToUInt16(value));
                break;
            
            case DataType.Int: 
                writer.WriteValue((int)value);
                break;
            
            case DataType.Float: 
                writer.WriteValue((float)value);
                break;
            
            default :
                throw new NotImplementedException();
        }
    }
}