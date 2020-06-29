msbuild CannedBytes.Midi.sln /t:Clean /p:Configuration=Release
msbuild CannedBytes.Midi.sln /t:Build /p:Configuration=Release

nuget push CannedBytes.Midi\bin\Release\CannedBytes.Midi.2.0.2.nupkg -src https://api.nuget.org/v3/index.json
nuget push CannedBytes.Midi.Components\bin\Release\CannedBytes.Midi.Components.2.0.2.nupkg -src https://api.nuget.org/v3/index.json
nuget push CannedBytes.Midi.IO\bin\Release\CannedBytes.Midi.IO.2.0.2.nupkg -src https://api.nuget.org/v3/index.json
nuget push CannedBytes.Midi.Message\bin\Release\CannedBytes.Midi.Message.2.0.2.nupkg -src https://api.nuget.org/v3/index.json
nuget push CannedBytes.Midi.Xml\bin\Release\CannedBytes.Midi.Xml.2.0.2.nupkg -src https://api.nuget.org/v3/index.json
