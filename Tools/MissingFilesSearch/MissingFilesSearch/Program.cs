
using System.IO;
using System.Text;

namespace MissingFilesSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments.ArgumentReader(args).GetArgs();
            if(!string.IsNullOrWhiteSpace(arguments.Path))
            {
                GetFiles(arguments, "dbr");                
                GetFiles(arguments, "tex");
                GetFiles(arguments, "pfx");
                GetFiles(arguments, "msh");
                GetFiles(arguments, "qst");
                GetFiles(arguments, "anm");
            }
        }

        private static void GetFiles(Arguments.Args arguments, string pattern)
        {
            var files = new Search.Searcher().GetFiles(arguments, pattern);
            var sbuilder = new StringBuilder();
            foreach (var item in files)
            {
                sbuilder.AppendLine(item.ToString());
                sbuilder.AppendLine("");
            }
            File.WriteAllText(string.Format("missing.{1}.{0:yyyyMMddHHmmss}.log", System.DateTime.Now, pattern), sbuilder.ToString());
            System.Console.WriteLine("");
        }
    }
}
