using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CL
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get the Description for enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nameInstead">return it's name if there isn't Descript when set true</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value, Boolean nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }

        public static int EnumToInt(Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static T IntToEnum<T>(int value)
        {
            Type enumType = typeof(T);
            if (!Enum.IsDefined(enumType, value))
            {
                throw new ArgumentException("The value within corresponding enumeration is undefined!");
            }

            return (T)Enum.ToObject(enumType, value);
        }

        public static string EnumToString(Enum value)
        {
            return value.ToString();
        }

        public static T StringToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Convert enum to Dictionary<Int32, String>
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="getText">use EnumToString(enumValue) if null</param>
        /// <returns></returns>
        public static Dictionary<Int32, String> EnumToDictionary(Type enumType, Func<Enum, String> getText = null)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("The parameter must be enumerated types!", "enumType");
            }
            Dictionary<Int32, String> enumDic = new Dictionary<int, string>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum enumValue in enumValues)
            {
                Int32 key = Convert.ToInt32(enumValue);
                String value;
                if (getText == null)
                {
                    value = EnumToString(enumValue);
                }
                else
                {
                    value = getText(enumValue);
                }
                enumDic.Add(key, value);
            }
            return enumDic;
        }
    }
}
