using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FreecraftCore.Serializer;
using FreecraftCore.Serializer.CustomTypes;
namespace FreecraftCore.Serializer.CustomTypes
{
    [AutoGeneratedWireMessageImplementationAttribute]
    public partial class TestCustomSerializerReferenceType : IWireMessage<TestCustomSerializerReferenceType>
    {
        public Type SerializableType => typeof(TestCustomSerializerReferenceType);
        public TestCustomSerializerReferenceType Read(Span<byte> buffer, ref int offset)
        {
            FreecraftCore.Serializer.CustomTypes.TestCustomSerializerReferenceTypeSerializer.Instance.InternalRead(this, buffer, ref offset);
            return this;
        }
        public void Write(TestCustomSerializerReferenceType value, Span<byte> buffer, ref int offset)
        {
            FreecraftCore.Serializer.CustomTypes.TestCustomSerializerReferenceTypeSerializer.Instance.InternalWrite(this, buffer, ref offset);
        }
    }
}