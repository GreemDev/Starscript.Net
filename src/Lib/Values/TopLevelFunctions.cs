﻿namespace Starscript;

/// <summary>
/// The intention of this class is to be statically imported.
/// </summary>
public static class TopLevelFunctions
{
    public static Value Boolean(bool value) => value;
    public static Value Number(double value) => value;
    public static Value String(string value) => value;
    public static Value Function(StarscriptFunction value) => value;
    public static Value Map(ValueMap value) => value;

    public static Value Object(object obj) => new Value.Object(obj);
}