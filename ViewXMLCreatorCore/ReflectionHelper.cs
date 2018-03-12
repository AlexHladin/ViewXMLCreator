using System.Linq;
using System.Reflection;
using System;

namespace ViewXMLCreatorCore
{
    public static class ReflectionHelper
    {

        public static Type[] GetAllClassInNamespace(string namespaceName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.Namespace == namespaceName)
                .ToArray();
        }

        public static PropertyInfo[] GetPropertiesOfClass(object someClass)
        {
            return someClass.GetType()
                .GetProperties()
                .ToArray();
        }
    }
}
