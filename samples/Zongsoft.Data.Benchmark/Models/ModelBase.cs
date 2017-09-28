using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zongsoft.Data.Benchmark.Models
{
	public abstract class ModelBase : System.ComponentModel.INotifyPropertyChanged
	{
		#region 静态常量
		private static readonly string[] EmptyArray = new string[0];
		#endregion

		#region 事件声明
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 成员字段
		private int[] __masks__;
		private Dictionary<string, object> _changedProperties = new Dictionary<string, object>();
		#endregion

		#region 构造函数
		protected ModelBase()
		{
		}
		#endregion

		#region 保护方法
		protected void SetPropertyValue<T>(string propertyName, ref T target, T value)
		{
			if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			//更新目标值
			target = value;

			//激发“PropertyChanged”事件
			this.RaisePropertyChanged(propertyName, value);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, ref T target, T value)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			//获取属性表达式指定的属性信息
			//var property = this.GetPropertyInfo(propertyExpression);

			//更新目标的值
			target = value;

			//激发“PropertyChanged”事件
			//this.RaisePropertyChanged(property.Name, value);
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 返回当前对象是否有属性值被改变过。
		/// </summary>
		public bool HasChanges()
		{
			return __masks__ != null && __masks__.Length > 0;
		}

		public IEnumerable<string> GetChangedKeys()
		{
			return BloomFilter.Get(this.GetType(), __masks__);
		}
		#endregion

		#region 激发事件
		protected void RaisePropertyChanged<T>(string propertyName, T value)
		{
			//BloomFilter.Set(this.GetType(), propertyName, ref __masks__);

			_changedProperties[propertyName] = value;
			//_changedProperties[propertyName] = Delegate.CreateDelegate(typeof(Func<T>), info);

			//激发“PropertyChanged”事件
			//this.OnPropertyChanged(propertyName);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = this.PropertyChanged;

			if(handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		#region 私有方法
		private PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> propertyExpression)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			var memberExpression = propertyExpression.Body as MemberExpression;

			if(memberExpression == null)
				throw new ArgumentException("The expression is not a property expression.", nameof(propertyExpression));

			if(memberExpression.Member.MemberType != MemberTypes.Property)
				throw new InvalidOperationException($"The '{memberExpression.Member.Name}' member is not property.");

			return (PropertyInfo)memberExpression.Member;
		}
		#endregion

		#region 嵌套子类
		private static class BloomFilter
		{
			#region 私有变量
			private readonly static int[] MASKS = new int[]
			{
				0x1, 0x2, 0x4, 0x8, 0x10, 0x20, 0x40, 0x80,
				0x1_00, 0x2_00, 0x4_00, 0x8_00, 0x10_00, 0x20_00, 0x40_00, 0x80_00,
				0x1_00_00, 0x2_00_00,0x4_00_00,0x8_00_00,0x10_00_00, 0x20_00_00,0x40_00_00,0x80_00_00,
				0x1_00_00_00,0x2_00_00_00,0x4_00_00_00,0x8_00_00_00,0x10_00_00_00,0x20_00_00_00,0x40_00_00_00,int.MinValue,
			};

			private readonly static IDictionary<Type, string[]> _filters = new Dictionary<Type, string[]>();
			#endregion

			#region 公共方法
			public static IEnumerable<string> Get(Type type, int[] masks)
			{
				if(_filters.TryGetValue(type, out var keys))
				{
					for(int i = 0; i < masks.Length; i++)
					{
						for(int j = 0; j < MASKS.Length; j++)
						{
							if((masks[i] & MASKS[j]) == MASKS[j])
								yield return keys[i * 32 + j];
						}
					}
				}
			}

			public static bool Set(Type type, string name, ref int[] masks)
			{
				//if(type == null || type == typeof(object) || type.IsValueType || type.IsAbstract || type.IsInterface)
				//	return false;

				if(!_filters.TryGetValue(type, out var keys))
				{
					lock(_filters)
					{
						keys = Generate(type);
						_filters[type] = keys;
					}
				}

				if(keys != null && keys.Length > 0)
				{
					if(masks == null)
					{
						lock(MASKS)
						{
							if(masks == null)
								masks = new int[(int)Math.Ceiling(keys.Length / 32.0)];
						}
					}

					var index = 0; //Array.BinarySearch(keys, name, StringComparer.Ordinal);

					if(index >= 0)
					{
						SetMask(ref masks, index);
						return true;
					}
				}

				return false;
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
			private static string[] Generate(Type type)
			{
				//var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).OrderBy(p => p.Name);
				//return properties.Select(p => p.Name).ToArray();

				var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

				if(properties == null || properties.Length == 0)
					return null;

				var keys = new string[properties.Length];

				for(int i = 0; i < properties.Length; i++)
				{
					keys[i] = properties[i].Name;
				}

				Array.Sort(keys);

				return keys;
			}

			private static void SetMask(ref int[] masks, int position)
			{
				var index = position / 32;
				var offset = position % 32;

				while(true)
				{
					var oldMask = masks[index];
					var newMask = oldMask | MASKS[offset];

					var result = System.Threading.Interlocked.CompareExchange(ref masks[index], newMask, oldMask);

					if(result == oldMask)
						break;
				}
			}
			#endregion
		}
		#endregion
	}
}
