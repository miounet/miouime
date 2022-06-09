using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
namespace MiouIME.Core
{
    /// <summary>
    /// 用于统计用户输出字词频的类
    /// </summary>
    public class Counter
    {
        public static List<CountObj> CounterList = new List<CountObj>();
        public static bool MustWhile=false;
        static Counter()
        {
            //Thread thini = new Thread(new ThreadStart(IniCounter));
            //thini.Start();
        }

        private static void IniCounter()
        {
            string[] ary = File.ReadAllLines(InputHelp.appPath + "\\Counter\\Counter.txt", Encoding.Unicode);
            foreach (string s in ary)
            {
                if (s.Length > 0)
                    GetCounterByValue(s.Split(' ')[0], s.Split(' ')[1], Convert.ToDateTime(s.Split(' ')[3])).Count = Int32.Parse(s.Split(' ')[2]);
            }
        }
        public static CountObj GetCounterByValue(string value, string code,DateTime dt)
        {
            for (int i = 0; i < CounterList.Count; i++)
            {
                if (CounterList[i].Value == value)
                    return CounterList[i];
            }
            CountObj obj = new CountObj();
            obj.Value = value;
            obj.Count = 0;
            obj.Code = code;
            obj.DT = dt;
            CounterList.Add(obj);
            return obj;
        }

        public static void AddCounter1(object VC)
        {
            return;
            string vc = VC.ToString();
            string c = vc.Split(' ')[1];
            string v = vc.Split(' ')[0];
            CountObj obj = GetCounterByValue(v, c, DateTime.Now);
            obj.Count++;
            obj.DT = DateTime.Now;

        }

        public static void AddCounterThread(string vc)
        {
            //Thread th = new Thread(new ParameterizedThreadStart(AddCounter));
            //th.Start(vc);
        }

        public static void SaveData()
        {
            while (true)
            {
                Thread.Sleep(1000 * 30);
                if (!MustWhile) continue;
                MustWhile = false;
                CountObj[] cobj=new CountObj[CounterList.Count];
                lock (CounterList)
                {
                    CounterList.CopyTo(cobj);
                }
                if (cobj == null) return;
                if (cobj.Length > 0)
                {
                    int count = 0;
                    DateTime nowdt = DateTime.Now;
                    for (int i = 0; i < cobj.Length; i++)
                    {
                        if (((TimeSpan)(nowdt - cobj[i].DT)).TotalDays <= 60)
                            count++;
                    }
                    string[] sarry = new string[count];
                    count = 0;
                    for (int i = 0; i < cobj.Length; i++)
                    {
                        if (((TimeSpan)(nowdt - cobj[i].DT)).TotalDays > 60) continue;
                        sarry[count] = cobj[i].Value + " " + cobj[i].Code + " " + cobj[i].Count + " " + cobj[i].DT.ToString();
                        count++;
                    }

                    File.WriteAllLines(InputHelp.appPath + "\\Counter\\Counter.txt", sarry, Encoding.Unicode);
                }
            }
        }

    }

    /// <summary>
    /// 计数器
    /// </summary>
    public class CountObj
    {
        string _c = "";
        string _v = "";
        int _count = 0;
        DateTime _dt = DateTime.Now;
        public string Code
        {
            set { _c = value; }
            get { return _c; }
        }
        public string Value
        {
            set { _v = value; }
            get { return _v; }
        }
        public int Count
        {
            set { _count = value; }
            get { return _count; }
        }
        public DateTime DT
        {
            set { _dt = value; }
            get { return _dt; }
        }
    }
}
