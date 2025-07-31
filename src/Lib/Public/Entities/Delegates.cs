namespace Starscript;

public delegate void CompletionCallback(string completion, bool function);

public delegate Value? ContextualStarscriptFunction(StarscriptFunctionContext ctx);

public delegate Value? StarscriptFunction(StarscriptHypervisor hypervisor, byte argCount);