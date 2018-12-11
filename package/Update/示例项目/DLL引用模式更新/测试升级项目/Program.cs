using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace 测试升级项目
{
	class Program
	{
		static void Main(string[] args)
		{
			WL("测试升级项目");
			Console.WriteLine();
			WL("正在恢复老版本程序目录，准备进行更新");

			//生成临时的批处理文件，以便于调用批处理恢复旧的文件，便于演示
			var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var root = System.IO.Path.GetDirectoryName(currentAssembly);
			var cmdPath = System.IO.Path.Combine(root, "init.bat");
			System.IO.File.WriteAllText(cmdPath, Properties.Resources.Initialize, System.Text.Encoding.Default);
			//执行批处理进行恢复
			System.Diagnostics.Process.Start("cmd.exe", "/c \"" + cmdPath + "\"").WaitForExit();

			//准备更新
			var updateSrc = CombilePath(root, @"..\..\..\..\示例升级包\{0}");
			var updateInfoFile = "update_c.xml";
			var logFile = "testappexe.log";

			WL("");
			WL("开始检查更新。");
			WL("更新地址：{0}", updateSrc);
			WL("更新文件：{0}", updateInfoFile);
			WL("应用程序目录：{0}", root);
			WL("更新日志：{0}", logFile);
			WL("");
			WL("警告！升级包中并不包含此项目编译的可执行程序，因此升级完毕后，此程序会被删除。要重新测试更新，请重新编译项目！");
			WL("按任意键继续....");
			Console.ReadKey();
			WL("开始检测更新，请稍等....");

			var updater = FSLib.App.SimpleUpdater.Updater.Instance;
			updater.Context.LogFile = logFile;
			FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple(updateSrc, updateInfoFile);

			WL("");
			Console.ReadKey();
		}

		static void WL(string msg, params object[] args)
		{
			Console.WriteLine(msg, args);
		}

		static string CombilePath(string first, string second)
		{
			if (string.IsNullOrEmpty(second)) return first;

			var pathNow = first;
			if (second.StartsWith("\\"))
			{
				if (first[1] != ':')
				{
					//如果第一个路径不是绝对路径，但是第二个路径却是 \ 开始的，那么直接返回第二个路径
					pathNow = second;
				}
				else
				{
					//第二个是绝对路径
					pathNow = string.Format("{0}:{1}", first[0], second);
				}
			}
			else
			{
				pathNow = string.Concat(first, first.EndsWith("\\") ? "" : "\\", second);
			}
			Console.WriteLine(pathNow);
			//如果没有向上返回等字符，那么直接返回。
			if (pathNow.IndexOf("..") == -1) return pathNow;

			//格式化路径
			var pathArray = pathNow.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			var stack = new Stack<string>(pathArray.Length);
			foreach (var pathFragment in pathArray)
			{
				if (pathFragment == ".") continue;

				switch (pathFragment)
				{
					case "..":
						stack.Pop();
						break;
					default:
						stack.Push(pathFragment);
						break;
				}
			}
			pathArray = stack.ToArray();
			Array.Reverse(pathArray);
			return string.Join("\\", pathArray);
		}
	}
}
