using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;

namespace DeepCopier
{
    internal class DeepCopier<T>
    {
        public DeepCopier()
        {
            this._sourceExpr = Expression.Parameter(typeof(T), "source");
            this._destExpr = Expression.Parameter(typeof(T), "dest");
        }

        public Expression<Action<T, T>> GetDeepCopyFieldLambda(string fieldName)
        {
            return this.GetDeepCopyFieldLambda(typeof(T).GetField(fieldName));
        }

        public Expression<Action<T, T>> GetDeepCopyFieldLambda(FieldInfo field)
        {
            Expression deepCopyFieldExpression = this.GetDeepCopyFieldExpression(field);
            return Expression.Lambda<Action<T, T>>(deepCopyFieldExpression, new ParameterExpression[]
            {
                this._sourceExpr,
                this._destExpr
            });
        }

        internal IEnumerable<Expression> GetAllDeepCopyFieldExpressions()
        {
            return from field in typeof(T).GetFields()
                   select this.GetDeepCopyFieldExpression(field);
        }

        internal Expression<Action<T, T>> GetAllDeepCopyFieldLambda()
        {
            return this.Lambda(this.GetAllDeepCopyFieldExpressions());
        }

        internal Action<T, T> CompileAllDeepCopyFieldAction()
        {
            return this.GetAllDeepCopyFieldLambda().Compile();
        }

        internal Task<Action<T, T>> StartCompileAllDeepCopyFieldAction()
        {
            return Task.Run<Action<T, T>>(new Func<Action<T, T>>(this.CompileAllDeepCopyFieldAction));
        }

        public Expression GetDeepCopyFieldExpression(string fieldName)
        {
            return this.GetDeepCopyFieldExpression(typeof(T).GetField(fieldName));
        }

        public Expression GetDeepCopyFieldExpression(FieldInfo field)
        {
            Type fieldType = field.FieldType;
            MemberExpression dataExpr = Expression.Field(this._sourceExpr, field);
            Expression deepCloneExpression = this.GetDeepCloneExpression(fieldType, dataExpr);
            MemberExpression left = Expression.Field(this._destExpr, field);
            return Expression.Assign(left, deepCloneExpression);
        }

        public Expression<Action<T, T>> Lambda(Expression expression)
        {
            return Expression.Lambda<Action<T, T>>(expression, new ParameterExpression[]
            {
                this._sourceExpr,
                this._destExpr
            });
        }

        public Expression<Action<T, T>> Lambda(params Expression[] expressions)
        {
            return this.Lambda(expressions);
        }

        public Expression<Action<T, T>> Lambda(IEnumerable<Expression> expressions)
        {
            Expression expression = Expression.Block(expressions);
            return this.Lambda(expression);
        }

        public Action<T, T> Compile(Expression expression)
        {
            return this.Lambda(expression).Compile();
        }

        internal IEnumerable<Expression> GetAllShallowCopyFieldExpressions()
        {
            return from field in typeof(T).GetFields()
                   select this.GetShallowCopyFieldExpression(field);
        }

        internal Expression<Action<T, T>> GetShallowCopyCopyFieldLambda()
        {
            return this.Lambda(this.GetAllShallowCopyFieldExpressions());
        }

        internal Action<T, T> CompileAllShallowCopyFieldAction()
        {
            return this.GetShallowCopyCopyFieldLambda().Compile();
        }

        internal Task<Action<T, T>> StartCompileAllShallowCopyFieldAction()
        {
            return Task.Run<Action<T, T>>(new Func<Action<T, T>>(this.CompileAllShallowCopyFieldAction));
        }

        public Expression GetShallowCopyFieldExpression(FieldInfo field)
        {
            MemberExpression right = Expression.Field(this._sourceExpr, field);
            MemberExpression left = Expression.Field(this._destExpr, field);
            return Expression.Assign(left, right);
        }

        public Expression GetDeepCloneExpression(Type dataType, Expression dataExpr)
        {
            bool flag = dataType.IsValueType || typeof(string) == dataType;
            if (!flag)
            {
                bool isArray = dataType.IsArray;
                if (isArray)
                {
                    Type elementType = dataType.GetElementType();
                    bool flag2 = elementType.IsValueType || typeof(string) == elementType;
                    if (flag2)
                    {
                        MethodInfo method = typeof(ExpressionHelper).GetMethod("CloneArray", BindingFlags.Static | BindingFlags.Public);
                        MethodInfo method2 = method.MakeGenericMethod(new Type[]
                        {
                            elementType
                        });
                        return Expression.Call(method2, dataExpr);
                    }
                }
                else
                {
                    bool isGenericType = dataType.IsGenericType;
                    if (isGenericType)
                    {
                        Type[] genericArguments = dataType.GetGenericArguments();
                        Type genericTypeDefinition = dataType.GetGenericTypeDefinition();
                        bool flag3 = typeof(SortedDictionary<,>) == genericTypeDefinition;
                        if (flag3)
                        {
                            SortedDictionary<int, int> sortedDictionary = new SortedDictionary<int, int>();
                            Type type = genericArguments[0];
                            Type type2 = genericArguments[1];
                            bool flag4 = !type.IsValueType && typeof(string) != type;
                            if (flag4)
                            {
                                throw new NotSupportedException("SortedDictionary " + dataType.Name + " key is not a value type");
                            }
                            bool flag5 = type2.IsValueType || typeof(string) == type2;
                            if (flag5)
                            {
                                Type type3 = genericTypeDefinition.MakeGenericType(new Type[]
                                {
                                    type,
                                    type2
                                });
                                Type type4 = typeof(IDictionary<,>).MakeGenericType(new Type[]
                                {
                                    type,
                                    type2
                                });
                                Type type5 = typeof(IComparer<>).MakeGenericType(new Type[]
                                {
                                    type
                                });
                                ConstructorInfo constructor = type3.GetConstructor(new Type[]
                                {
                                    type4,
                                    type5
                                });
                                MemberExpression memberExpression = Expression.Property(dataExpr, "Comparer");
                                NewExpression notNullExpr = Expression.New(constructor, new Expression[]
                                {
                                    dataExpr,
                                    memberExpression
                                });
                                return DeepCopier<T>.getNullConditionalExpr(type3, dataExpr, notNullExpr);
                            }
                            ParameterExpression parameterExpression = Expression.Parameter(type2, "value");
                            Expression deepCloneExpression = this.GetDeepCloneExpression(type2, parameterExpression);
                            LambdaExpression arg = Expression.Lambda(deepCloneExpression, new ParameterExpression[]
                            {
                                parameterExpression
                            });
                            MethodInfo copySortedDictionaryGenericMethod = ExpressionHelper.GetCopySortedDictionaryGenericMethod(type, type2);
                            return Expression.Call(copySortedDictionaryGenericMethod, dataExpr, arg);
                        }
                        else
                        {
                            bool flag6 = typeof(Dictionary<,>) == genericTypeDefinition;
                            if (flag6)
                            {
                                Type type6 = genericArguments[0];
                                Type type7 = genericArguments[1];
                                bool flag7 = !type6.IsValueType && typeof(string) != type6;
                                if (flag7)
                                {
                                    throw new NotSupportedException("Dictionary " + dataType.Name + " key is not a value type");
                                }
                                bool flag8 = type7.IsValueType || typeof(string) == type7;
                                if (flag8)
                                {
                                    Type type8 = genericTypeDefinition.MakeGenericType(new Type[]
                                    {
                                        type6,
                                        type7
                                    });
                                    Type type9 = typeof(IDictionary<,>).MakeGenericType(new Type[]
                                    {
                                        type6,
                                        type7
                                    });
                                    ConstructorInfo constructor2 = type8.GetConstructor(new Type[]
                                    {
                                        type9
                                    });
                                    NewExpression notNullExpr2 = Expression.New(constructor2, new Expression[]
                                    {
                                        dataExpr
                                    });
                                    return DeepCopier<T>.getNullConditionalExpr(type8, dataExpr, notNullExpr2);
                                }
                                ParameterExpression parameterExpression2 = Expression.Parameter(type7, "value");
                                Expression deepCloneExpression2 = this.GetDeepCloneExpression(type7, parameterExpression2);
                                LambdaExpression arg2 = Expression.Lambda(deepCloneExpression2, new ParameterExpression[]
                                {
                                    parameterExpression2
                                });
                                MethodInfo copyDictionaryGenericMethod = ExpressionHelper.GetCopyDictionaryGenericMethod(type6, type7);
                                return Expression.Call(copyDictionaryGenericMethod, dataExpr, arg2);
                            }
                            else
                            {
                                bool flag9 = typeof(List<>) == genericTypeDefinition;
                                if (flag9)
                                {
                                    Type type10 = genericArguments[0];
                                    Type type11 = genericTypeDefinition.MakeGenericType(new Type[]
                                    {
                                        type10
                                    });
                                    Type type12 = typeof(IEnumerable<>).MakeGenericType(new Type[]
                                    {
                                        type10
                                    });
                                    bool flag10 = type10.IsValueType || typeof(string) == type10;
                                    if (flag10)
                                    {
                                        ConstructorInfo constructor3 = type11.GetConstructor(new Type[]
                                        {
                                            type12
                                        });
                                        NewExpression notNullExpr3 = Expression.New(constructor3, new Expression[]
                                        {
                                            dataExpr
                                        });
                                        return DeepCopier<T>.getNullConditionalExpr(type11, dataExpr, notNullExpr3);
                                    }
                                    ParameterExpression parameterExpression3 = Expression.Parameter(type10, "item");
                                    Expression deepCloneExpression3 = this.GetDeepCloneExpression(type10, parameterExpression3);
                                    LambdaExpression arg3 = Expression.Lambda(deepCloneExpression3, new ParameterExpression[]
                                    {
                                        parameterExpression3
                                    });
                                    MethodInfo selectGenericMethod = ExpressionHelper.GetSelectGenericMethod(type10, type10);
                                    MethodCallExpression arg4 = Expression.Call(selectGenericMethod, dataExpr, arg3);
                                    MethodInfo toListGenericMethod = ExpressionHelper.GetToListGenericMethod(type10);
                                    MethodCallExpression notNullExpr4 = Expression.Call(toListGenericMethod, arg4);
                                    return DeepCopier<T>.getNullConditionalExpr(type11, dataExpr, notNullExpr4);
                                }
                            }
                        }
                    }
                }
                throw new NotImplementedException(dataType.Name + " is not implemented");
            }
            return dataExpr;
        }

        private static ConditionalExpression getNullConditionalExpr(Type type, Expression testExpr, Expression notNullExpr)
        {
            ConstantExpression constantExpression = Expression.Constant(null, type);
            BinaryExpression test = Expression.Equal(testExpr, constantExpression);
            return Expression.Condition(test, constantExpression, notNullExpr);
        }

        private ParameterExpression _sourceExpr;

        private ParameterExpression _destExpr;
    }

    class ExpressionHelper
    {
        static public System.Reflection.MethodInfo GetSelectGenericMethod(Type sourceType, Type resultType)
        {
            var selectMethod = typeof(Enumerable).GetMethods().Where(
                m => m.Name == "Select" &&
                m.GetParameters()[1].ParameterType.Name == "Func`2").Single();
            return selectMethod.MakeGenericMethod(sourceType, resultType);
        }

        static public System.Reflection.MethodInfo GetToListGenericMethod(Type sourceType)
        {
            var toListMethod = typeof(Enumerable).GetMethod("ToList");
            return toListMethod.MakeGenericMethod(sourceType);
        }

        static public Type GetKeyValuePairType(Type keyType, Type valueType)
        {
            return typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
        }

        static public System.Reflection.MethodInfo GetToDictionaryGenericMethod(Type source ,Type keyType, Type valueType)
        {
            // ToDictionary(source, keySelector, elemSelector)
            var toDictMethod = typeof(Enumerable).GetMethods().Where(
                m => m.Name == "ToDictionary" &&
                m.GetParameters().Length == 3 &&
                m.GetGenericArguments().Length == 3).Single();
            return toDictMethod.MakeGenericMethod(source, keyType, valueType);
        }

        static public System.Reflection.MethodInfo GetToDictionaryGenericMethod(Type keyType, Type valueType)
            => GetToDictionaryGenericMethod(GetKeyValuePairType(keyType, valueType), keyType, valueType);

        static public System.Reflection.MethodInfo GetCopyDictionaryGenericMethod(Type keyType, Type elementType)
        {
            var method = typeof(ExpressionHelper).GetMethod("CopyDictionary");
            return method.MakeGenericMethod(keyType, elementType);
        }

        public static Dictionary<TKey, TElement> CopyDictionary<TKey, TElement>(IDictionary<TKey, TElement> source, Func<TElement, TElement> elementCopier)
        {
            if (source == null) return null;
            Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>();
            foreach (var kvp in source)
            {
                d.Add(kvp.Key, elementCopier(kvp.Value));
            }
            return d;
        }

        public static SortedDictionary<TKey, TElement> CopySortedDictionary<TKey, TElement>(SortedDictionary<TKey, TElement> source, Func<TElement, TElement> elementCopier)
        {
            if (source == null) return null;
            SortedDictionary<TKey, TElement> d = new SortedDictionary<TKey, TElement>(source.Comparer);
            foreach (var kvp in source)
            {
                d.Add(kvp.Key, elementCopier(kvp.Value));
            }
            return d;
        }

        static public System.Reflection.MethodInfo GetCopySortedDictionaryGenericMethod(Type keyType, Type elementType)
        {
            // ToDictionary(source, keySelector, elemSelector)
            var method = typeof(ExpressionHelper).GetMethod("CopySortedDictionary");
            return method.MakeGenericMethod(keyType, elementType);
        }

        public static T[] CloneArray<T>(T[] sourceArray)
        {
            if (sourceArray == null) return null;
            T[] destArr = new T[sourceArray.Length];
            sourceArray.CopyTo(destArr, 0);
            return destArr;
        }

        public static T[] CloneArray2<T>(T[] sourceArray, Func<T, T> elementCopier)
        {
            if (sourceArray == null) return null;
            T[] destArr = new T[sourceArray.Length];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                destArr[i] = elementCopier(sourceArray[i]);
            }
            return destArr;
        }
    }
}
