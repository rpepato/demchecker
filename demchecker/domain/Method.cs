using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.domain
{
    public class Method
    {
        public Class Class { get; private set; }
        public string Name { get; private set; }
        public IList<string> ParameterTypes { get; private set;}

        public IList<string> LocalVariables { get; private set; }

        public IList<Violation> Violations { get; private set; }

        public Method(Class klass, string name)
        {
            this.Class = klass;
            this.Name = name;
            this.ParameterTypes = new List<string>();
            LocalVariables = new List<string>();
            Violations = new List<Violation>();
        }

        public void AddLocalVariable(string localVariable)
        {
            if (!LocalVariables.Contains(localVariable))
            {
                LocalVariables.Add(localVariable);
            }
        }

        public void AddParameterType(string parameterType)
        {
            if (!ParameterTypes.Contains(parameterType))
            {
                ParameterTypes.Add(parameterType);
            }
        }
    }
}
