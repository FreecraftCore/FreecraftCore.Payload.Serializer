﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using Fasterflect;
using JetBrains.Annotations;

#if NET35
using System.Reflection.Emit;
#endif

namespace FreecraftCore.Serializer
{
	//TODO: Redo doc. It's no longer valid at all.
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public  abstract class MemberSerializationMediator<TContainingType, TMemberType> : MemberSerializationMediator, IMemberSerializationMediator<TContainingType>
	{
		/// <summary>
		/// Delegate that can grab the <see cref="MemberInformation"/> member value.
		/// </summary>
		[NotNull]
		protected Func<TContainingType, object> MemberGetter { get; }

		[NotNull]
		protected Action<TContainingType, TMemberType> MemberAccessor { get; }

		protected MemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
			: base(memberInfo, serializer)
		{
			if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));

			//Due to perf problems fasterflect setting wasn't fast enough.
			//Introducing a compiled lambda to delegate for get/set should provide the much needed preformance.

			ParameterExpression instanceOfTypeToReadMemberOn = Expression.Parameter(memberInfo.DeclaringType, "instance");
			MemberExpression member = Expression.PropertyOrField(instanceOfTypeToReadMemberOn, memberInfo.Name);
			UnaryExpression castExpression = Expression.TypeAs(member, typeof(object)); //use object to box

			//Build the getter lambda
			MemberGetter = Expression.Lambda(castExpression, instanceOfTypeToReadMemberOn).Compile()
				as Func<TContainingType, object>;

			if(MemberGetter == null)
				throw new InvalidOperationException($"Failed to build {nameof(MemberSerializationMediator)} for Member: {memberInfo.Name} for Type: {typeof(TContainingType).FullName}.");;

			//The below may seem ridiculous, when we could use reflection or even fasterflect, but it makes the different
			//of almost an order of magnitude.
			//Based on: http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree
#if !NET35
			//Now we need to do property setting
			ParameterExpression targetExp = Expression.Parameter(memberInfo.DeclaringType, "target");
			ParameterExpression valueExp = Expression.Parameter(typeof(TMemberType), "value");

			// Expression.Property can be used here as well
			MemberExpression memberExp = Expression.PropertyOrField(targetExp, memberInfo.Name);
			BinaryExpression assignExp = Expression.Assign(memberExp, valueExp);

			MemberAccessor = Expression.Lambda<Action<TContainingType, TMemberType>>(assignExp, targetExp, valueExp)
				.Compile();
#endif
			//TODO: Handle for net35. Profile fasterflect vs reflection emit
		}

		public abstract void WriteMember(TContainingType obj, IWireMemberWriterStrategy dest);

		public abstract void SetMember(TContainingType obj, IWireMemberReaderStrategy source);
	}

	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public abstract class MemberSerializationMediator : IMemberSerializationMediator
	{
		/// <summary>
		/// Cached <see cref="MemberInfo"/>.
		/// </summary>
		[NotNull]
		protected MemberInfo MemberInformation { get; }

		/// <summary>
		/// Serializer to serialize for the <see cref="MemberInformation"/>.
		/// </summary>
		[NotNull]
		protected ITypeSerializerStrategy TypeSerializer { get; }

		protected MemberSerializationMediator([NotNull] MemberInfo memberInfo, [NotNull] ITypeSerializerStrategy serializer)
		{
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo), $"Provided argument {nameof(memberInfo)} is null.");

			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {nameof(serializer)} is null.");

			MemberInformation = memberInfo;
			TypeSerializer = serializer;
		}

		public abstract void SetMember(object obj, [NotNull] IWireMemberReaderStrategy source);

		public abstract void WriteMember(object obj, [NotNull] IWireMemberWriterStrategy dest);
	}
}
