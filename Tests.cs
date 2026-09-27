using System;
using System.IO;
using System.Linq;
using UnityLogFinder;
class Tests {
 static int count;
 static void Check(bool ok,string name){if(!ok)throw new Exception(name);Console.WriteLine("PASS: "+name);count++;}
 static void Write(string root,string company,string game,string file){var folder=Path.Combine(root,company,game);Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,file),"Test log\nException: test");}
 static void Main(){var root=Path.Combine(Path.GetTempPath(),"UnityLogFinder-test-"+Guid.NewGuid());Directory.CreateDirectory(root);try{
  Check(Scanner.Scan(Path.Combine(root,"missing"),true).Logs.Count==0,"Missing root");
  Write(root,"Alexey Roenko","Game A","Player.log");Write(root,"Alexey Roenko","Game A","Player-prev.log");Write(root,"Roenko Games","Гра Б","Player-prev.log");Write(root,"Other","Game C","Player.log");Write(root,"Alexey Roenko Fake","Game D","Player.log");Write(root,"Other","Game E","save.dat");
  var preferred=Scanner.Scan(root,true);Check(preferred.Logs.Count==3,"Both developers, both log types, previous-only game");Check(preferred.Logs.All(x=>x.Developer=="Alexey Roenko"||x.Developer=="Roenko Games"),"Exact developer names");
  Check(Scanner.Scan(root,false).Logs.Count==5,"Unfiltered logs exclude unrelated files");
  Write(root,"alexey roenko","Game F","Player.log");Check(Scanner.Scan(root,true).Logs.Count==4,"Case-insensitive matching");
  string source=preferred.Logs[0].Path;File.SetLastWriteTime(source,DateTime.Now.AddDays(1));Check(Scanner.Scan(root,true).Logs[0].Path==source,"Newest first");
  var destination=Path.Combine(root,"export.log");using(var writer=new FileStream(source,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite)){Scanner.Export(source,destination);Check(File.ReadAllText(destination)=="Test log\nException: test","Export while game holds log open");}
  bool rejected=false;try{Scanner.Export(source,source);}catch(IOException){rejected=true;}Check(rejected,"Reject overwrite of source");
  Console.WriteLine(count+" tests passed.");
 }finally{Directory.Delete(root,true);}}
}

