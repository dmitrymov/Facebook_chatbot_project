IF EXIST lib\net40 GOTO pack

mkdir lib\net40

:pack
copy ..\bin\Net40\Release\FacebookChat.dll lib\net40\
nuget.exe pack Package.nuspec
nuget.exe push -Source http://packages.nuget.org/v1/ Facebook-Chat.1.1.nupkg 0aa04c9f-1186-4f20-9ccf-074580e0d9c0