using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoron.Model;

namespace Yoron
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fileName = RequestFileName(args);
            try
            {
                var souceCode = await ReadCode(fileName);
                Compiler.Compile(souceCode, fileName.Remove(fileName.Length - 4, 3));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                throw;
            }
            Console.WriteLine("コンパイルが終了しました。");
            Console.ReadLine();
        }

        private static string RequestFileName(string[] args)
        {
            if (!args.Any() || !File.Exists(args[0]))
            {
                string fileName;
                do
                {
                    Console.WriteLine("ファイル名を入力してください。");
                    fileName = Console.ReadLine();
                } while (!File.Exists(fileName));
                return fileName;
            }
            return args[0];
        }

        private static async Task<string> ReadCode(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
