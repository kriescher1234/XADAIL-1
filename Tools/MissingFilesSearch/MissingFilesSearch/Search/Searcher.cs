using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace MissingFilesSearch.Search
{
    internal class Searcher
    {
        private static int _counter;
        private static int _count;
        private static int _percent;
        private static object _lock = new object();
        private static string _pattern = string.Empty;        

        public List<Dto.FilesReference> GetFiles(Arguments.Args args, string pattern = "*")
        {
            _counter = 0;
            _percent = 0;
            _pattern = pattern;
            var dbrFiles = GetDbrFiles(args);
            var files = Directory.GetFiles(args.Path, "*." + pattern, SearchOption.AllDirectories).ToList();
            List<string> whitePattern = GetWhitelistPattern(args);

            var grimFiles = Directory.GetFiles(args.GrimDawnPath, "*." + pattern, SearchOption.AllDirectories).ToList();
            grimFiles.AddRange(files);
            if (whitePattern.Count > 0)
            {
                dbrFiles = dbrFiles.Where(x => whitePattern.Any(x.Contains)).ToList();
            }
            _count = dbrFiles.Count;
            return dbrFiles.AsParallel()
                 .Select(x => new Dto.FilesReference { Dbr = x, References = InitializeFilesReferences(x, pattern, grimFiles, args) })
                 .Where(x => x.References.Count > 0).ToList();
        }

        protected virtual List<string> GetDbrFiles(Arguments.Args args)
        {
            return Directory.GetFiles(args.Path, "*.dbr", SearchOption.AllDirectories).ToList();
        }

        private static List<string> GetWhitelistPattern(Arguments.Args args)
        {
            return (string.IsNullOrWhiteSpace(args.WhitePattern) ? "" : args.WhitePattern)
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToList();
        }

        private List<string> InitializeFilesReferences(string file, string pattern, List<string> files, Arguments.Args args)
        {
            var references = new List<string>();
            var text = File.ReadAllLines(file).ToList();
            var ignorePatterns = (string.IsNullOrWhiteSpace(args.IgnorePattern) ? "!!!!" : args.IgnorePattern)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower()).ToList();            
            references.AddRange(text.AsParallel().Where(x => x.EndsWith("." + pattern + ","))
                .Where(x => !x.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("fileNameHistoryEntry", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("/", "\\"))                 
                .SelectMany(x => x.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => !ignorePatterns.Any(y => x.Contains(y.ToLower())))
                .Where(x => !files.Any(y => y.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                .ToList());
            Increment();
            return references;
        }

        private static void Increment()
        {
            lock (_lock)
            {
                _counter++;
                var percent = (_counter * 100 / _count);
                var update = _percent != percent;
                if (update)
                {
                    _percent = percent;                    
                    Console.Write("\rValidating ({3}) files: {0}/{1} ({2}%)", _counter, _count, percent, _pattern);
                }
            }
        }
    }
}
