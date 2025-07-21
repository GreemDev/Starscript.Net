namespace Starscript;

public interface IStarscriptObject<in T> where T : IStarscriptObject<T>
{
    public ValueMap ToStarscript();

    public static virtual implicit operator ValueMap(T value) => value.ToStarscript();
    public static virtual implicit operator Value(T value) => value.ToStarscript();
}