using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Siemens.Simatic.Web.PortalApi.Infrustrues
{
    /// <summary>
    /// 自定义序列化和反序列化转换器
    /// </summary>
    public class CustomJsonConverter : JsonConverter
    {
        //ILog log = LogManager.GetLogger(typeof(CustomJsonConverter));//写日志
        /// <summary>
        /// 用指定的值替换空值NULL
        /// </summary>
        public object PropertyNullValueReplaceValue { get; set; }

        /// <summary>
        /// 从字符流读取对象
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            using (JTokenWriter writer = new JTokenWriter())
            {
                JsonReaderToJsonWriter(reader, writer);

                return writer.Token.ToObject(objectType);
            }
        }

        /// <summary>
        /// 通过对象写字符流
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            JsonReader reader = t.CreateReader();
            JsonReaderToJsonWriter(reader, writer);
        }

        public void JsonReaderToJsonWriter(JsonReader reader, JsonWriter writer)
        {
            do
            {
                //log.Info("reader.TokenType-->" + reader.TokenType);
                switch (reader.TokenType)
                {
                    case JsonToken.None:
                        break;
                    case JsonToken.StartObject:
                        writer.WriteStartObject();
                        break;
                    case JsonToken.StartArray:
                        writer.WriteStartArray();
                        break;
                    case JsonToken.PropertyName:
                        writer.WritePropertyName(reader.Value.ToString());
                        break;
                    case JsonToken.Comment:
                        writer.WriteComment((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                    //数字/字符/布尔型/GUID都转成string
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Bytes:
                        writer.WriteValue(reader.Value.ToString());
                        break;
                    case JsonToken.Null:
                        if (this.PropertyNullValueReplaceValue != null)
                        {
                            writer.WriteValue(this.PropertyNullValueReplaceValue);
                        }
                        else
                        {
                            writer.WriteNull();
                        }
                        break;
                    case JsonToken.Undefined:
                        writer.WriteUndefined();
                        break;
                    case JsonToken.EndObject:
                        writer.WriteEndObject();
                        break;
                    case JsonToken.EndArray:
                        writer.WriteEndArray();
                        break;
                    case JsonToken.EndConstructor:
                        writer.WriteEndConstructor();
                        break;
                    case JsonToken.Date:
                        if (reader.Value != null)
                        {
                            writer.WriteValue(((DateTime)reader.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        break;
                    case JsonToken.Raw:
                        writer.WriteRawValue((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                }
            } while (reader.Read());
        }

        /// <summary>
        /// 自定义转换器是否可用
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}