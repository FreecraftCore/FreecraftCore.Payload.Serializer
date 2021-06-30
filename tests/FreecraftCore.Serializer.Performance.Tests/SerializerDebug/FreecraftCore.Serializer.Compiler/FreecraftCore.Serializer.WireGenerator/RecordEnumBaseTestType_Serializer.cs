﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore;

namespace FreecraftCore
{
	[AutoGeneratedWireMessageImplementationAttribute]
	public partial record RecordEnumBaseTestType
	{
		public override Type SerializableType => typeof(RecordEnumBaseTestType);
		public override BaseRecordEnumBaseTestType Read(Span<byte> buffer, ref int offset)
		{
			throw new NotSupportedException("Record types do not support WireMessage Read.");
		}

		public override void Write(BaseRecordEnumBaseTestType value, Span<byte> buffer, ref int offset)
		{
			RecordEnumBaseTestType_Serializer.Instance.Write(this, buffer, ref offset);
		}
	}
}

namespace FreecraftCore.Serializer
{
	[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
	//THIS CODE IS FOR AUTO-GENERATED SERIALIZERS! DO NOT MODIFY UNLESS YOU KNOW WELL!
	/// <summary>
	/// FreecraftCore.Serializer's AUTO-GENERATED (do not edit) serialization
	/// code for the Type: <see cref = "RecordEnumBaseTestType"/>
	/// </summary>
	public sealed partial class RecordEnumBaseTestType_Serializer : BaseAutoGeneratedRecordSerializerStrategy<RecordEnumBaseTestType_Serializer, RecordEnumBaseTestType>
	{
		/// <summary>
		/// Auto-generated deserialization/read method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override RecordEnumBaseTestType Read(Span<byte> buffer, ref int offset)
		{
			var local_OpCode = GenericPrimitiveEnumTypeSerializerStrategy<TestEnumOpcode, Int32>.Instance.Read(buffer, ref offset);
			var local_BaseValue = GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset);
			FreecraftCore.RecordEnumBaseTestType value = new FreecraftCore.RecordEnumBaseTestType(GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Read(buffer, ref offset), DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Read(buffer, ref offset), SendSizePrimitiveArrayTypeSerializerStrategy<int, Int32>.Instance.Read(buffer, ref offset))
			{OpCode = local_OpCode, BaseValue = local_BaseValue};
			return value;
		}

		/// <summary>
		/// Auto-generated serialization/write method.
		/// Partial method implemented from shared partial definition.
		/// </summary>
		/// <param name = "value">See external doc.</param>
		/// <param name = "buffer">See external doc.</param>
		/// <param name = "offset">See external doc.</param>
		public override void Write(RecordEnumBaseTestType value, Span<byte> buffer, ref int offset)
		{
			//Type: BaseRecordEnumBaseTestType Field: 1 Name: OpCode Type: TestEnumOpcode
			;
			GenericPrimitiveEnumTypeSerializerStrategy<TestEnumOpcode, Int32>.Instance.Write(value.OpCode, buffer, ref offset);
			//Type: BaseRecordEnumBaseTestType Field: 2 Name: BaseValue Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.BaseValue, buffer, ref offset);
			//Type: RecordEnumBaseTestType Field: 1 Name: TestField Type: Int32
			;
			GenericTypePrimitiveSerializerStrategy<Int32>.Instance.Write(value.TestField, buffer, ref offset);
			//Type: RecordEnumBaseTestType Field: 2 Name: TestField2 Type: String
			;
			DontTerminateLengthPrefixedStringTypeSerializerStrategy<ASCIIStringTypeSerializerStrategy, Int32>.Instance.Write(value.TestField2, buffer, ref offset);
			//Type: RecordEnumBaseTestType Field: 3 Name: TestArray Type: Int32[]
			;
			SendSizePrimitiveArrayTypeSerializerStrategy<int, Int32>.Instance.Write(value.TestArray, buffer, ref offset);
		}
	}
}