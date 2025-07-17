using System.Globalization;
using System.Runtime.InteropServices;

namespace Starscript;

/// <summary>
///     A class that holds any Starscript value.
/// </summary>
public class Value
{
    public const string ToStringIdentifier = "_toString";
    
    private static readonly Value True = new Boolean(true);
    private static readonly Value False = new Boolean(false);
    
    public static Value Null { get; } = new(ValueType.Null);
    
    public ValueType Type { get; }

    public Value(ValueType type)
    {
        Type = type;
    }

    public static implicit operator Value(bool? value) => value is null ? Null : value.Value ? True : False;
    public static implicit operator Value(double? value) => value is null ? Null : new Number(value.Value);
    public static implicit operator Value(string? value) =>value is null ? Null : new String(value);
    public static implicit operator Value(StarscriptFunction value) => new Function(value);
    public static implicit operator Value(ValueMap? value) => value is null ? Null : new Map(value);

    public bool IsNull => Type is ValueType.Null;
    public bool IsBool => Type is ValueType.Boolean;
    public bool IsNumber => Type is ValueType.Number;
    public bool IsString => Type is ValueType.String;
    public bool IsFunction => Type is ValueType.Function;
    public bool IsMap => Type is ValueType.Map;
    public bool IsObject => Type is ValueType.Object;

    public bool GetBool() => ((Boolean)this).Value;
    public double GetNumber() => ((Number)this).Value;
    public string GetString() => ((String)this).Value;
    public StarscriptFunction GetFunction() => ((Function)this).Value;
    public ValueMap GetMap() => ((Map)this).Value;
    public object GetObject() => ((Object)this).Value;

    public bool IsTruthy => Type switch
    {
        ValueType.Number or ValueType.String or ValueType.Function or ValueType.Map or ValueType.Object => true,
        ValueType.Boolean => GetBool(),
        _ => false
    };

    public static bool operator ==(Value left, Value right) => left.Equals(right);
    public static bool operator !=(Value left, Value right) => !left.Equals(right);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Value val) return false;

        if (Type != val.Type) return false;

        return Type switch
        {
            ValueType.Null => true,
            ValueType.Boolean => GetBool() == val.GetBool(),
            ValueType.Number => GetNumber() == val.GetNumber(),
            ValueType.String => GetString() == val.GetString(),
            ValueType.Function => GetFunction() == val.GetFunction(),
            ValueType.Map => GetMap() == val.GetMap(),
            ValueType.Object => GetObject() == val.GetObject(),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        var result = 31 * base.GetHashCode();
        
        switch (Type)
        {
            case ValueType.Boolean: result += GetBool().GetHashCode(); break;
            case ValueType.Number: result += GetNumber().GetHashCode(); break;
            case ValueType.String: result += GetString().GetHashCode(); break;
            case ValueType.Function: result += GetFunction().GetHashCode(); break;
            case ValueType.Map: result += GetMap().GetHashCode(); break;
            case ValueType.Object: result += GetObject().GetHashCode(); break;
            case ValueType.Null:
            default: break;
        }

        return result;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case ValueType.Null: return "null";
            case ValueType.Boolean: return GetBool() ? "true" : "false";
            case ValueType.Number:
                var val = GetNumber();
                return val % 1 == 0
                    ? ((int)val).ToString()
                    : val.ToString(FullDoubleFormatInfo);
            case ValueType.String: return GetString();
            case ValueType.Function:
                var ptr = Marshal.GetFunctionPointerForDelegate(GetFunction());
                return $"<function@0x{ptr:x8}>";
            case ValueType.Map:
                var customToString = GetMap().GetRaw(ToStringIdentifier);
                return customToString?.Invoke()?.ToString() ?? "<map>";
            case ValueType.Object: return GetObject().ToString() ?? string.Empty;
            default: return string.Empty;
        }
    }

    private static readonly NumberFormatInfo FullDoubleFormatInfo = new() { NumberDecimalDigits = 99 };

    #region Internal types
    internal class Boolean(bool value) : Value(ValueType.Boolean)
    {
        public bool Value => value;
    }
    
    internal class Number(double value) : Value(ValueType.Number)
    {
        public double Value => value;
    }
    
    internal class String(string value) : Value(ValueType.String)
    {
        public string Value => value;
    }
    
    internal class Function(StarscriptFunction value) : Value(ValueType.Function)
    {
        public StarscriptFunction Value => value;
    }
    
    internal class Map(ValueMap value) : Value(ValueType.Map)
    {
        public ValueMap Value => value;
    }
    
    internal class Object(object value) : Value(ValueType.Object)
    {
        public object Value => value;
    }
    #endregion
}
