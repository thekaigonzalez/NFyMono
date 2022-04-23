import std.stdio;
import std.string;
import std.file;

void main()
{
	write("Project name? ");

	string name = readln().strip;

	mkdir(name);

	File f = File(name ~ "/" ~ name ~ ".js", "w");

	f.write("// please refer to the generated INSTALL.md to learn how to add your plugin to NFy Mono.\nfunction onNMonoEngineStart(env) {\n\t// NJPlaySong(\"My Favorite Song\"); will play a song.\n\tNJPrint(\"hello, world!\");\n}");

	f.close();

	File b = File(name ~ "/INSTALL.md", "w");

	b.write("# How to install your plugin\n\nThis directory contains THE_NAME_OF_YOUR_PLUGIN.js, what you need to do is as follows:\n\nCopy PLUGIN_NAME.js into your NFy Mono installation directory's plugins.\n\nThen, you need to run NFy Mono with the --`specials` directive. After, you should see 'hello, world' at the top of the console!\n");

	b.close();

}
