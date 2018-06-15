@echo off

SET msbuild="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
SET current=%cd%

setlocal EnableDelayedExpansion

SET proj_1="%current%\mssql\Zongsoft.Data.MsSql.sln"
SET proj_2="%current%\mysql\Zongsoft.Data.MySql.sln"
SET proj_3="%current%\oracle\Zongsoft.Data.Oracle.sln"
SET proj_4="%current%\postgresql\Zongsoft.Data.PostgreSql.sln"


for /L %%i in (1,1,4) do (
	if exist !proj_%%i! (
		@echo [%%i] !proj_%%i!
		%msbuild% !proj_%%i! /t:rebuild /clp:ErrorsOnly,PerformanceSummary,NoSummary /v:minimal

		REM if errorlevel 1 @echo The !proj_%%i! file compile ERROR!!!  & pause > null

		if errorlevel 1 (
			choice /T 30 /D y /C ny /M "!proj_%%i! 项目编译出错，是否要退出？"

			if errorlevel 2 goto EXIT
		)
	) else @echo [%%i] The '!proj_%%i!' file is not exists.
)

:EXIT
