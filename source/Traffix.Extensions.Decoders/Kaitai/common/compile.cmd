REM Compile Kaitai specification to .NET Core source code

kaitai-struct-compiler -t csharp --dotnet-namespace Traffix.Extensions.Decoders.Common -d ../../Common ./*.ksy