# main-chat
Implementation of a FiveM server chat entirely in C#.

FiveM is a multiplayer framework for Grand Theft Auto 5.

# Goal of this repository
The goal of this repository is to show some basics of C# development in the FiveM framework. I've had hard time finding some documentation on a few subjects when it comes to C# and I thought this project covers some important basics.

This project is supposed to be out-of-the-box. Meaning it will support messages with the default HTML/JS interface and the default chatMessage event handler arguments.

This repository implements two important NUI functions - SendNUIMessage and RegisterNUICallback. RegisterNUICallback is a tricky function to implement as it uses ExpandoObject (unlike the lua function that uses dynamics).

# What this repository doesn't include (yet)
This is not a complete rewrite, it lacks the functions of template and suggestion adding. It is currently basic (simple chatMessage events).

# Compilation instruction
The VS solution includes two projects for the two DLLs (MainChatClient and MainChatServer). Go to the projects' properties and make sure to change the post-build events in the "Build Events" tab so the DLLs are copied to the correct path (main resource folder) upon build.

Also, each project refers to a different version of CitizenFX.Core.dll - make sure to reference those correctly.
I've referenced the client's CitizenFX dll at (..\FiveM Application Data\citizen\clr2\lib\mono\4.5\CitizenFX.Core.dll)
and the server's at (..\FXServer\citizen\clr2\lib\mono\4.5\CitizenFX.Core.dll)

There may also be references to Newtonsoft.Json. There are redundant currently.
