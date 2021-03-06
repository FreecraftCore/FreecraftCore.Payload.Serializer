﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FreecraftCore.Serializer
{
	public abstract class BaseSerializationStatementsBlockEmitter : IStatementsBlockEmittable
	{
		/// <summary>
		/// The actual type to emit serialization for.
		/// </summary>
		public ITypeSymbol ActualType { get; }

		/// <summary>
		/// The member to serialize.
		/// </summary>
		public ISymbol Member { get; }

		/// <summary>
		/// Indicates the serialization mode to emit for.
		/// </summary>
		protected SerializationMode Mode { get; }

		protected BaseSerializationStatementsBlockEmitter([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode)
		{
			if (!Enum.IsDefined(typeof(SerializationMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(SerializationMode));
			ActualType = actualType ?? throw new ArgumentNullException(nameof(actualType));
			Member = member ?? throw new ArgumentNullException(nameof(member));
			Mode = mode;
		}

		public abstract List<StatementSyntax> CreateStatements();
	}
}
