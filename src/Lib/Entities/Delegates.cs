namespace Starscript;

public delegate void CompletionCallback(string completion, bool function);

public delegate Value StarscriptFunction(Starscript starscript, int argCount);