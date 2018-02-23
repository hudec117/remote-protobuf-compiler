# Remote Protobuf Compiler

A server/client duo allowing you to compile your local [Protocol Buffer](https://developers.google.com/protocol-buffers) files on a remote host (e.g. Raspberry Pi, virtual server, etc)

**NOTE:** Neither the server or client were built with extensibility, maintainability or readability in mind. This is merely a tool to aid the development of another one of my projects.

If you encounter any bugs or would like to see a feature added then please open an issue.

## Download

To download the server or client see [here](https://github.com/hudec117/remote-protobuf-compiler/releases).

## Client

### Dependencies

* Windows 10 or better (probably compatible with lower versions but not tested)
* .NET 4.6 or better

### Installation

To install the client:

1. Download the client release.
2. Extract contents of the archive to preferred location (E.g. C:\Program Files(x86)\\)
3. Optionally: Right click > Send To > Desktop to create a shortcut on your desktop.

## Server

### Dependencies

* Linux environment (tested on Raspbian and Ubuntu)
* [Google's Protocol Buffer](https://github.com/google/protobuf) compiler installed. (tested on 3.5.1)
* [tofrodos](https://launchpad.net/ubuntu/+source/tofrodos) package installed (`sudo apt install tofrodos`)
* The release has a self-contained ASP.NET Core 2.0 runtime so you don't need to install the runtime separately.

### Installation

To install the server:

1. Download the server release.
2. Transfer the archive to your target machine if necessary.
3. Extract the contents of the archive to preferred location (E.g. /opt/)

### Launching

To launch the server:

1. Navigate to your install
2. Execute: `./RemoteProtobufCompilerServer --urls http://HOST:PORT &`
   
   Adjust `HOST` and `PORT` to your configuration.
   
   Note: If you execute this via SSH, when you logout it will stop.

I have found that the simplest (not the best) way to launch the server on boot is:

1. Add the following to your `/etc/rc.local` file.

   `/INSTALL_PATH/RemoteProtobufCompilerServer/RemoteProtobufCompilerServer --urls http://HOST:PORT &`

   Adjust `INSTALL_PATH`, `HOST` and `PORT` to your configuration.

### Development

The server was developed in a Linux environment using Visual Studio code and is therefore only compatible with a Linux environment. If you would like to see the server become compatible with Windows please open an issue.

## Built With

* [Newtonsoft.Json](https://www.newtonsoft.com/json) - On the client to serialise and deserialise locally saved settings.
* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) - On the server to create a web API.
