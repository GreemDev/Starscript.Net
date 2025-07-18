namespace Starscript;

/// <summary>
///     A single token containing its <see cref="SemanticTokenType"/> and position in the source string.
/// </summary>
public partial record struct SemanticToken(SemanticTokenType Type, int Start, int End);

public enum SemanticTokenType
{
    Dot,
    Comma,
    Operator,
    String,
    Number,
    Keyword,
    Paren,
    Brace,
    Identifier,
    Map,
    Section,
    Error
}