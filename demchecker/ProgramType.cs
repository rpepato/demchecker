using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker
{
    public class ProgramType
    {
        public ProgramType()
        {
            MemberVariables = new List<string>();
            MemberTypes = new List<IType>();
        }

        public IList<string> MemberVariables { get; private set; }

        public IList<IType> MemberTypes { get; private set; }
    }
}
