# Welcome to NFy!

This will go over everything you need to know to begin listening with NFy.

## 1 - Directory

The current directory requires these folders to function correctly, 

- *playlists*
- *songs*

Usually, if not found, they'll be created for you with the help of the [SetupAPI](../reference/NFy/SetupAPI/index.md). But,
NFyMono comes with songs built-in, songs that i've personally converted, and maybe contributors have converted. See [Conversion](#2---conversion)

## 2 - Conversion

Every song needs to be converted to `.ogg` or `.wav` (preferrably `.ogg`),

luckily, the original NFy came with open source software for that specific task.

- `mp3oggd.py` - compile each file in `songs_compile` directory to .ogg from mp3 (or other)
- `mp32ogg.py` - compile a file to ogg. from mp3 (or other)

## 3 - Rotations

Rotations are song arrays that play in order after each other.

You can create rotations using JSON format, simply thanks to the [RotationAPI](../reference/NFy/RotationAPI/index.md)

usually a rotation looks like

```json

{
    "songs": [
        "Awesome song 1",
        "Awesome song 2"
    ]
}

```

And in NFy mono 3 (soon **4**), it shows how far you are into the rotation.

**(ROTATIONS ARE STILL IN EARLY ALPHA AND CONTAIN BUGS)**

## 4 - Running Executable

The executable requires the njmono.pck file to be in the same directory as the njmono.exe, it contains necessary things for NFy Mono to run.

As well as the discord DLLs, can't forget those!

## 5 - Developers

### Useful NFy Mono Development Resources

[NFy vs NFy Mono API](../api.md)

[NFy Mono APIs](../reference/index.md)

## 6 - News

### How it works

The news file format is as shown, `month-day-year.json`

It contains `body`, `author` and `title`, but with recent updates, `title` is becoming obsolete for official NFy Clients.
it will remain in support for any unofficial NFy Client.

And they're usually converted from [NewsScript](#newsscript-markup-language), the standard markup language.

this allows the program to download the daily news without hassle.

### NewsScript (Markup Language)

NewsScript (NewScript) is the general purpose markup language used in NFy & NFy Mono, designed by Kai Gonzalez to be a 
simplistic language to convert to API JavaScript Object Notation, it has been acquired by the RCBash programming language toolchain,
and is now closed-source.

NewsScript only has a few simple rules:

```
(TITLE/AUTHOR)

NEWS
```

and converts to HTML and JSON (with this edit, Markdown and JSON).

## 7 - Theme Cycling

### Available Themes

Currently, there are 2 available themes, 

- **NFy Classic** (Licensed under GNU GPL v3)
- **NFy Darker** (Licensed under MIT, like the rest of the NFyMono project.)

And there will be for a while.

## 8 - Extensions

NFy Mono contains a **GDScript Discord API** that the C# interface interoperates with.

You can view API functionality [Here](../reference/gds/index.md)

## 9 - Setting up NFy Listening

Back to setup, after loading into NFy, you can play songs using the play song button, and
stop them using the stop song button.

You can loop by ticking the `Loop?` button, and loop rotations using the `Loop Playlist?` button.

Selection menus such as `Song Names` and `Playlists` are available in the middle.

News is to the side.