using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace UnityLogFinder {
 public class LogEntry {
  public string Game {get;set;} public string Developer {get;set;} public string FileName {get;set;}
  public DateTime Modified {get;set;} public long Bytes {get;set;} public string Path {get;set;}
 }
 public class ScanResult {public List<LogEntry> Logs=new List<LogEntry>(); public int Skipped;}
 public static class Scanner {
  static string[] Dirs(string path,ScanResult result) {try{return Directory.GetDirectories(path);}catch(IOException){result.Skipped++;}catch(UnauthorizedAccessException){result.Skipped++;}return new string[0];}
  static bool Link(string path) {return (File.GetAttributes(path)&FileAttributes.ReparsePoint)!=0;}
  public static ScanResult Scan(string root,bool preferred) {
   var result=new ScanResult(); if(!Directory.Exists(root))return result;
   foreach(var company in Dirs(root,result)) {
    var developer=System.IO.Path.GetFileName(company);
    if(preferred && !developer.Equals("Alexey Roenko",StringComparison.OrdinalIgnoreCase) && !developer.Equals("Roenko Games",StringComparison.OrdinalIgnoreCase))continue;
    try {
     if(Link(company))continue;
     foreach(var game in Dirs(company,result)) {
      try {
       if(Link(game))continue;
       foreach(var name in new[]{"Player.log","Player-prev.log"}) {
        var path=System.IO.Path.Combine(game,name); if(!File.Exists(path)||Link(path))continue;
        var info=new FileInfo(path);result.Logs.Add(new LogEntry{Game=System.IO.Path.GetFileName(game),Developer=developer,FileName=name,Modified=info.LastWriteTime,Bytes=info.Length,Path=path});
       }
      }catch(IOException){result.Skipped++;}catch(UnauthorizedAccessException){result.Skipped++;}
     }
    }catch(IOException){result.Skipped++;}catch(UnauthorizedAccessException){result.Skipped++;}
   }
   result.Logs=result.Logs.OrderByDescending(x=>x.Modified).ThenBy(x=>x.Game).ToList();return result;
  }
  public static void Export(string source,string destination) {
   if(string.Equals(System.IO.Path.GetFullPath(source),System.IO.Path.GetFullPath(destination),StringComparison.OrdinalIgnoreCase))throw new IOException("Choose a destination different from the original log.");
   using(var input=new FileStream(source,FileMode.Open,FileAccess.Read,FileShare.ReadWrite|FileShare.Delete))
   using(var output=new FileStream(destination,FileMode.Create,FileAccess.Write))input.CopyTo(output);
  }
 }
}
