using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    class ClassWithACallOnStaticMethod
    {
        int x = 1;
        public void WillCallAStaticMethod()
        {
            AssemblyName.Equals(null, null);
        }
    }
}
