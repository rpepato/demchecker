using demchecker.domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker
{
    public class CSVExtractor
    {
        private IList<Violation> Violations { get; set; }

        public CSVExtractor(IList<Violation> violations)
        {
            Violations = violations;
        }

        public void Generate()
        {
            string path = @"C:\temp\lod-e-concrete-resume.csv";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("projeto;classe;violacoes");
            foreach(var project in Violations.Select(v => v.ProjectName).Distinct())
            {
                foreach(var klass in Violations.Where(v => v.ProjectName == project).Select(v => v.Klass).Distinct())
                {
                    var line = project + ";";
                    line += klass.Name + ";" + klass.Violations.Count();
                    sb.AppendLine(line);
                }
            }
            File.WriteAllText(path, sb.ToString());
        }

        public void Full()
        {
            string path = @"C:\temp\lod-e-concrete-full.csv";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("projeto;metodo;classe;arquivo;linha;expressao");
            foreach(var v in DemeterAnalysis.Current.Violations)
            {
                var line = v.ProjectName + ";";
                line += v.Method.Name + ";";
                line += v.Klass.Name + ";";
                line += v.FileName + ";";
                line += v.LineNumber + ";";
                line += v.ViolationExpression;
                sb.AppendLine(line);
            }
            File.WriteAllText(path, sb.ToString());
        }
    }
}
