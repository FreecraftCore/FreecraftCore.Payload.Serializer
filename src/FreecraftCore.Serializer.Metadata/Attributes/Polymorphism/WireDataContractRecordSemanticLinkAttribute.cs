﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Similar to <see cref="WireDataContractBaseTypeAttribute"/> but will use semantic analysis to determine
	/// the key value. The default is to use the first constructor argument to the base record type
	/// as the key value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class WireDataContractRecordSemanticLinkAttribute : Attribute
	{

	}
}
