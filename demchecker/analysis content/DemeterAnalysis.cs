using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.analysis_content
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

        public IList<Violation> Violations { get; private set; }

        private DemeterAnalysis()
        {
            Classes = new List<Class>();
            Projects = new List<Project>();
            Solutions = new List<Solution>();
            Files = new List<File>();
            Violations = new List<Violation>();
        }

        public void AddMethod(Method method)
        {
            DemeterAnalysis.Current.CurrentClass.Methods.Push(method);
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
