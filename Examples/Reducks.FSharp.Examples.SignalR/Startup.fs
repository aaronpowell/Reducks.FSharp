module Startup

open Owin
open Microsoft.Owin
open System
open System.IO
open System.Threading.Tasks
open Microsoft.Owin.StaticFiles
open Microsoft.Owin.FileSystems
open System.Reflection
open Microsoft.AspNet.SignalR

type public Startup() =
    member x.Configuration (app:IAppBuilder) =
        let hubConfig = new HubConfiguration()
        hubConfig.EnableDetailedErrors <- true
        app.MapSignalR(hubConfig) |> ignore

        let fileSystem = new PhysicalFileSystem(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\..\..\wwwroot") :> IFileSystem
        let options = new FileServerOptions()
        options.FileSystem <- fileSystem
        app.UseFileServer options |> ignore
        app.UseDefaultFiles() |> ignore

