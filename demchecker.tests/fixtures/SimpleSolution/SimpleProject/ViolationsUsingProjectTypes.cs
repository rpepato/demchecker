using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    class ViolationsUsingProjectTypes
    {
        private CustomClass customClass;

        public void Test()
        {
            customClass = new CustomClass();
            // LoD Violation
            customClass.GetAnotherCustomClass().DoSomething();
            customClass.GetAnotherCustomClass().GetYetAnotherCustomClass().Perform();
        }
    }
}
