using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    class ChangeFile
    {
        string path;
        string type;
        public ChangeFile(string _type, string _path)
        {
            path = _path;
            type = _type;
        }

        public void StartChange()
        {
            SplitTree splitTree = new SplitTree(type, path);
            splitTree.StartSplit();
            foreach (var method in splitTree.methodStructs)
            {
                Console.WriteLine(method.solution.FilePath + " " + method.project.Name + " " + method.document.Name + " " + method.method.Identifier);
                string pattern = @"(^[^{]+{)|(}$)";
                string target = " ";
                Regex regex = new Regex(pattern);
                string result = regex.Replace(method.method.ToString(), target);
                SearchSQL a = new SearchSQL();
                a.Search(result);
            }
        }
    }
}
