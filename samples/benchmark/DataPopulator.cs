using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Zongsoft.Data.Benchmark
{
	public class DataPopulator
	{
		#region 成员字段
		private Type _entityType;
		#endregion

		#region 私有变量
		private string[] _names;
		private Action<object, IDataRecord, int>[] _setters;
		#endregion

		#region 构造函数
		private DataPopulator(Type entityType)
		{
			_entityType = entityType;
		}
		#endregion

		#region 公共属性
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
		}
		#endregion

		#region 公共方法
		public System.Collections.IEnumerable Populate(IDataReader reader)
		{
			var setters = new Action<object, IDataRecord, int>[reader.FieldCount];

			for(int i = 0; i < reader.FieldCount; i++)
			{
				var index = Array.BinarySearch(_names, reader.GetName(i), StringComparer.Ordinal);

				if(index >= 0)
					setters[i] = _setters[index];
			}

			while(reader.Read())
			{
				var instance = Activator.CreateInstance(_entityType);

				for(var i = 0; i < reader.FieldCount; i++)
				{
					var setter = setters[i];

					if(setter != null)
						setter.Invoke(instance, reader, i);
				}

				yield return instance;
			};
		}
		#endregion

		#region 静态方法
		public static DataPopulator Build(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException(nameof(entityType));

			var properties = entityType
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanWrite)
				.OrderBy(p => p.Name)
				.ToArray();

			if(properties == null || properties.Length == 0)
				return null;

			var populator = new DataPopulator(entityType)
			{
				_names = new string[properties.Length],
				_setters = new Action<object, IDataRecord, int>[properties.Length],
			};

			for(int i = 0; i < properties.Length; i++)
			{
				populator._names[i] = properties[i].Name;
				populator._setters[i] = DataEmitter.GenerateSetter(entityType, properties[i]);
			}

			return populator;
		}
		#endregion
	}
}
