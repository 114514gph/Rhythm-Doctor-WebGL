@echo off
chcp 65001 >nul
echo ========================================
echo   Rhythm Doctor WebGL 本地服务器
echo ========================================
echo.
echo  正在启动服务器...
echo  浏览器将自动打开 http://localhost:8080/
echo.
echo  关闭此窗口即可停止服务器
echo.
powershell -ExecutionPolicy Bypass -File "%~dp0start-server.ps1"
pause
