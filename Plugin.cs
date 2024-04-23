﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

public static class Patcher
{
    public static BepInEx.Logging.ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("WriteIniPatch");
    // List of assemblies to patch
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    public static void log(string text)
    {
        logger.LogInfo(text);
    }

    // Patches the assemblies
    public static void Patch(AssemblyDefinition assembly)
    {
        log("Loading assembly module...");
        var mainModule = assembly.MainModule;
        TypeDefinition songEntry = null;
        foreach (TypeDefinition type in mainModule.Types)
        {
            if (type.Name == "SongEntry")
            {
                songEntry = type;
            }
        }

        if (songEntry == null) {
            log("Failed to get SongEntry");
            return;
        }
        log("Loaded SongEntry type.");

        MethodDefinition writeIni = null;
        foreach (MethodDefinition method in songEntry.Methods)
        {
            if (method.Name == "ʷʲʿʾʷʿʽʽʷʴʷ")
            {
                writeIni = method;
            }
        }

        if (writeIni == null) {
            log("Failed to get writeIni method.");
            return;
        } 
        log("Loaded writeIni method.");
        
        var processor = writeIni.Body.GetILProcessor();


        var nop = processor.Create(OpCodes.Nop);

        var codes = writeIni.Body.Instructions;

        var start = 0x2a2;
        var end = 0x2c0;

        var count = end - start;

        Instruction prev = null;

        for (int i = 0; i < writeIni.Body.Instructions.Count; i++) {
            if (codes[i].Offset >= start && codes[i].Offset <= end) {
              prev = codes[i].Previous;
              break;
            }
        }

        for (int i = 0; i < count; i++) {
            processor.Remove(prev.Next);
        }

        log("Instructions replaced.");

        log(assembly.FullName);
    }
}
