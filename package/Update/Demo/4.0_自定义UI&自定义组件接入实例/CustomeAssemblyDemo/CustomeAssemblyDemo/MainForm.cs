/*
 * Created by SharpDevelop.
 * User: iFish
 * Date: 2015/5/7
 * Time: 22:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FSLib.App.SimpleUpdater;

namespace CustomeAssemblyDemo
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : FSLib.App.SimpleUpdater.Dialogs.AbstractUpdateBase
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		protected override void OnPackageDownload(FSLib.App.SimpleUpdater.PackageEventArgs e)
		{
			base.OnPackageDownload(e);
			
			label1.Text="正在下载 "+e.Package.PackageName+" ...";
		}
		
		protected override void OnUpdateFinished()
		{
			base.OnUpdateFinished();
			
			label1.Text="更新成功！";
		}
		
		protected override void OnInstallUpdate()
		{
			base.OnInstallUpdate();
			label1.Text="正在安装更新...";
		}
		
		protected override void OnRequestCloseApplication(FSLib.App.SimpleUpdater.QueryCloseApplicationEventArgs e)
		{
			label1.Text="正在关闭程序..";
			
			//取消屏蔽下面这行，则会进行默认的处理（询问或自动关闭，依据更新信息的要求来）。如果屏蔽，则必须自己处理进程，并设置 e.IsCancelled 的值。
			//base.OnRequestCloseApplication(e);
			
			foreach (var p in e.Processes) {
				p.Kill();
			}
			//如果需要自定义处理，那么处理完成后一定要赋予一个值，否则会导致更新库对此处的值保持等待。
			//设置为false表示正常结束。返回true则表明出现了问题，要求取消更新
			e.IsCancelled=false;
		}
		
		protected override void OnRemoveFiles()
		{
			label1.Text="正在移除旧文件...";
			base.OnRemoveFiles();
		}
		protected override void OnRemoveFile(InstallFileEventArgs e)
		{
			label1.Text="正在删除 "+e.Source;
			base.OnRemoveFile(e);
		}
		
		protected override void OnInstallFile(InstallFileEventArgs e)
		{
			base.OnInstallFile(e);
			label1.Text="正在安装 "+e.Source;
		}
		
		protected override void OnDecompressPackage(FSLib.App.SimpleUpdater.PackageEventArgs e)
		{
			//Package有可能为null因为这个事件比较特殊，在所有的包开始解压缩之前会提前触发一次用作提醒，此时是没有包的。
			if(e.Package==null)
				return;
			
			label1.Text="正在解压缩 "+(e.Package.PackageName??"");
			base.OnDecompressPackage(e);
		}
		
		protected override void OnError(Exception ex)
		{
			label1.Text="ERROR:"+ex.ToString();
			base.OnError(ex);
		}

	}
}
