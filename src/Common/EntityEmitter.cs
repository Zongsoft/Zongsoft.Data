/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015-2017 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.Data.
 *
 * Zongsoft.Data is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.Data is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.Data; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace Zongsoft.Data.Common
{
	internal static class EntityEmitter
	{
		private static readonly MethodInfo __IsDBNull__ = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

		public static Action<object, IDataRecord, int> GenerateFieldSetter(FieldInfo field)
		{
			var fieldType = field.FieldType;

			if(fieldType.IsEnum)
				fieldType = Enum.GetUnderlyingType(fieldType);

			var method = new DynamicMethod("Set" + field.Name, null, new Type[] { typeof(object), typeof(IDataRecord), typeof(int) }, field.DeclaringType, true);
			var generator = method.GetILGenerator();

			var ending = generator.DefineLabel();

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Callvirt, __IsDBNull__);
			generator.Emit(OpCodes.Brtrue, ending);

			generator.DeclareLocal(field.DeclaringType);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, field.DeclaringType);
			generator.Emit(OpCodes.Stloc_0);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, typeof(DataRecordExtension).GetMethod("GetValue", new Type[] { typeof(IDataRecord), typeof(int) }).MakeGenericMethod(fieldType));
			generator.Emit(OpCodes.Stfld, field);

			generator.MarkLabel(ending);
			generator.Emit(OpCodes.Ret);

			return (Action<object, IDataRecord, int>)method.CreateDelegate(typeof(Action<object, IDataRecord, int>));
		}

		public static Action<object, IDataRecord, int> GeneratePropertySetter(PropertyInfo property)
		{
			var propertyType = property.PropertyType;

			if(propertyType.IsEnum)
				propertyType = Enum.GetUnderlyingType(propertyType);

			var method = new DynamicMethod("Set" + property.Name, null, new Type[] { typeof(object), typeof(IDataRecord), typeof(int) }, property.DeclaringType, true);
			var generator = method.GetILGenerator();

			var ending = generator.DefineLabel();

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Callvirt, __IsDBNull__);
			generator.Emit(OpCodes.Brtrue, ending);

			generator.DeclareLocal(property.DeclaringType);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, property.DeclaringType);
			generator.Emit(OpCodes.Stloc_0);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, typeof(DataRecordExtension).GetMethod("GetValue", new Type[] { typeof(IDataRecord), typeof(int) }).MakeGenericMethod(propertyType));
			generator.Emit(OpCodes.Callvirt, property.SetMethod);

			generator.MarkLabel(ending);
			generator.Emit(OpCodes.Ret);

			return (Action<object, IDataRecord, int>)method.CreateDelegate(typeof(Action<object, IDataRecord, int>));
		}
	}
}
