using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    class ClassWithMethodCallOnPrimitiveTypeInLambdaConstruction
    {
        public void MyMethod()
        {
            List<int> integers = new List<int>();
            integers.Where(i => i.ToString() == "0");
        }
    }
}
