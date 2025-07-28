using Starscript.Util;

namespace Starscript;

public class Constraint
{
    public Predicate<int> Test { get; }
    public Func<string> FriendlyError { get; }

    public Constraint(Predicate<int> test, Func<string> friendlyError)
    {
        Test = test;
        FriendlyError = friendlyError;
    }
    
    public string GetError(StarscriptFunctionContext context)
        => $"{context.FormattedName} requires {FriendlyError()}, got {context.ArgCount}.";

    public static readonly Constraint None = new(_ => true, () => string.Empty);

    public static Constraint ExactlyOneArgument { get; } = ExactCount(1);
    public static Constraint ExactlyTwoArguments { get; } = ExactCount(2);
    public static Constraint ExactlyThreeArguments { get; } = ExactCount(3);

    public static Constraint ExactCount(int count) => new(
        it => it == count,
        () => "argument".Pluralize(count, prefixQuantity: true)
    );
    
    public static Constraint AtLeast(int min) => new(
        it => it >= min,
        () => $"at least {"argument".Pluralize(min, prefixQuantity: true)}"
    );

    public static Constraint AtMost(int max) => new(
        it => it <= max,
        () => $"at most {"argument".Pluralize(max, prefixQuantity: true)}"
    );
    
    public static Constraint Within(int min, int max) => new(
        it => it >= min && it <= max,
        () => $"{min}..{max} {"argument".Pluralize(min + max)}"
    );
}