//The MIT License (MIT)

//Copyright (c) 2015 Roberto Pepato

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
