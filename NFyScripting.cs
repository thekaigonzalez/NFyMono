using Godot;
using System;

static class JAPI {
    public static void JCreateDir(string dname) {
        System.IO.Directory.CreateDirectory(dname);
    }
}