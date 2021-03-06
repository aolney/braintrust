(*Copyright 2016 Andrew M. Olney

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*)

#r "../node_modules/fable-core/Fable.Core.dll"
#load "../node_modules/fable-import-electron/Fable.Import.Electron.fs"

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Electron

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mutable mainWindow: BrowserWindow option = Option.None

let createMainWindow () =
    let options = createEmpty<BrowserWindowOptions>
    options.width <- Some 1920.
    options.height <- Some 1080.
    
    //we need this to make grid work
    let webPreferences = createEmpty<WebPreferences>
    webPreferences.experimentalFeatures <- Some true
    options.webPreferences <- Some webPreferences
 
    let window = electron.BrowserWindow.Create(options)

    // Load the index.html of the app.
    window.loadURL("file://" + Node.__dirname + "/../index.html");

    #if DEBUG
    fs.watch(Node.__dirname + "/renderer.js", fun _ ->
        window.webContents.reloadIgnoringCache() |> ignore
    ) |> ignore
    #endif

    // Emitted when the window is closed.
    window.on("closed", unbox(fun () ->
        // Dereference the window object, usually you would store windows
        // in an array if your app supports multi windows, this is the time
        // when you should delete the corresponding element.
        mainWindow <- Option.None
    )) |> ignore

    mainWindow <- Some window

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
electron.app.on("ready", unbox createMainWindow)

// Quit when all windows are closed.
electron.app.on("window-all-closed", unbox(fun () ->
    // On OS X it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if Node.``process``.platform <> "darwin" then
        electron.app.quit()
))

electron.app.on("activate", unbox(fun () ->
    // On OS X it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if mainWindow.IsNone then
        createMainWindow()
))
