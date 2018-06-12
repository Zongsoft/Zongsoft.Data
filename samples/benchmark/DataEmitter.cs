using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Zongsoft.Data.Benchmark
{
	public static class DataEmitter
	{
		private static readonly MethodInfo __IsDBNull__ = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

		public static Action<object, IDataRecord, int> GenerateSetter(Type entityType, PropertyInfo property)
		{
			var propertyType = property.PropertyType;

			if(propertyType.IsEnum)
				propertyType = Enum.GetUnderlyingType(propertyType);

			var method = new DynamicMethod("Set" + property.Name, null, new Type[] { typeof(object), typeof(IDataRecord), typeof(int) }, entityType, true);
			var generator = method.GetILGenerator();

			var ending = generator.DefineLabel();

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Callvirt, __IsDBNull__);
			generator.Emit(OpCodes.Brtrue, ending);

			generator.DeclareLocal(entityType);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, entityType);
			generator.Emit(OpCodes.Stloc_0);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, typeof(Zongsoft.Data.DataExtensions).GetMethod("GetValue", new Type[] { typeof(IDataRecord), typeof(int) }).MakeGenericMethod(propertyType));
			generator.Emit(OpCodes.Callvirt, property.SetMethod);

			generator.MarkLabel(ending);
			generator.Emit(OpCodes.Ret);

			return (Action<object, IDataRecord, int>)method.CreateDelegate(typeof(Action<object, IDataRecord, int>));
		}
	}
}
