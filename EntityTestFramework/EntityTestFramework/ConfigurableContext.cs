using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using EntityTestFramework.Exceptions;
using EntityTestFramework.ExpressionHelpers;
using Microsoft.Data.Entity;

namespace EntityTestFramework
{
    public class ConfigurableContext<T> where T : DbContext, new()
    {
        private readonly T _context;
        private readonly Dictionary<Type, object> _data;

        public ConfigurableContext(Action<ConfigurableContext<T>> configuration)
        {
            _data = new Dictionary<Type, object>();

            _context = CreateContextInstanceWithFakeSaveMethod()
            configuration.Invoke(this);
        }

        private T CreateContextInstanceWithFakeSaveMethod()
        {
            const string name = nameof(ConfigurableContext<T>);
            var asn = new AssemblyName(name);
            var builder = AppDomain.CurrentDomain.DefineDynamicAssembly(asn, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = builder.DefineDynamicModule(name, name + ".dll");

            var genericType = typeof(T);

            var typeBuilder = moduleBuilder.DefineType(genericType.Name + "Fake", genericType.Attributes, genericType);

            CreateDefaultSaveChangesMethod(typeBuilder, genericType);
            //CreateParameterSaveChangesMethod(typeBuilder, genericType);
            //CreateAsyncSaveChangesMethod(typeBuilder, genericType);
            //CreateAsyncSaveChangesParameterMethod(typeBuilder, genericType);


            var newContextType = typeBuilder.CreateType();

            return (T)Activator.CreateInstance(newContextType);
        }

        private static void CreateDefaultSaveChangesMethod(TypeBuilder typeBuilder, Type genericType)
        {
            var saveChangesSig = typeBuilder.DefineMethod("SaveChanges",
                MethodAttributes.Public | MethodAttributes.Virtual, typeof (int), Type.EmptyTypes);

            var gen = saveChangesSig.GetILGenerator();

            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(saveChangesSig, genericType.GetMethod("SaveChanges", new Type[0]));
        }


        public static implicit operator T(ConfigurableContext<T> configurableContext)
        {
            return configurableContext._context;
        }

        public void Setup<TU>(Expression<Func<T, DbSet<TU>>> property) where TU : class, new()
        {
            Setup(property, new List<TU>());
        }

        public void Setup<TU>(Expression<Func<T, DbSet<TU>>> property, params TU[] seeds) where TU : class, new()
        {
            Setup(property, seeds.ToList());
        }

        public void Setup<TU>(Expression<Func<T, DbSet<TU>>> property, List<TU> seed) where TU : class, new()
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member.Member as PropertyInfo;

            var fakeDbSet = new FakeDbSet<TU>(seed);

            _data.Add(typeof(TU), fakeDbSet);
            propertyInfo.SetValue(_context, fakeDbSet);
        }

        public void HasBeenSaved<TU>(Expression<Func<TU, bool>> assertions) where TU : class
        {
            var exactResults = GetStoredEntities<TU>();
            ExpressionAnalyser.CheckMessagesMatch(exactResults, assertions);
        }

        public void HasNotBeenSaved<TU>(Expression<Func<TU, bool>> assertions) where TU : class
        {
            var results = GetStoredEntities<TU>();
            ExpressionAnalyser.CheckMessagesDoNotMatch(results, assertions);
        }

        public void HasNotBeenSaved<TU>() where TU : class
        {
            var results = GetStoredEntities<TU>();
            if (results.Any())
            {
                throw new EntitiesSavedException($"Entities of type {typeof(TU).Name} have been saved");
            }
        }

        private List<TU> GetStoredEntities<TU>() where TU : class
        {
            if (!_data.ContainsKey(typeof(TU)))
            {
                throw new Exception();
            }

            var fakeDbSet = _data[typeof(TU)] as FakeDbSet<TU>;

            return fakeDbSet.Entities;
        }
    }
}
