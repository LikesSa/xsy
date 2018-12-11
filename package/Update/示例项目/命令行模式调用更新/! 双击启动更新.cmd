@ECHO OFF

cd /d %~dp0
cd ..
ECHO 当前目录：%CD%
ECHO.
ECHO 正在清理之前更新后的文件（如果存在）
CD 命令行模式调用更新
DEL *.txt /f /s /q
DEL 安装*执行.cmd /f /s /q
ECHO.
ECHO.
CD ..

ECHO 正在从【示例老版本程序目录】复制老版本的程序到当前目录【命令行模式调用更新】便于演示
ECHO.
XCOPY 示例老版本程序目录\* 命令行模式调用更新\ /e /y
ECHO.
ECHO.
ECHO 当前更新配置：
ECHO.
ECHO 更新地址：	%CD%\示例升级包\{0}
ECHO 信息文件：	update_c.xml
ECHO 当前版本：	1.0.0.0
ECHO 当前应用程序目录（相对）：	(空)
ECHO 日志文件：	testapp.log
ECHO 隐藏检查更新界面：	(是)
ECHO 命令行：	/startupdate /cv "1.0.0.0" /url "%CD%\示例升级包\{0}" /infofile "update_c.xml" /log "testapp.log" 
ECHO.

ECHO 正在启动进程
命令行模式调用更新\SimpleUpdater.exe /startupdate /cv "1.0.0.0" /url "%CD%\示例升级包\{0}" /infofile "update_c.xml" /log "testapp.log" /hideCheckUI

PAUSE

