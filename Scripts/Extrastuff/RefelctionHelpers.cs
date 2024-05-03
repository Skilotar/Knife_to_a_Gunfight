using System;
using System.Reflection;

namespace Knives
{
	// Token: 0x020001A0 RID: 416
	internal static class OMITBReflectionHelpers
	{
		// Token: 0x06000895 RID: 2197 RVA: 0x00069290 File Offset: 0x00067490
		public static T GetTypedValue<T>(this FieldInfo This, object instance)
		{
			return (T)((object)This.GetValue(instance));
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x000692B0 File Offset: 0x000674B0
		public static T ReflectGetField<T>(Type classType, string fieldName, object o = null)
		{
			FieldInfo field = classType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | ((o != null) ? BindingFlags.Instance : BindingFlags.Static));
			return (T)((object)field.GetValue(o));
		}

		public static void InvokeMethod(Type type, string methodName, object typeInstance = null, object[] methodParams = null)
		{
			BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | ((typeInstance == null) ? BindingFlags.Static : BindingFlags.Instance);
			type.GetMethod(methodName, bindingAttr).Invoke(typeInstance, methodParams);
		}
	}
}