﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	[Generator]
	public sealed class WireTypeSerializationSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				INamedTypeSymbol[] symbols = context
					.Compilation
					.Assembly
					.GlobalNamespace
					.GetAllTypes()
					.Where(t => t.ContainingAssembly != null && t.ContainingAssembly.Equals(context.Compilation.Assembly, SymbolEqualityComparer.Default))
					.ToArray();

				ISerializationSourceOutputStrategy outputStrategy = new ExternalContentCollectorSerializationSourceOutputStrategy();
				SerializerSourceEmitter emitter = new SerializerSourceEmitter(symbols, outputStrategy, context.Compilation);
				emitter.Generate();

				foreach (var entry in outputStrategy.Content)
					context.AddSource(entry.Key, entry.Value);

				//TODO: This is a hack but for some reason sometimes we end up writing serializers out to strange places.
				if (Directory.GetCurrentDirectory().Contains(context.Compilation.AssemblyName))
				{
					//Write contents to Debug directory
					string rootPath = Path.Combine("SerializerDebug");

					if(!Directory.Exists(rootPath))
						Directory.CreateDirectory(rootPath);

					//Now we log out the serialization debug filers
					foreach(var entry in outputStrategy.Content)
						File.WriteAllText(Path.Combine(rootPath, $"{entry.Key}.cs"), entry.Value);
				}
			}
			catch (System.Reflection.ReflectionTypeLoadException e)
			{
				File.WriteAllText("Error.txt", $"{e}\n\nLoader: {e.LoaderExceptions.Select(ex => ex.ToString()).Aggregate((s1, s2) => $"{s1}\n{s2}")}");
			}
			catch (Exception e)
			{
				File.WriteAllText("Error.txt", e.ToString());

				try
				{
					context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
				}
				catch (Exception)
				{
					throw e;
				}
			}
		}

		private static string BuildStackTrace(Exception e)
		{
			return e.StackTrace
				.Replace('{', ' ')
				.Replace('}', ' ')
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Skip(5)
				.Aggregate((s1, s2) => $"{s1} {s2}");
		}
	}
}
