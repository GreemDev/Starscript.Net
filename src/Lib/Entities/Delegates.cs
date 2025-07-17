namespace Starscript;

public delegate void CompletionCallback(string completion, bool function);

public delegate Value? ContextualStarscriptFunction(StarscriptFunctionContext ctx);

public delegate Value? StarscriptFunction(Starscript starscript, byte argCount);