namespace TkachevProject4.Extensions;

using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
