/*
 * Created by SharpDevelop.
 * User: iFish
 * Date: 2015/5/7
 * Time: 22:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CustomeAssemblyDemo
{
	/// <summary>
	/// Description of CustomConnect.
	/// </summary>
	public class CustomConnect:FSLib.App.SimpleUpdater.Defination.IUpdateNotify
	{
		public CustomConnect()
		{
		}

		#region IUpdateNotify implementation

		public void Init(FSLib.App.SimpleUpdater.Updater updater)
		{
			System.Windows.Forms.MessageBox.Show("已连接。当前更新程序的临时路径为："+System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
		}

		#endregion
	}
}
