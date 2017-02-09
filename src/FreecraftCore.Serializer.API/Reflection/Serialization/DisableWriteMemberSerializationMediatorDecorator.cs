﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer.API.Reflection
{
	public class DisableWriteMemberSerializationMediatorDecorator<TContainingType> : IMemberSerializationMediator<TContainingType>
	{
		[NotNull]
		private IMemberSerializationMediator<TContainingType> decoratedMediator { get; }

		public DisableWriteMemberSerializationMediatorDecorator([NotNull] IMemberSerializationMediator<TContainingType> decoratedMediator)
		{
			if (decoratedMediator == null) throw new ArgumentNullException(nameof(decoratedMediator));

			this.decoratedMediator = decoratedMediator;
		}

		public void WriteMember(object obj, IWireStreamWriterStrategy dest)
		{
			//We ignore reading the member since we're not writing it
			return;
		}

		public void WriteMember(TContainingType obj, IWireStreamWriterStrategy dest)
		{
			//We ignore reading the member since we're not writing it
			return;
		}

		public void SetMember(TContainingType obj, IWireStreamReaderStrategy source)
		{
			decoratedMediator.SetMember(obj, source);
		}

		public void SetMember(object obj, IWireStreamReaderStrategy source)
		{
			decoratedMediator.SetMember(obj, source);
		}
	}
}
