﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace FreecraftCore.Serializer
{
	public interface INameBuildable
	{
		string BuildName();
	}

	internal sealed class GeneratedSerializerNameStringBuilder : INameBuildable
	{
		//Linked to autogenerated template type template. (Commented out usually)
		public const string SERIALIZER_NAME = "AutoGeneratedTemplateSerializerStrategy";

		public static INameBuildable Create([NotNull] Type serializableType)
		{
			if (serializableType == null) throw new ArgumentNullException(nameof(serializableType));

			return (INameBuildable) Activator.CreateInstance(typeof(GeneratedSerializerNameStringBuilder<>).MakeGenericType(serializableType));
		}

		public static INameBuildable Create([NotNull] ITypeSymbol serializableType)
		{
			if(serializableType == null) throw new ArgumentNullException(nameof(serializableType));

			return new GeneratedSerializerNameStringBuilder(serializableType);
		}

		public ITypeSymbol Symbol { get; }

		public GeneratedSerializerNameStringBuilder([NotNull] ITypeSymbol symbol)
		{
			Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
		}

		public override string ToString()
		{
			try
			{
				//TODO: This causes weirdly named WireMessageType files to be generated for Types with this attribute
				//Special case of overriden Type serializer
				if(Symbol.HasAttributeExact<CustomTypeSerializerAttribute>())
				{
					//Get the type specified on the type itself\
					//and return full name due to namespacing issues with custom serializers
					return Symbol.GetAttributeExact<CustomTypeSerializerAttribute>()
						.ConstructorArguments
						.Select(a => a.Value)
						.Cast<ITypeSymbol>()
						.First()
						.ToFullName();
				}
				else
					return $"{ComputeName(Symbol)}_{SERIALIZER_NAME}";
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to compute SerializerName for Type: {Symbol.Name}", e);
			}
		}

		public string BuildName()
		{
			return ToString();
		}

		private static string ComputeName(ITypeSymbol type)
		{
			if(type is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)
			{
				//Soooo, when it comes to TypeSymbols it's not like Reflection where
				//the name will contain a backtick `. It's just the generic type name
				//and we'll need to fill in the type arg values still.
				string baseName = type.Name;
				StringBuilder builder = new StringBuilder(baseName);
				builder.Append('_');

				//TODO: For long names this could exceed the name
				//The common language runtime imposes a limitation on the full class name length, specifying that it should not exceed 1,023 bytes in UTF-8 encoding.
				foreach(ITypeSymbol genericTypeArg in namedSymbol.TypeArguments)
				{
					if(genericTypeArg is INamedTypeSymbol casted && casted.IsGenericType && !type.Equals(casted, SymbolEqualityComparer.Default)) //Avoid self referencing generic types??
					{
						//There is a case when the generic type arg ITSELF may be generic
						//therefore we must recursively compute the type name
						builder.Append(ComputeName(casted));
					}
					else
						builder.Append($"{genericTypeArg.Name}");
				}

				if(builder.Length > 1000)
					throw new InvalidOperationException($"Generated serializer name far too large. Requested: {builder.Length} Max: {1000} Name: {builder.ToString()}");

				return builder.ToString();

			}
			else
				return type.Name;
		}
	}

	internal sealed class GeneratedSerializerNameStringBuilder<TSerializableType> : INameBuildable
	{
		public override string ToString()
		{
			return $"{ComputeName(typeof(TSerializableType))}_{GeneratedSerializerNameStringBuilder.SERIALIZER_NAME}";
		}

		public string BuildName()
		{
			return ToString();
		}

		private static string ComputeName(Type type)
		{
			if (type.IsGenericType)
			{
				string baseName = type.Name.Remove(type.Name.IndexOf('`'));
				StringBuilder builder = new StringBuilder(baseName);
				builder.Append('_');

				//TODO: For long names this could exceed the name
				//The common language runtime imposes a limitation on the full class name length, specifying that it should not exceed 1,023 bytes in UTF-8 encoding.
				foreach (var genericTypeArg in type.GetGenericArguments())
				{
					if(genericTypeArg.IsGenericType && genericTypeArg != type) //Avoid self referencing generic types??
					{
						//There is a case when the generic type arg ITSELF may be generic
						//therefore we must recursively compute the type name
						builder.Append(ComputeName(genericTypeArg));
					}
					else
						builder.Append($"{genericTypeArg.Name}");
				}

				if (builder.Length > 1000)
					throw new InvalidOperationException($"Generated serializer name far too large. Requested: {builder.Length} Max: {1000} Name: {builder.ToString()}");

				return builder.ToString();

			}
			else
				return typeof(TSerializableType).Name;
		}
	}
}
