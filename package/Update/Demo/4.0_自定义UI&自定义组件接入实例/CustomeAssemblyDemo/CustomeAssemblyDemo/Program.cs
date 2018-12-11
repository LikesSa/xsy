/*
 * Created by SharpDevelop.
 * User: iFish
 * Date: 2015/5/7
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace CustomeAssemblyDemo
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //var reg = Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\SOFTWARE\Sysinternals\Streams");
            //{
            //    Console.WriteLine(reg.Name);
            //    reg.SetValue("EulaAccepted", 1, RegistryValueKind.DWord);
            //}

            //Console.ReadKey();
            //return;

            //获得当前的更新实例
            var updater=FSLib.App.SimpleUpdater.Updater.Instance;
			
			/* 
			 * 1.注册程序集。当程序集被注册的时候，任何程序集中实现了 FSLib.App.SimpleUpdater.Defination.IUpdateNotify 接口的都将会被自动实例化并调用
			 *   通过此方法可以实现自己的事件捕捉以及处理类
			 *   此例中， 类 CustomConnect 将会被实例化并调用
			 */
			updater.UsingAssembly(System.Reflection.Assembly.GetExecutingAssembly());
			/*
			 * 2.自定义界面UI。此调用将会替换掉默认的更新界面（此例中将会把更新界面替换为 MainForm）
			 *   和上面的使用方法类似，但可实现完全自定义的效果
			 *   要用来替换的界面必须是基于 FSLib.App.SimpleUpdater.Dialogs.AbstractUpdateBase 派生的子类
			 */
			updater.UsingFormUI<MainForm>();
			
			//检测更新(此处演示网址为此控件库的更新地址)
			FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple("http://118.190.80.48/update/{0}","update_c.xml");


            Console.WriteLine("正在检测更新....");
            Console.ReadKey();
        }
		
	}
}
