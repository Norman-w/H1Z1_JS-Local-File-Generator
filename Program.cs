using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace H1Z1_JS语言文件生成器
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("正在读取源文件");
            //读取原始文件
            string srcDirFile = "基础文件\\en_us_data.dir";
            string srcDatFile = "基础文件\\en_us_data.dat";
            Processor pr = new Processor();
            var retOrangnal = pr.ReadENFile(srcDirFile, srcDatFile);
            Console.WriteLine("读取源文件完成,正在读取要替换的字典文件\r\n");
            //读取要替换的字典文件
            string dicFilePath = "字典.txt";
            var lines = System.IO.File.ReadAllLines(dicFilePath);
            if (lines == null || lines.Length < 1)
            {
                Console.WriteLine("文件为空");
                Console.ReadLine();
                return;
            }
            var headers = new Dictionary<string, string>();
            var index = 1;
            foreach (var line in lines)
            {
                var kv = line.Split(new char[] { '\t' });
                if (kv == null || kv.Length != 2)
                {
                    Console.WriteLine("这行数据有问题:{0}", line);
                }
                var k = kv[0];
                var v = kv[1];
                if (headers.ContainsKey(k))
                {
                    Console.WriteLine("这行重复:{0}", line);
                }
                headers.Add(k, v);
                Console.WriteLine("当前第{0}行", index);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\t"+k);
                Console.ResetColor();
                Console.WriteLine("将替换为");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\t" + v);
                Console.ResetColor();
                Console.WriteLine("");
                index++;
            }
            Console.WriteLine("读取字典文件完成,正在替换内容");
            //替换
            pr.ReplaceStringRecord(retOrangnal, null, new List<string>() { "AU", "US", "EU" }, headers);
            //生成新文件
            if (System.IO.Directory.Exists("生成的文件"))
            {
                System.IO.Directory.CreateDirectory("生成的文件");
            }
            Console.WriteLine("替换内容完成,正在保存");
            string destDirFile = "生成的文件\\en_us_data.dir";
            string destDatFile = "生成的文件\\en_us_data.dat";
            var dirFileHeaders = LoadDirFileHeader(srcDirFile);

            Console.ForegroundColor = ConsoleColor.Green;
            pr.CreateDirFile(destDirFile, retOrangnal, dirFileHeaders);
            Console.WriteLine("dir文件已保存在  {0}",destDirFile);
            pr.CreateDataFile(destDatFile, retOrangnal);
            Console.WriteLine("dat文件已保存在  {0}", destDatFile);

            Console.WriteLine("\r\n^_^本次替换完成^_^\r\n\r\n");
            string norman = "This tool is made by [Norman]";

            foreach (var c in norman)
            {
                Console.Write(c);
                Console.Beep((int)c * 50, 100);
            }
            Console.Beep(3000, 1000);
            Console.ResetColor();

            Console.ReadLine();
        }
        void Testing()
        {
            Processor pr = new Processor();
            var retEN = pr.ReadENFile(@"D:\qp.enni.kr\file\en_us_data.dir", @"D:\qp.enni.kr\file\en_us_data.dat");
            var retCN = pr.ReadENFile(@"D:\qp.enni.kr\file\zh_cn_data.dir", @"D:\qp.enni.kr\file\zh_cn_data.dat");



            //var retTempFile = pr.CreateDataFile(null, retEN);
            //检查文件中的字符计算是否有问题
            var retCNCreated = pr.ReadENFile(@"D:\qp.enni.kr\file\newH1Z1LocalFile\en_us_data.dir", @"D:\qp.enni.kr\file\newH1Z1LocalFile\en_us_data.dat");
            pr.CheckValueLength(retCNCreated);


            #region 文件对比 对比出来的原始文件和新生成的文件的内容是完全一样的.

            ////原始没有修改的文件是什么内容
            //var allBytesOrangnal = System.IO.File.ReadAllBytes(@"D:\qp.enni.kr\file\en_us_data.dat");
            ////新创建的文件是什么内容
            //var allBytesCreated = pr.CreateDataFile(null, retEN);
            //Dictionary<long, byte> dic = new Dictionary<long, byte>();
            //for (int i = 0; i < allBytesOrangnal.Length; i++)
            //{
            //    dic.Add(i, allBytesOrangnal[i]);
            //}
            //for (int i = 0; i < allBytesCreated.Length; i++)
            //{
            //    var current = allBytesCreated[i];
            //    //aaa += (char)current;
            //    var oldValue = dic[i];
            //    var newValue = allBytesCreated[i];
            //    //char oldChar = (char)oldValue;
            //    //char newChar = (char)newValue;
            //    if (oldValue != newValue)
            //    {

            //    }
            //}
            #endregion
            List<string> ignoreKeys = new List<string>() { "AU", "US", "EU" };
            Dictionary<string, string> waitReplaceByUserDic = new Dictionary<string, string>();
            waitReplaceByUserDic.Add("A-Z", "H1emu中文社区");
            waitReplaceByUserDic.Add("Hi there dude", "H1emu中文社区(Azaz.ge)");
            pr.ReplaceStringRecord(retEN, retCN, ignoreKeys, waitReplaceByUserDic);
            var allLinesEN = System.IO.File.ReadAllLines(@"D:\qp.enni.kr\file\en_us_data.dir", Encoding.UTF8);
            List<string> headers = new List<string>();
            foreach (var item in allLinesEN)
            {
                if (!item.StartsWith("##"))
                {
                    break;
                }
                headers.Add(item);
            }
            pr.CreateDataFile(@"D:\qp.enni.kr\file\newH1Z1LocalFile\en_us_data.dat", retEN);
            pr.CreateDirFile(@"D:\qp.enni.kr\file\newH1Z1LocalFile\en_us_data.dir", retEN, headers);
            //Console.WriteLine(sb);
            Console.ReadLine();
        }
        /// <summary>
        /// 从一个文件中读取dir文件的文件头
        /// </summary>
        /// <param name="dirFilePath"></param>
        /// <returns></returns>
        static List<string> LoadDirFileHeader(string dirFilePath)
        {
            var allLinesEN = System.IO.File.ReadAllLines(dirFilePath, Encoding.UTF8);
            List<string> headers = new List<string>();
            foreach (var item in allLinesEN)
            {
                if (!item.StartsWith("##"))
                {
                    break;
                }
                headers.Add(item);
            }
            return headers;
        }
    }
}
