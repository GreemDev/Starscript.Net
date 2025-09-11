namespace Starscript.Internal;

public enum Token : byte
{
    String, Identifier, Number,

    Null,
    True, False,
    And, Or,

    EqualEqual, BangEqual,
    Greater, GreaterEqual,
    Less, LessEqual,

    Plus, Minus,
    Star, Slash, Percentage, UpArrow, DoubleUpArrow,
    Bang,

    Dot, Comma,
    QuestionMark, Colon,
    LeftParen, RightParen,
    LeftBrace, RightBrace,

    Section,
    Ampersand,
    Tilde,
    VBar,
    DoubleGreater, TripleGreater,
    DoubleLess,

    Error, EOF
}