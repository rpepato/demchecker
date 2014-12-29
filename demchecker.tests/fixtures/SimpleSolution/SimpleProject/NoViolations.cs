using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProject
{
    public class NoViolations
    {
        private string myMemberReference;
        private EntityTransaction transaction;

        public void MyFirstMethod(int parameter, NoViolations anotherParameter, StreamReader yetAnotherParameter, Queue<string> anotherOne)
        {
            var reader = new StreamReader("");
            string x = "10";
            var s = "this is a char array".ToCharArray();
            var y = s;
            var j = new List<double>();
            var z = this.GetSomeData().First();
        }

        public string GetSomeData()
        {
            return null;
        }
    }
}
