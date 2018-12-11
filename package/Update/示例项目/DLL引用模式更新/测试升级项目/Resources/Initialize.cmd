@ECHO OFF

cd /d %~dp0
ECHO 正在清理之前更新后的文件（如果存在）
DEL *.txt /f /s /q
DEL 安装*执行.cmd /f /s /q
ECHO.
ECHO.

ECHO 正在从【示例老版本程序目录】复制老版本的程序到当前目录便于演示
ECHO.
XCOPY ..\..\..\..\示例老版本程序目录\* .\ /e /y
