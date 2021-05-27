using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class ChangeFile
    {
        string path;
        string type;
        public ChangeFile(string _type, string _path)
        {
            path = _path;
            type = _type;
        }
        public Dictionary<MethodStruct, List<SqlToEfStruct>> StartChange()
        {
            SplitTree splitTree = new SplitTree(type, path);
            splitTree.StartSplit();
            Dictionary<MethodStruct, List<SqlToEfStruct>> fileReplace = new Dictionary<MethodStruct, List<SqlToEfStruct>>();
            foreach (var method in splitTree.methodStructs)
            {                
                string pattern = @"(^[^{]+{)|(}$)";
                string target = " ";
                Regex regex = new Regex(pattern);
                string result = regex.Replace(method.method.ToString(), target);
                SearchSQL a = new SearchSQL();
                List<SqlToEfStruct> arr = a.Search(result);
                fileReplace.Add(method, arr);
            }
            return fileReplace;
        }
    }
}
