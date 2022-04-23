using Godot;
using System;

static class JAPI {
    public static void JCreateDir(string dname) {
        System.IO.Directory.CreateDirectory(dname);
    }
    public static void JWriteFile(string fileName, string text) {
        System.IO.File.WriteAllText(fileName, text);
    }
}