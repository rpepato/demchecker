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

using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.domain
{
    public class DemeterAnalysis
    {
        private static DemeterAnalysis _instance;


        internal Method CurrentMethod { get; private set; }
        internal Class CurrentClass { get; private set; }
        internal Project CurrentProject { get; private set; }
        internal Solution CurrentSolution { get; private set; }
        internal File CurrentFile { get; private set; }

        public IList<Class> Classes { get; private set; }
        public IList<Project> Projects { get; private set; }
        public IList<Solution> Solutions { get; private set; }
        public IList<File> Files { get; private set; }

        internal IList<IViolation> Violations { get; private set; }

        public int TotalOfInspectedInstructions { get; private set; }

        private DemeterAnalysis()
        {
            Classes = new List<Class>();
            Projects = new List<Project>();
            Solutions = new List<Solution>();
            Files = new List<File>();
            Violations = new List<IViolation>();
            TotalOfInspectedInstructions = 0;
        }

        public void AddMethod(Method method)
        {
            DemeterAnalysis.Current.CurrentClass.Methods.Add(method);
            DemeterAnalysis.Current.CurrentMethod = method;
        }

        public void AddClass(Class klass)
        {
            DemeterAnalysis.Current.CurrentClass = klass;
            DemeterAnalysis.Current.Classes.Add(klass);
        }

        public void AddProject(Project project)
        {
            DemeterAnalysis.Current.CurrentProject = project;
            DemeterAnalysis.Current.Projects.Add(project);
        }

        public void AddSolution(Solution solution)
        {
            DemeterAnalysis.Current.CurrentSolution = solution;
            DemeterAnalysis.Current.Solutions.Add(solution);
        }

        public void AddFiles(File file)
        {
            DemeterAnalysis.Current.CurrentFile = file;
            DemeterAnalysis.Current.Files.Add(file);
        }

        public void AddViolation(Violation violation)
        {
            DemeterAnalysis.Current.Violations.Add(violation);
            DemeterAnalysis.Current.CurrentClass.Violations.Add(violation);
            if (DemeterAnalysis.Current.CurrentMethod != null)
            {
                DemeterAnalysis.Current.CurrentMethod.Violations.Add(violation);
            }
        }

        public void IncrementInspectedInstruction()
        {
            TotalOfInspectedInstructions++;
        }

        public static DemeterAnalysis Current { 
            get
            {
                if (_instance == null)
                {
                    _instance = new DemeterAnalysis();
                }
                return _instance;
            }
        }

        internal static void Reset() {
            _instance = null;
        }
    }
}
