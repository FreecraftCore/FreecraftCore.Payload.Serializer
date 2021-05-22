﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using FreecraftCore.Serializer.Perf;

namespace FreecraftCore.Serializer.Performance.Tests
{
	class Program
	{
		private static byte[] realWorldBytes = new byte[]
		{
			1, //opcode

			0, //auth result (success)

			203, //20 byte M2 response
			164,
			231,
			13,
			97,
			45,
			211,
			167,
			253,
			241,
			138,
			250,
			202,
			47,
			151,
			53,
			6,
			192,
			213,
			118,

			0, //auth flags 4 byte uint flags enum
			0,
			128,
			0,

			0, //survey id 4 byte uint
			0,
			0,
			0,

			0, //unk3 ushort
			0
		};

		static void Main(string[] args)
		{
			realWorldBytes = realWorldBytes
				.Take(1)
				.Concat(new byte[5000])
				.Concat(realWorldBytes.Skip(1))
				.ToArray();

			Span<byte> buffer = new Span<byte>(new byte[10000]);
			int offset = 0;

			SerializerService serializer = new SerializerService();
			serializer.RegisterPolymorphicSerializer<AuthPacketBaseTest, AuthPacketBaseTest_Serializer>();

			//JIT
			AuthPacketBaseTest packet = serializer.Read<AuthPacketBaseTest>(realWorldBytes, 0);
			serializer.Write(packet, buffer, ref offset);

			BenchMarkMethod(serializer);
			Console.ReadKey();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void BenchMarkMethod(SerializerService serializer)
		{
			Stopwatch watch = new Stopwatch();
			Span<byte> buffer = new Span<byte>(new byte[10000]);
			int offset = 0;

			watch.Start();
			for (int i = 0; i < 5000000; i++)
			{
				AuthPacketBaseTest packet = serializer.Read<AuthPacketBaseTest>(realWorldBytes, 0);
				serializer.Write(packet, buffer, ref offset);
				offset = 0;
			}
			watch.Stop();
			Console.WriteLine($"MS: {watch.ElapsedMilliseconds}");
		}
	}
}
