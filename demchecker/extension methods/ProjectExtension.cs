using ICSharpCode.NRefactory.CSharp;
using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.extension_methods
{
    public static class ProjectExtension
    {
        public static IList<string> GetNamespaceDeclarations(this Project project)
        {
            IList<string> namespaceList = new List<string>();
            foreach (var file in project.Files)
            {
                var namespaces = file.SyntaxTree.Descendants.OfType<NamespaceDeclaration>();
                foreach (var nameSpace in namespaces)
                {
                    if (!namespaceList.Contains(nameSpace.Name.ToUpper()))
                    {
                        namespaceList.Add(nameSpace.Name.ToUpper());
                    }
                }
            }

            return namespaceList;
        }
    }
}
