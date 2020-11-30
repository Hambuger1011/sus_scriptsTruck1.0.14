using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
public class TinyJson
{
    public static string ToJson(object item)
    {
        return JSONWriter.ToJson(item);
    }
    public static T ToObject<T>(string json)
    {
        return (T)FromJson(json, typeof(T));
    }


    public static object FromJson(string json, Type type)
    {
        return JSONParser.FromJson(json, type);
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class JsonAttribute : Attribute
    {
        public string Name;
        public JsonAttribute(string name)
        {
            Name = name;
        }
    }



    static Dictionary<string, Dictionary<string, FieldInfo>> s_fields = new Dictionary<string, Dictionary<string, FieldInfo>>();
    static Dictionary<string, Dictionary<string, PropertyInfo>> s_props = new Dictionary<string, Dictionary<string, PropertyInfo>>();

    static string GetMemberName(MemberInfo member)
    {
        if (member.IsDefined(typeof(JsonAttribute), true))
        {
            JsonAttribute dataMemberAttribute = (JsonAttribute)Attribute.GetCustomAttribute(member, typeof(JsonAttribute), true);
            if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                return dataMemberAttribute.Name;
        }

        return member.Name;
    }
    public static Dictionary<string, FieldInfo> GetFields(Type type)
    {
        Dictionary<string, FieldInfo> map;
        if (!s_fields.TryGetValue(type.Name, out map))
        {
            map = new Dictionary<string, FieldInfo>();
            s_fields.Add(type.Name, map);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                if (field.FieldType == type)
                {
                    throw new Exception("死循环:" + type + "." + field.Name);
                }
                if (field.IsPublic)
                {
                    if (!field.IsDefined(typeof(NonSerializedAttribute), true))
                    {
                        map.Add(field.Name, field);
                    }
                }
                else if (field.IsDefined(typeof(SerializeField), true))
                {
                    map.Add(GetMemberName(field), field);
                }
            }
        }
        return map;
    }


    public static Dictionary<string, PropertyInfo> GetProperties(Type type)
    {
        Dictionary<string, PropertyInfo> map;
        if (!s_props.TryGetValue(type.Name, out map))
        {
            map = new Dictionary<string, PropertyInfo>();
            s_props.Add(type.Name, map);

            var arr = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var a in arr)
            {
                if (!a.CanRead || !a.CanWrite)
                {
                    continue;
                }
                if (a.GetIndexParameters().Length > 0)
                {
                    continue;
                }
                map.Add(GetMemberName(a), a);
            }
        }
        return map;
    }

    //Really simple JSON writer
    //- Outputs JSON structures from an object
    //- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
    //- Will only output public fields and property getters on objects
    public static class JSONWriter
    {
        public static string ToJson(object item)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                AppendValue(stringBuilder, item, 0);
            }
            catch (Exception ex)
            {
                stringBuilder.Length = 0;
                stringBuilder.AppendLine(ex.ToString());
                UnityEngine.Debug.LogError(ex);
            }
            return stringBuilder.ToString();
        }

        public static void AppendValue(StringBuilder stringBuilder, object item, int depth)
        {
            if (depth > 1024)
            {
                UnityEngine.Debug.LogError("栈溢出:" + depth);
                throw new Exception("栈溢出:" + depth);
            }
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            Type type = item.GetType();
            if (customWrite != null && customWrite(stringBuilder, type, item))
            {
                return;
            }

            #region string
            if (type == typeof(string))
            {
                stringBuilder.Append('"');
                string str = (string)item;
                for (int i = 0; i < str.Length; ++i)
                    if (str[i] < ' ' || str[i] == '"' || str[i] == '\\')
                    {
                        stringBuilder.Append('\\');
                        int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (UInt32)str[i]);
                    }
                    else
                        stringBuilder.Append(str[i]);
                stringBuilder.Append('"');
            }
            #endregion
            #region 数字
            //else if (type == typeof(byte) || type == typeof(int))
            //{
            //    stringBuilder.Append(item.ToString());
            //}
            else if (type == typeof(float))
            {
                stringBuilder.Append(((float)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                stringBuilder.Append(((double)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            #endregion
            else if (type == typeof(bool))
            {
                stringBuilder.Append(((bool)item) ? "true" : "false");
            }
            else if (type.IsPrimitive)
            {
                stringBuilder.Append(item.ToString());
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append('"');
                stringBuilder.Append(item.ToString());
                stringBuilder.Append('"');
            }
            #region list
            else if (item is IList)
            {
                stringBuilder.Append('[');
                bool isFirst = true;
                IList list = item as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i], depth + 1);
                }
                stringBuilder.Append(']');
            }
            #endregion
            #region dication
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                stringBuilder.Append('{');
                IDictionary dict = item as IDictionary;
                bool isFirst = true;
                foreach (object key in dict.Keys)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append(key.ToString());
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key], depth + 1);
                }
                stringBuilder.Append('}');
            }
            #endregion
            #region 类
            else
            {
                stringBuilder.Append('{');
                AppendObjectValue(stringBuilder, type, item, depth);
                stringBuilder.Append('}');
            }
            #endregion
        }

        public static void AppendObjectValue(StringBuilder stringBuilder, Type type, object item, int depth, bool isFirst = true)
        {
            var fields = TinyJson.GetFields(type);
            var props = TinyJson.GetProperties(type);
            FieldInfo field = null;
            PropertyInfo prop = null;
            try
            {
                foreach (var itr in fields)
                {
                    field = itr.Value;
                    object value = field.GetValue(item);

                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append(itr.Key);
                    stringBuilder.Append("\":");
                    if(value == null)
                    {
                        stringBuilder.Append("null");
                    }
                    else
                    {
                        AppendValue(stringBuilder, value, depth + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = type + "." + field.Name + "|异常:" + ex;
                UnityEngine.Debug.LogError(msg);
            }


            try
            {
                foreach (var itr in props)
                {
                    prop = itr.Value;
                    object value = prop.GetValue(item, null);

                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append(itr.Key);
                    stringBuilder.Append("\":");
                    if (value == null)
                    {
                        stringBuilder.Append("null");
                    }
                    else
                    {
                        AppendValue(stringBuilder, value, depth + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = type + "." + prop.Name + "|异常:" + ex;
                UnityEngine.Debug.LogError(msg);
            }

        }





        #region 自定义解析
        public delegate bool CustomWrite(StringBuilder sb, Type t, object obj);
        public static CustomWrite customWrite;
        #endregion
    }







    // Really simple JSON parser in ~300 lines
    // - Attempts to parse JSON files with minimal GC allocation
    // - Nice and simple "[1,2,3]".FromJson<List<int>>() API
    // - Classes and structs can be parsed too!
    //      class Foo { public int Value; }
    //      "{\"Value\":10}".FromJson<Foo>()
    // - Can parse JSON without type information into Dictionary<string,object> and List<object> e.g.
    //      "[1,2,3]".FromJson<object>().GetType() == typeof(List<object>)
    //      "{\"Value\":10}".FromJson<object>().GetType() == typeof(Dictionary<string,object>)
    // - No JIT Emit support to support AOT compilation on iOS
    // - Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
    // - Only public fields and property setters on classes/structs will be written to
    //
    // Limitations:
    // - No JIT Emit support to parse structures quickly
    // - Limited to parsing <2GB JSON files (due to int.MaxValue)
    // - Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
    public static class JSONParser
    {
        /*
         * 如果一个类型包含非静态字段（实例字段），则对于该字段，该类型的每个实例均有其自身的独立存储位置；
         * 在一个实例中设置字段并不影响其他实例中该字段的值。而相反，对于静态字段，无论有多少实例，该字段
         * 只位于一个存储位置（或者，更具体地说，在每个 AppDomain 中，只位于一个存储位置）。然而，如果将 
         * System.ThreadStaticAttribute 应用于静态字段，则该字段将变为线程静态字段，即，对于该字段，每个
         * 线程（而非实例）将保留其自身的存储位置。在一个线程上设置线程静态的值将不会影响其在其他线程上的值。
         */
        [ThreadStatic]
        static Stack<List<string>> splitArrayPool;
        [ThreadStatic]
        static StringBuilder stringBuilder;


        static JSONParser()
        {
            if (stringBuilder == null) stringBuilder = new StringBuilder();
            if (splitArrayPool == null) splitArrayPool = new Stack<List<string>>();
        }

        public static T FromJson<T>(string json)
        {
            return (T)FromJson(json, typeof(T));
        }

        #region 处理字符串
        public static object FromJson(string json, Type type)
        {
            //Remove all whitespace not within strings to make parsing simpler
            stringBuilder.Length = 0;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '"')
                {
                    i = AppendUntilStringEnd(true, i, json);//获取key或value
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                stringBuilder.Append(c);
            }

            //Parse the thing!
            return ParseValue(type, stringBuilder.ToString());
        }

        static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
        {
            stringBuilder.Append(json[startIdx]);
            for (int i = startIdx + 1; i < json.Length; i++)
            {
                if (json[i] == '\\')
                {
                    if (appendEscapeCharacter)
                        stringBuilder.Append(json[i]);
                    stringBuilder.Append(json[i + 1]);
                    i++;//Skip next character as it is escaped
                }
                else if (json[i] == '"')
                {
                    stringBuilder.Append(json[i]);
                    return i;
                }
                else
                    stringBuilder.Append(json[i]);
            }
            return json.Length - 1;
        }
        #endregion

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        public static List<string> Split(string json)
        {
            List<string> splitArray = splitArrayPool.Count > 0 ? splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            int parseDepth = 0;
            stringBuilder.Length = 0;
            for (int i = 1; i < json.Length - 1; i++)
            {
                switch (json[i])
                {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0)
                        {
                            splitArray.Add(stringBuilder.ToString());
                            stringBuilder.Length = 0;
                            continue;
                        }
                        break;
                }

                stringBuilder.Append(json[i]);
            }

            splitArray.Add(stringBuilder.ToString());

            return splitArray;
        }

        public static object ParseValue(Type type, string json)
        {
            if (customRead != null)
            {
                object o;
                if (customRead(json, type,  out o))
                {
                    return o;
                }
            }
            if (type == typeof(string))
            {
                if (json.Length <= 2)
                    return string.Empty;
                StringBuilder parseStringBuilder = new StringBuilder(json.Length);
                for (int i = 1; i < json.Length - 1; ++i)
                {
                    if (json[i] == '\\' && i + 1 < json.Length - 1)
                    {
                        int j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        if (j >= 0)
                        {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }
                        if (json[i + 1] == 'u' && i + 5 < json.Length - 1)
                        {
                            UInt32 c = 0;
                            if (UInt32.TryParse(json.Substring(i + 2, 4), System.Globalization.NumberStyles.AllowHexSpecifier, null, out c))
                            {
                                parseStringBuilder.Append((char)c);
                                i += 5;
                                continue;
                            }
                        }
                    }
                    parseStringBuilder.Append(json[i]);
                }
                return parseStringBuilder.ToString();
            }
            else if (type.IsPrimitive)
            {
                var result = Convert.ChangeType(json, type, System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            //if (type == typeof(decimal))
            //{
            //    decimal result;
            //    decimal.TryParse(json, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
            //    return result;
            //}
            else if (type.IsEnum)
            {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try
                {
                    return Enum.Parse(type, json, false);
                }
                catch
                {
                    return "unknown enum:" + json;
                }
            }
            else if (type.IsArray)
            {
                Type arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                Array newArray = Array.CreateInstance(arrayType, elems.Count);
                for (int i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i]), i);
                splitArrayPool.Push(elems);
                return newArray;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GetGenericArguments()[0];
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                var list = (IList)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count });
                for (int i = 0; i < elems.Count; i++)
                    list.Add(ParseValue(listType, elems[i]));
                splitArrayPool.Push(elems);
                return list;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType, valueType;
                {
                    Type[] args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                //if (keyType != typeof(string))
                //    return null;

                //Must be a valid dictionary element
                if (json[0] != '{' || json[json.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary = (IDictionary)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                for (int i = 0; i < elems.Count; i += 2)
                {
                    if (elems[i].Length <= 2)
                        continue;
                    string strkeyValue = elems[i].Substring(1, elems[i].Length - 2);
                    var keyValue = Convert.ChangeType(strkeyValue, keyType, System.Globalization.CultureInfo.InvariantCulture);
                    object val = ParseValue(valueType, elems[i + 1]);
                    dictionary.Add(keyValue, val);
                }
                return dictionary;
            }
            else if (type == typeof(object))
            {
                return ParseAnonymousValue(json);
            }
            else if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                return ParseObject(type, json);
            }
            //throw new System.NullReferenceException("未知类型:" + type);
            UnityEngine.Debug.LogError("未知类型:" + type);
            return null;
        }

        /// <summary>
        /// 不知道类型，转Dicationary<string,Object>
        /// </summary>
        static object ParseAnonymousValue(string json)
        {
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (int i = 0; i < elems.Count; i += 2)
                    dict.Add(elems[i].Substring(1, elems[i].Length - 2), ParseAnonymousValue(elems[i + 1]));
                return dict;
            }
            if (json[0] == '[' && json[json.Length - 1] == ']')
            {
                List<string> items = Split(json);
                var finalList = new List<object>(items.Count);
                for (int i = 0; i < items.Count; i++)
                    finalList.Add(ParseAnonymousValue(items[i]));
                return finalList;
            }
            if (json[0] == '"' && json[json.Length - 1] == '"')
            {
                string str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }
            if (char.IsDigit(json[0]) || json[0] == '-')
            {
                if (json.Contains("."))
                {
                    double result;
                    double.TryParse(json, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
                    return result;
                }
                else
                {
                    int result;
                    int.TryParse(json, out result);
                    return result;
                }
            }
            if (json == "true")
                return true;
            if (json == "false")
                return false;
            // handles json == "null" as well as invalid JSON
            return null;
        }

        /// <summary>
        /// 已知类型
        /// </summary>
        public static object ParseObject(Type type, string json)
        {
            object instance = CreateObject(type);
            if (instance == null)
            {
                UnityEngine.Debug.LogError("创建对象失败:" + type);
                return null;
            }
            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            List<string> elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            var fields = TinyJson.GetFields(type);
            var props = TinyJson.GetProperties(type);
            for (int i = 0; i < elems.Count; i += 2)
            {
                var e = elems[i];
                if (e.Length <= 2)
                    continue;
                string key = e.Substring(1, e.Length - 2);
                string value = elems[i + 1];
                object objValue = null;
                try
                {
                    FieldInfo fieldInfo;
                    PropertyInfo propertyInfo;
                    if (fields.TryGetValue(key, out fieldInfo))
                    {
                        objValue = ParseValue(fieldInfo.FieldType, value);
                        fieldInfo.SetValue(instance, objValue);
                    }
                    else if (props.TryGetValue(key, out propertyInfo))
                    {
                        objValue = ParseValue(propertyInfo.PropertyType, value);
                        propertyInfo.SetValue(instance, objValue, null);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError("反序列化出错:" + type + "[" + key + "]=" + value + "(" + objValue + ")" + "\n" + ex);
                    //throw ex;
                }
            }

            return instance;
        }
      

        #region 对象生成
        public delegate bool CustomCreateObject(Type type, out object obj);
        public static CustomCreateObject customCreateObject;
        public static object CreateObject(Type type)
        {
            object instance;
            if (customCreateObject != null && customCreateObject(type, out instance))
            {
                return instance;
            }
            else
            {
                instance = FormatterServices.GetUninitializedObject(type);
            }
            return instance;
        }
        #endregion

        #region 自定义解析
        public delegate bool CustomRead(string json, Type type,  out object obj);
        public static CustomRead customRead;
        #endregion
    }
}