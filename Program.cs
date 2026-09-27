using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
[assembly: AssemblyTitle("UnityLogFinder")]
[assembly: AssemblyVersion("1.0.0.0")]
namespace UnityLogFinder {
 static class Program {
  [STAThread] static void Main() {Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new MainForm());}
 }
 public class MainForm:Form {
  readonly CheckBox preferred=new CheckBox{Text="Only Alexey Roenko and Roenko Games",Checked=true,AutoSize=true,Margin=new Padding(0,8,16,0)};
  readonly TextBox search=new TextBox{Width=180};
  readonly Button refresh=new Button{Text="Refresh",AutoSize=true};
  readonly DataGridView grid=new DataGridView{Dock=DockStyle.Fill,ReadOnly=true,AllowUserToAddRows=false,AllowUserToDeleteRows=false,AutoGenerateColumns=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,MultiSelect=false,RowHeadersVisible=false,BackgroundColor=Color.White,BorderStyle=BorderStyle.None,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
  readonly Label status=new Label{Dock=DockStyle.Fill,AutoEllipsis=true,TextAlign=ContentAlignment.MiddleLeft};
  readonly Label path=new Label{Dock=DockStyle.Fill,AutoEllipsis=true,TextAlign=ContentAlignment.MiddleLeft};
  readonly List<Button> actions=new List<Button>();
  readonly string root=System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"AppData","LocalLow");
  List<LogEntry> logs=new List<LogEntry>();int skipped;bool busy;
  public MainForm() {
   Text="UnityLogFinder";Size=new Size(1100,640);MinimumSize=new Size(900,450);StartPosition=FormStartPosition.CenterScreen;Font=new Font("Segoe UI",10);BackColor=Color.White;AutoScaleMode=AutoScaleMode.Dpi;
   var layout=new TableLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(24),ColumnCount=1,RowCount=7};
   foreach(int height in new[]{48,32,48})layout.RowStyles.Add(new RowStyle(SizeType.Absolute,height));
   layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));foreach(int height in new[]{36,46,40})layout.RowStyles.Add(new RowStyle(SizeType.Absolute,height));
   layout.Controls.Add(new Label{Text="UnityLogFinder",Font=new Font("Segoe UI",23,FontStyle.Bold),AutoSize=true},0,0);
   layout.Controls.Add(new Label{Text="Find game logs. Open a file or save a copy for support.",AutoSize=true,ForeColor=Color.DimGray},0,1);
   var filters=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false};filters.Controls.Add(preferred);filters.Controls.Add(new Label{Text="Search:",AutoSize=true,Margin=new Padding(0,8,6,0)});filters.Controls.Add(search);filters.Controls.Add(refresh);layout.Controls.Add(filters,0,2);
   AddColumn("Game","Game",22);AddColumn("Developer","Developer",20);AddColumn("Log file","FileName",18);AddColumn("Modified","Modified",25);AddColumn("Bytes","Bytes",12);
   grid.Columns[3].DefaultCellStyle.Format="yyyy-MM-dd HH:mm:ss";grid.Columns[4].DefaultCellStyle.Format="N0";grid.RowTemplate.Height=30;grid.ColumnHeadersHeight=36;grid.EnableHeadersVisualStyles=false;grid.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(239,243,249);grid.DefaultCellStyle.SelectionBackColor=Color.FromArgb(218,232,252);grid.DefaultCellStyle.SelectionForeColor=Color.Black;
   layout.Controls.Add(grid,0,3);layout.Controls.Add(path,0,4);
   var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill};AddAction(buttons,"Open log",OpenLog);AddAction(buttons,"Open folder",()=>Process.Start("explorer.exe","/select,\""+Selected.Path+"\""));AddAction(buttons,"Copy log path",()=>{Clipboard.SetText(Selected.Path);status.Text="Log path copied.";});AddAction(buttons,"Save log as...",SaveLog);
   layout.Controls.Add(buttons,0,5);layout.Controls.Add(status,0,6);Controls.Add(layout);
   refresh.Click+=async(s,e)=>await Reload();preferred.CheckedChanged+=async(s,e)=>await Reload();search.TextChanged+=(s,e)=>ApplyFilter();grid.SelectionChanged+=(s,e)=>UpdateSelection();grid.CellDoubleClick+=(s,e)=>{if(e.RowIndex>=0)Run(OpenLog);};Shown+=async(s,e)=>await Reload();
  }
  LogEntry Selected {get{return grid.CurrentRow==null?null:grid.CurrentRow.DataBoundItem as LogEntry;}}
  void OpenLog(){Process.Start(new ProcessStartInfo(Selected.Path){UseShellExecute=true});}
  void AddColumn(string title,string property,int weight){grid.Columns.Add(new DataGridViewTextBoxColumn{HeaderText=title,DataPropertyName=property,FillWeight=weight,SortMode=DataGridViewColumnSortMode.NotSortable});}
  void AddAction(FlowLayoutPanel panel,string title,Action action){var button=new Button{Text=title,AutoSize=true,Height=34,Margin=new Padding(0,4,10,0)};button.Click+=(s,e)=>Run(action);actions.Add(button);panel.Controls.Add(button);}
  void Run(Action action){if(Selected==null)return;try{action();}catch(Exception ex){MessageBox.Show(this,ex.Message,"Unable to access log",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
  void SaveLog(){var item=Selected;using(var dialog=new SaveFileDialog{Filter="Log files (*.log)|*.log|All files (*.*)|*.*",FileName=item.Game+"-"+item.FileName,OverwritePrompt=true}){if(dialog.ShowDialog(this)==DialogResult.OK){Scanner.Export(item.Path,dialog.FileName);status.Text="Saved: "+dialog.FileName;}}}
  async Task Reload(){if(busy)return;busy=true;refresh.Enabled=false;preferred.Enabled=false;status.Text="Scanning game logs...";try{bool only=preferred.Checked;var result=await Task.Run(()=>Scanner.Scan(root,only));if(IsDisposed)return;logs=result.Logs;skipped=result.Skipped;ApplyFilter();}catch(Exception ex){if(!IsDisposed)status.Text="Scan failed: "+ex.Message;}finally{busy=false;if(!IsDisposed){refresh.Enabled=true;preferred.Enabled=true;}}}
  void ApplyFilter(){string query=search.Text.Trim();var visible=logs.Where(x=>(x.Game+" "+x.Developer+" "+x.FileName).IndexOf(query,StringComparison.OrdinalIgnoreCase)>=0).ToList();grid.DataSource=visible;status.Text=visible.Count==0?"No logs found. Clear search, turn off the developer filter, or launch the game first.":visible.Count+" log file(s) - newest first";if(skipped>0)status.Text+=" - unreadable locations skipped: "+skipped;UpdateSelection();}
  void UpdateSelection(){var item=Selected;path.Text=item==null?root:item.Path;foreach(var button in actions)button.Enabled=item!=null;}
 }
}
