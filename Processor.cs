using System;
using System.Collections.Generic;
using System.Text;

namespace H1Z1_JS语言文件生成器
{
    public class Processor
    {
        //public Dictionary<long, StringRecord> ReadENFile(byte[] dirFileBytes, byte[] dataFileBytes)
        //{

        //}
        public Dictionary<long,StringRecord> ReadENFile(string dirFilePath, string dataFilePath)
        {
            var ret = new Dictionary<long,StringRecord>();
            string[] dirfileLines = System.IO.File.ReadAllLines(dirFilePath);
            string[] datafileLines = System.IO.File.ReadAllLines(dataFilePath);
            foreach (var dir in dirfileLines)
            {
                if (dir.StartsWith("##"))
                {
                    continue;
                }
                string[] t4 = dir.Split(new char[] { '\t' });
                StringRecord record = new StringRecord();
                record.Index = long.Parse(t4[0]);
                record.Start = long.Parse(t4[1]);
                record.Length = long.Parse(t4[2]);
                ret.Add(record.Index, record);
            }
            StringRecord lastRecord = null;
            foreach (var data in datafileLines)
            {
                var t3 = data.Split(new char[] { '\t' });
                StringRecord record;
                if (t3.Length!= 3)
                {
                    record = lastRecord;
                    record.Value += "\n"+t3[0];
                }
                else
                {
                    var index = long.Parse(t3[0]);
                    if (ret.ContainsKey(index) == false)
                    {
                        continue;
                    }
                    record = ret[index];
                    record.Value = t3[2];//.Trim(new char[] { '\r', '\n' });
                    record.Type = t3[1];
                }
                lastRecord = record;
            }
            return ret;
        }
        public void ReplaceStringRecord(Dictionary<long,StringRecord> dest,
            Dictionary<long,StringRecord> src,
            List<string> ignoreValues,
            Dictionary<string,string> waitReplaceByUserDic
            )
        {
            //int max = 4000;
            //int current = 0;
            //把dest中的字符替换成src里面的.如果没有的话保持原来的.替换后重新计算长度信息.
            long currentStartIndex = 3;
            foreach (var d in dest)
            {
                //current++;
                
                var index = d.Key;
                long newLength = d.Value.Length;
                var newValue = d.Value.Value;
                var newType = d.Value.Type;
                ////if (current < max)
                ////{

                ////}
                ////else 
                if (ignoreValues.Contains(d.Value.Value) == true)
                {

                }
                else if(waitReplaceByUserDic.ContainsKey(d.Value.Value))
                {
                    //如果用户指定的要替换某个字符,使用用户制定的
                    newValue = waitReplaceByUserDic[d.Value.Value];
                    newLength = Encoding.UTF8.GetByteCount(newValue) + index.ToString().Length + 1 + newType.Length + 1;
                    newType = d.Value.Type;
                }
                else if (src!= null && src.ContainsKey(index))
                {
                    //如果中文里面包含这个字典.替换并计算一下长度
                    newValue = src[index].Value;
                    //var srcValueLength = Encoding.UTF8.GetByteCount(newValue);
                    var srcValueLength = src[index].Length;
                    var length2 = Encoding.UTF8.GetByteCount(src[index].Value) + index.ToString().Length + 1 + newType.Length + 1;
                    if (srcValueLength!= length2)
                    {
                        
                    }
                    newLength = srcValueLength;
                    newType = src[index].Type;
                }
                d.Value.Type = newType;
                d.Value.Value = newValue;
                d.Value.Length = newLength;
                d.Value.Start = currentStartIndex;
                currentStartIndex = d.Value.Start + d.Value.Length + 2;
            }
        }
        public byte[] CreateDirFile(string destPath, Dictionary<long,StringRecord> records, List<string> headers)
        {
            StringBuilder fileBuilder = new StringBuilder();
            foreach (var item in headers)
            {
                fileBuilder.Append(item).Append("\r\n");
            }
            foreach (var r in records)
            {
                var index = r.Key;
                var record = r.Value;
                fileBuilder.Append(record.Index).Append('\t').Append(record.Start).Append('\t').Append(record.Length).Append('\t').Append('d').Append("\r\n");
            }
            var ret = Encoding.UTF8.GetBytes(fileBuilder.ToString());
            if (string.IsNullOrEmpty(destPath))
            {
            }
            else
            {
                System.IO.File.WriteAllBytes(destPath, ret);
            }
            return ret;
        }

        public byte[] CreateDataFile(string destPath, Dictionary<long, StringRecord> records)
        {
            StringBuilder fileBuilder = new StringBuilder();
            foreach (var r in records)
            {
                var record = r.Value;
                fileBuilder.Append(record.Index).Append('\t').Append(record.Type).Append('\t').Append(record.Value).Append("\r\n");
            }
            var list = new List<byte>(){239,187,191};
            list.AddRange(Encoding.UTF8.GetBytes(fileBuilder.ToString()));
            var ret = list.ToArray();
            if (string.IsNullOrEmpty(destPath))
            {
            }
            else
            {
                System.IO.File.WriteAllBytes(destPath, ret);
            }
            return ret;
        }

        public void CheckValueLength(Dictionary<long,StringRecord> records)
        {
            foreach (var r in records)
            {
                var record = r.Value;
                if (record.Value.Contains("哈姆雷特"))
                {

                }
                var lengthInFile = record.Length;
                var lengthCalced = Encoding.UTF8.GetByteCount(record.Value) + record.Index.ToString().Length + 1 + record.Type.Length + 1;
                if (lengthInFile != lengthCalced)
                {

                }
            }
        }
    }
}
