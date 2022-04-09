using System;
using System.IO;

static class SetupAPI {
    public static void SetupNFy() {
        bool clean = true; // clean setup
        Console.WriteLine("Setting up NFy...");
        Console.WriteLine("Checking for songs/");

        if (!Directory.Exists("songs")) {
            Console.WriteLine("Creating directory `songs' to fix any undefined behaviour.");
            clean = false;
        }

        if (!Directory.Exists("playlists")) {
            Console.WriteLine("Creating directory `playlists' to fix any undefined behaviour.");
            clean = false;
        }

        if (clean) {
            Console.WriteLine("Setup completed with no errors");
        }
    }
}