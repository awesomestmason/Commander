using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Commander.Core.CommandProvider;

public static class ProviderHelper
{
    public static void AddHandlersFromAssembly(CommandProvider provider, Assembly assembly)
    {
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ICommandHandler<>))))
                .ToList();
        foreach (var handlerType in commandHandlerTypes)
        {
            if (!handlerType.IsValueType && handlerType.GetConstructor(Type.EmptyTypes) == null)
            {
                continue;
            }
            object? newInstance = Activator.CreateInstance(handlerType);
            if(newInstance == null)
            {
                continue;
            }
            var handlerInterfaceTypes = handlerType.GetInterfaces().Where(t => t.GetGenericTypeDefinition().Equals(typeof(ICommandHandler<>)));
            foreach(var handlerInterfaceType in handlerInterfaceTypes)
            {
                var commandType = handlerInterfaceType.GetGenericArguments()[0];
                MethodInfo addMethod = typeof(CommandProvider).GetMethod(nameof(CommandProvider.SetHandler))!;
                MethodInfo addMethodGeneric = addMethod.MakeGenericMethod(commandType);
                addMethodGeneric.Invoke(provider, new object[] { newInstance });
            }
        }
    }
}
