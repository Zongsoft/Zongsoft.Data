/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2015-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
	public static class EntityEmitter
	{
		#region 委托定义
		public delegate void Populator(ref object target, IDataRecord record, int ordinal);
		#endregion

		#region 私有变量
		private static readonly MethodInfo __IsDBNull__ = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
		private static readonly MethodInfo __GetValue__ = typeof(DataRecordExtension).GetMethod("GetValue", new Type[] { typeof(IDataRecord), typeof(int) });
		#endregion

		#region 公共方法
		public static Populator GenerateFieldSetter(FieldInfo field)
		{
			var fieldType = field.FieldType;

			if(fieldType.IsEnum)
				fieldType = Enum.GetUnderlyingType(fieldType);

			var method = new DynamicMethod(field.DeclaringType.FullName + "_Set" + field.Name, null, new Type[] { typeof(object).MakeByRefType(), typeof(IDataRecord), typeof(int) }, typeof(EntityEmitter), true);
			var generator = method.GetILGenerator();
			var ending = generator.DefineLabel();

			generator.DeclareLocal(field.DeclaringType);

			//if(record.IsDBNull(ordinal))
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Callvirt, __IsDBNull__);
			generator.Emit(OpCodes.Brtrue, ending);

			//local_0 = (T)target;
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldind_Ref);
			if(field.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Unbox_Any, field.DeclaringType);
			else
				generator.Emit(OpCodes.Castclass, field.DeclaringType);
			generator.Emit(OpCodes.Stloc_0);

			//local_0 = ...
			if(field.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Ldloca_S, 0);
			else
				generator.Emit(OpCodes.Ldloc_0);

			//local_0 = DataRecordExtension.GetValue<T>(record, ordinal)
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, __GetValue__.MakeGenericMethod(fieldType));
			generator.Emit(OpCodes.Stfld, field);

			//target = local_0
			if(field.DeclaringType.IsValueType)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Box, field.DeclaringType);
				generator.Emit(OpCodes.Stind_Ref);
			}

			generator.MarkLabel(ending);
			generator.Emit(OpCodes.Ret);

			return (Populator)method.CreateDelegate(typeof(Populator));
		}

		public static Populator GeneratePropertySetter(PropertyInfo property)
		{
			var propertyType = property.PropertyType;

			if(propertyType.IsEnum)
				propertyType = Enum.GetUnderlyingType(propertyType);

			var method = new DynamicMethod(property.DeclaringType.FullName + "_Set" + property.Name, null, new Type[] { typeof(object).MakeByRefType(), typeof(IDataRecord), typeof(int) }, typeof(EntityEmitter), true);
			var generator = method.GetILGenerator();
			var ending = generator.DefineLabel();

			generator.DeclareLocal(property.DeclaringType);

			//if(record.IsDBNull(ordinal))
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Callvirt, __IsDBNull__);
			generator.Emit(OpCodes.Brtrue, ending);

			//local_0 = (T)target;
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldind_Ref);
			if(property.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Unbox_Any, property.DeclaringType);
			else
				generator.Emit(OpCodes.Castclass, property.DeclaringType);
			generator.Emit(OpCodes.Stloc_0);

			//local_0 = ...
			if(property.DeclaringType.IsValueType)
				generator.Emit(OpCodes.Ldloca_S, 0);
			else
				generator.Emit(OpCodes.Ldloc_0);

			//local_0 = DataRecordExtension.GetValue<T>(record, ordinal)
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, __GetValue__.MakeGenericMethod(propertyType));
			generator.Emit(OpCodes.Callvirt, property.SetMethod);

			//target = local_0
			if(property.DeclaringType.IsValueType)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Box, property.DeclaringType);
				generator.Emit(OpCodes.Stind_Ref);
			}

			generator.MarkLabel(ending);
			generator.Emit(OpCodes.Ret);

			return (Populator)method.CreateDelegate(typeof(Populator));
		}
		#endregion
	}
}
