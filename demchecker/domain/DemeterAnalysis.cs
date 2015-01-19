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
