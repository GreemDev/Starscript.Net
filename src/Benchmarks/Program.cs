// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Starscript.Net.Benchmarks;

BenchmarkRunner.Run<StarscriptVsStringFormat>();