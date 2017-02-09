﻿using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public interface IMemberSerializationMediator<TContainingType> : IMemberSerializationMediator
	{
		void WriteMember([NotNull] TContainingType obj, [NotNull] IWireStreamWriterStrategy dest);

		void SetMember(TContainingType obj, [NotNull] IWireStreamReaderStrategy source);
	}

	public interface IMemberSerializationMediator
	{
		void WriteMember(object obj, [NotNull] IWireStreamWriterStrategy dest);

		void SetMember(object obj, [NotNull] IWireStreamReaderStrategy source);
	}
}