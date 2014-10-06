using ICSharpCode.NRefactory.TypeSystem;
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

        public Stack<Method> Methods { get; private set; }

        public Class()
        {
            DeclaredTypes = new List<string>();
            Methods = new Stack<Method>(); // Why this is stack?
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
