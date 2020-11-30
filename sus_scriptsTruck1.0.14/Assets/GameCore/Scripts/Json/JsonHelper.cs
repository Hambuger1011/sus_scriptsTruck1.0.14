using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System;

public static class JsonHelper
{
    /// <summary>
    /// 字符串转Primitive
    /// </summary>
    public class StringPrimitiveConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType.IsPrimitive;
        }

        /// <summary>
        /// 字符串转:IsPrimitive
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Null)
            {
                try
                {
                    var result = Convert.ChangeType(reader.Value, objectType, System.Globalization.CultureInfo.InvariantCulture);
                    return result;
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value.ToString());
        }
    }


    public class NullToStringConverter : JsonConverter
    {
        /// <summary>
        /// Null to String
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return objectType != null && objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Null)
            {
                var result = reader.Value.ToString();
                return result;
            }
            return "";
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("");
                return;
            }
            writer.WriteValue(value.ToString());
        }
    }

    static JsonHelper()
    {
        LOG.Error("JsonHelper ctor()");
        Init();
    }

//#if UNITY_EDITOR

//    [UnityEditor.InitializeOnLoadMethod]
//#else
//    [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//#endif

//    public static void OnGameStartUp()
//    {
//        Init();
//    }

    static void Init()
    {
        Newtonsoft.Json.JsonSerializerSettings jsonSetting = new Newtonsoft.Json.JsonSerializerSettings();
        JsonConvert.DefaultSettings = () =>
        {
            //日期类型默认格式化处理
            jsonSetting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            jsonSetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            //空值处理
            jsonSetting.NullValueHandling = NullValueHandling.Ignore;//不返回空值，跳过空值序列化
            jsonSetting.Converters.Add(new StringEnumConverter());
            jsonSetting.Converters.Add(new ColorConverter());
            jsonSetting.Converters.Add(new StringPrimitiveConverter());
            jsonSetting.Converters.Add(new NullToStringConverter());
            return jsonSetting;
        };
    }

    public static T JsonToObject<T>(string json)
    {
        try
        {
            T data = JsonConvert.DeserializeObject<T>(json);
            return data;
        }
        catch (Exception e)
        {
            LOG.Error("解析json错误:type=" + typeof(T) + ",json=" + json);
            LOG.Error(e);
        }
        return default(T);
    }


    public static object JsonToObject(string json,Type type)
    {
        try
        {
            object data = JsonConvert.DeserializeObject(json, type);
            return data;
        }
        catch (Exception e)
        {
            LOG.Error("解析json错误:type=" + type + ",json=" + json);
            LOG.Error(e);
        }
        return null;
    }


    public static string ObjectToJson(object o, Formatting formatting = Formatting.None)
    {
        try
        {
            var json = JsonConvert.SerializeObject(o, formatting);
            return json;
        }
        catch (Exception e)
        {
            LOG.Error("json序列化错误:object=" + o);
            LOG.Error(e);
        }
        return string.Empty;
    }

    public static JsonObject JsonToJObject(string json)
    {
        try
        {
            JObject data = JsonConvert.DeserializeObject<JObject>(json);
            JsonObject jo = new JsonObject();
            jo.code = int.Parse(data["code"].ToString());
            jo.msg = data["msg"].ToString();
            return jo;
        }
        catch (Exception e)
        {
            LOG.Error("解析json错误:type=" + typeof(JObject) + ",json=" + json);
            LOG.Error(e);
        }
        return null;
    }

}

public class JsonObject
{
    public int code;
    public string msg;
}
