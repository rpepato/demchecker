using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    class ViolationsUsingExternalTypes
    {
        List<Double> doubles = new List<double>();
        
        public void MyMethod(int i)
        {
            var file = File.OpenRead(@"C:\point_to_some_file.txt");
            byte[] bytes = new byte[] {};
            //this is a LoD Violation
            var data = file.BeginRead(bytes, 1024, 1024, null, null).IsCompleted;
            
            //this is not - we have an int param
            var j = Convert.ToInt32("10").ToString();

            var f = doubles.FirstOrDefault(d => d.GetTypeCode().GetType().ToString() == "10");
        }
    }
}
