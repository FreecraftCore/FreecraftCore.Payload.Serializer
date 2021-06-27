﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using Glader.Essentials;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FreecraftCore.Serializer
{
	public sealed class RawEnumPrimitiveSerializationGenerator : BaseInvokationExpressionEmitter
	{
		public string PrimitiveSerializationType { get; }

		public RawEnumPrimitiveSerializationGenerator([NotNull] ITypeSymbol actualType, [NotNull] ISymbol member, SerializationMode mode,
			[NotNull] string primitiveSerializationType) 
			: base(actualType, member, mode)
		{
			PrimitiveSerializationType = primitiveSerializationType ?? throw new ArgumentNullException(nameof(primitiveSerializationType));
		}

		public override InvocationExpressionSyntax Create()
		{
			return InvocationExpression
				(
					MemberAccessExpression
					(
						SyntaxKind.SimpleMemberAccessExpression,
						MemberAccessExpression
						(
							SyntaxKind.SimpleMemberAccessExpression,
							GenericName
								(
									Identifier("GenericPrimitiveEnumTypeSerializerStrategy")
								)
								.WithTypeArgumentList
								(
									TypeArgumentList
									(
										SeparatedList<TypeSyntax>
										(
											new SyntaxNodeOrToken[]
											{
												ComputerEnumTypeName(),
												Token
												(
													TriviaList(),
													SyntaxKind.CommaToken,
													TriviaList
													(
														Space
													)
												),
												IdentifierName(PrimitiveSerializationType)
											}
										)
									)
								),
							IdentifierName("Instance")
						),
						IdentifierName(Mode.ToString())
					)
				)
				.WithArgumentList
				(
					ArgumentList
					(
						SeparatedList<ArgumentSyntax>
						(
							Mode == SerializationMode.Write ? ComputeWriteMethodArgs() : ComputeReadMethodArgs()
						)
					)
				);
		}

		private IdentifierNameSyntax ComputerEnumTypeName()
		{
			//Some enums are nested. To support serializing them we need to consider 
			//that they may be nested and fully qualify them.
			if (ActualType.ContainingType == null)
				return IdentifierName(ActualType.Name);
			else
				return IdentifierName(ActualType.ToFullName()); //non fully qualified, because of global::
		}

		private SyntaxNodeOrToken[] ComputeReadMethodArgs()
		{
			return new SyntaxNodeOrToken[]
			{
				Argument
				(
					IdentifierName(CompilerConstants.BUFFER_NAME)
				),
				Token
				(
					TriviaList(),
					SyntaxKind.CommaToken,
					TriviaList
					(
						Space
					)
				),
				Argument
					(
						IdentifierName(CompilerConstants.OFFSET_NAME)
					)
					.WithRefKindKeyword
					(
						Token
						(
							TriviaList(),
							SyntaxKind.RefKeyword,
							TriviaList
							(
								Space
							)
						)
					)
			};
		}

		private SyntaxNodeOrToken[] ComputeWriteMethodArgs()
		{
			return new SyntaxNodeOrToken[]
				{
					Argument
					(
						//This is the critical part that accesses the member and passed it for serialization.
						IdentifierName($"{CompilerConstants.SERIALZIABLE_OBJECT_REFERENCE_NAME}.{Member.Name}")
					),
					Token
					(
						TriviaList(),
						SyntaxKind.CommaToken,
						TriviaList
						(
							Space
						)
					)
				}
				.Concat(ComputeReadMethodArgs())
				.ToArray();
		}
	}
}
