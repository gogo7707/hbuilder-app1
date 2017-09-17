using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Randao.Core.Data
{
	internal class IDataReaderEntityBuilder<T>
	{
		private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
		private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

		private delegate T Load(IDataRecord dataRecord);

		private Load handler;
		private IDataReaderEntityBuilder() { }
		public T Build(IDataRecord dataRecord)
		{
			return handler(dataRecord);
		}

		public static IDataReaderEntityBuilder<T> CreateBuilder(IDataRecord dataRecord)
		{
			IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
			var myType = typeof(T);
			DynamicMethod method = new DynamicMethod("DynamicCreateEntity", myType,
					new Type[] { typeof(IDataRecord) }, myType, true);

			ILGenerator generator = method.GetILGenerator();
			LocalBuilder result = generator.DeclareLocal(myType);
			generator.Emit(OpCodes.Newobj, myType.GetConstructor(Type.EmptyTypes));

			generator.Emit(OpCodes.Stloc, result);
			for (int i = 0; i < dataRecord.FieldCount; i++)
			{
				PropertyInfo propertyInfo = DynamicPropertyCache.GetProperty<T>(dataRecord.GetName(i));
				if (propertyInfo != null)
				{
					Label endIfLabel = generator.DefineLabel();
					MethodInfo propertySetMethod = propertyInfo.GetSetMethod();
					if (propertyInfo != null && propertySetMethod != null)
					{
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldc_I4, i);
						generator.Emit(OpCodes.Callvirt, isDBNullMethod);
						generator.Emit(OpCodes.Brtrue, endIfLabel);
						generator.Emit(OpCodes.Ldloc, result);
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldc_I4, i);
						generator.Emit(OpCodes.Callvirt, getValueMethod);

						if (propertyInfo.PropertyType.IsValueType)
						{
							generator.Emit(OpCodes.Call, ConvertMethods[propertyInfo.PropertyType]);// 如果是值类型，拆箱 string = (string)object;
						}
						else
						{
							generator.Emit(OpCodes.Castclass, dataRecord.GetFieldType(i));
						}
						generator.Emit(OpCodes.Callvirt, propertySetMethod);
						generator.MarkLabel(endIfLabel);
					}
				}
			}
			generator.Emit(OpCodes.Ldloc, result);
			generator.Emit(OpCodes.Ret);
			dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
			return dynamicBuilder;
		}

		private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
        {   
           {typeof(Int32),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})}, 
           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})}, 
           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})}, 
           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})}, 
           {typeof(Decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})}, 
           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(float),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
           {typeof(String),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})},
		   {typeof(Guid),typeof(ConvertHelper).GetMethod("ConvertToGuid",new Type[]{typeof(object)})}
        };

		
		public static IDataReaderEntityBuilder<T> CreateBuilderNoCache(IDataRecord dataRecord)
		{
			IDataReaderEntityBuilder<T> dynamicBuilder = new IDataReaderEntityBuilder<T>();
			var entityType = typeof(T);
			DynamicMethod method = new DynamicMethod("DynamicCreateEntity", entityType,
					new Type[] { typeof(IDataRecord) }, entityType, true);

			ILGenerator generator = method.GetILGenerator();
			LocalBuilder result = generator.DeclareLocal(entityType);
			generator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));

			generator.Emit(OpCodes.Stloc, result);
			for (int i = 0; i < dataRecord.FieldCount; i++)
			{
				PropertyInfo propertyInfo = entityType.GetProperty(dataRecord.GetName(i), BindingFlags.Public | BindingFlags.IgnoreCase);
				if (propertyInfo != null)
				{
					Label endIfLabel = generator.DefineLabel();
					MethodInfo propertySetMethod = propertyInfo.GetSetMethod();
					if (propertyInfo != null && propertySetMethod != null)
					{
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldc_I4, i);
						generator.Emit(OpCodes.Callvirt, isDBNullMethod);
						generator.Emit(OpCodes.Brtrue, endIfLabel);
						generator.Emit(OpCodes.Ldloc, result);
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldc_I4, i);
						generator.Emit(OpCodes.Callvirt, getValueMethod);


						if (propertyInfo.PropertyType.IsValueType)
						{
							generator.Emit(OpCodes.Call, ConvertMethods[propertyInfo.PropertyType]);// 如果是值类型，拆箱 string = (string)object;
						}
						else
						{
							generator.Emit(OpCodes.Castclass, dataRecord.GetFieldType(i));
						}
						generator.Emit(OpCodes.Callvirt, propertySetMethod);
						generator.MarkLabel(endIfLabel);
					}
				}
			}
			generator.Emit(OpCodes.Ldloc, result);
			generator.Emit(OpCodes.Ret);
			dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
			return dynamicBuilder;
		}
	}

	internal class DynamicPropertyCache
	{
		private readonly static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> propertyCaches = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

		public static PropertyInfo GetProperty<T>(string propertyName)
		{
			var propertyDictionary = propertyCaches.GetOrAdd(typeof(T), type => type.GetProperties().ToDictionary(g => g.Name.ToLower()));
			PropertyInfo property;
			if (propertyDictionary.TryGetValue(propertyName.ToLower(), out property))
			{
				return property;
			}
			return null;
		}
	}

	internal class ConvertHelper
	{
		public static Guid ConvertToGuid(object obj)
		{
			if (null == obj)
			{
				return Guid.Empty;
			}
			return new Guid(obj.ToString());
		}
	}
}
