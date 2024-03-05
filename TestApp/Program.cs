using MageekCore;
using MageekCore.Data;
using MageekCore.Data.Collection;
using MageekCore.Data.Mtg;

Console.WriteLine("MAGEEK TESTS");
Console.WriteLine("Init...");
MageekService mageek = new MageekService();
var v = await mageek.Initialize();
if (v != MageekInitReturn.Error)
{ 
    Console.WriteLine("Done");
    Console.WriteLine("");

    Console.WriteLine("Test precos");

    //await mageek.RetrievePrecos();
    //await mageek.ParsePrecos(Path.Combine(Folders.PrecosFolder, "temp"), Folders.PrecosFolder);

    Console.WriteLine("Done");

    Console.WriteLine("");
    Console.WriteLine("- Press any key to exit -");
    Console.ReadKey();
}
