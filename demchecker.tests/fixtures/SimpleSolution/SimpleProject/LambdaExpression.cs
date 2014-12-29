using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    class LambdaExpression
    {
        public static Func<Assembly, bool>[] DefaultAssembliesToScan = new Func<Assembly, bool>[]
        {
            x =>
            {
                return !x.GetName().Name.StartsWith("Nancy.Testing",StringComparison.OrdinalIgnoreCase) &&
                    x.GetReferencedAssemblies().Any(r => r.Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase));
            }
        };
    }
}
