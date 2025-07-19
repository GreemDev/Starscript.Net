using Starscript.Util;

namespace Starscript;

public abstract class Constraint
{
    public Predicate<int> Test { get; }
    public Func<string> FriendlyError { get; }

    internal Constraint(Predicate<int> test, Func<string> friendlyError)
    {
        Test = test;
        FriendlyError = friendlyError;
    }

    public static readonly Constraint None = new NoConstraint();
    public static Constraint ExactCount(int count) => new ExactCountConstraint(count);
    public static Constraint AtLeast(int min) => new AtLeastConstraint(min);
    public static Constraint AtMost(int max) => new AtMostConstraint(max);
    public static Constraint Within(int min, int max) => new WithinConstraint(min, max);

    public string GetError(StarscriptFunctionContext context)
        => $"{context.FormattedName} requires {FriendlyError()}, got {context.ArgCount}.";

    private sealed class NoConstraint() : Constraint(_ => true, () => string.Empty);

    private sealed class ExactCountConstraint(int count) : Constraint(
        it => it == count,
        () => "argument".Pluralize(count, prefixQuantity: true)
    );

    private sealed class AtLeastConstraint(int min) : Constraint(
        it => it >= min,
        () => $"at least {"argument".Pluralize(min, prefixQuantity: true)}"
    );

    private sealed class AtMostConstraint(int min) : Constraint(
        it => it <= min,
        () => $"at most {"argument".Pluralize(min, prefixQuantity: true)}"
    );

    private sealed class WithinConstraint(int min, int max) : Constraint(
        it => it >= min && it <= max,
        () => $"{min}..{max} {"argument".Pluralize(min + max)}"
    );
}