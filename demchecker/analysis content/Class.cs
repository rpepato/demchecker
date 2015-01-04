﻿using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.analysis_content
{
   public class Class
    {
        public string Name { get; internal set; }

        public string FullQualifiedName { get; internal set; }

        public IList<string> DeclaredTypes { get; private set; }

        public IList<Method> Methods { get; private set; }

        public IList<Violation> Violations { get; private set; }

        public Class()
        {
            DeclaredTypes = new List<string>();
            Methods = new List<Method>(); // Why this is stack?
            Violations = new List<Violation>();
        }
         
        public bool Equals(Class x, Class y)
        {
 	        return x.FullQualifiedName == y.FullQualifiedName;
        }

        public int GetHashCode(Class obj)
        {
            return obj.FullQualifiedName.GetHashCode();
        }

       public void AddDeclaredType(string typeName)
        {
           if (!DeclaredTypes.Contains(typeName))
           {
               DeclaredTypes.Add(typeName);
           }
        }
    }
}
