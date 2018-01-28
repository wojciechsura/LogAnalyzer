using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Common.Extensions
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum value)
            where T : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (T[])fi.GetCustomAttributes(typeof(T), false);
            return attributes?.FirstOrDefault();            
        }
    }
}
