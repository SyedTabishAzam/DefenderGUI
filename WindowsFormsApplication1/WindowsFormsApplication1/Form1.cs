using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
namespace WindowsFormsApplication1
{
    enum CONNECTION_CODE { ACK = 500, SERVER_UPDATE = 501, SCENERIO_UPDATE = 502, EXIT = 503, ERROR_REC = 504 };
    public partial class DefenderGUI : Form
    {
        StreamWriter inputWriter;
        
        bool startGame = false;
        int initialCount = 0;
        int finalCount = 0;
        private List<Entity> entlist = new List<Entity>();
   
        public DefenderGUI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label1.Text = "Finding Server";
            StartInitialSubscriber();
            StartCommandPublisher();
            
           
        }

        private void Kill(string proc)
        {
            string procName = "";
            if (proc == "Subscriber")
            {
                procName = "DefenderInitialSubscriber";
            }
            if(proc=="Publisher")
            {
                procName = "DefenderCommandPublisher";
            }
            foreach (var processx in Process.GetProcessesByName(procName))
            {
                try
                {

                    processx.Kill();
                }
                catch(Win32Exception e)
                {

                }
            }
        }
        private void StartInitialSubscriber()
        {
            Kill("Subscriber");

            Process process = new Process();

            process.StartInfo.FileName = "RunSub.cmd";

            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += new DataReceivedEventHandler(HandleSubscriberOutput);

            process.Start();

            process.BeginOutputReadLine();

            StaticClass.Subscriber = process;
            

        }

 

        public void HandleSubscriberOutput(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine("sTILL BEING CALLED");
            string packet = outLine.Data;
            if (packet != null)
            {



                if (packet.Contains("SERVER") && (packet.Contains((CONNECTION_CODE.ACK).ToString("d"))))
                {
                    label1.BeginInvoke(new Action(() => label1.Text = "Establishing connection"));
                    label1.BeginInvoke(new Action(() => label1.BackColor= Color.Yellow));
                    label1.BeginInvoke(new Action(() => label1.ForeColor = Color.Black));


                }

                if (packet.Contains((CONNECTION_CODE.SERVER_UPDATE).ToString("d")))
                {
                    label1.BeginInvoke(new Action(() => label1.Text = "Connected to server. Waiting for players"));
                    label1.BeginInvoke(new Action(() => label1.BackColor = Color.GreenYellow));
                    
                    HandleControl(true);

                    List<string> names = packet.Split(',').ToList<string>();
                    foreach (string x in names)
                    {
                        UpdateConnection(x);
                    }

                }
                if (packet.Contains((CONNECTION_CODE.SCENERIO_UPDATE).ToString("d")))
                {
                    //End Of Entity symbol received
                    if (!(packet.Split('?')[1].Contains("EOE>(NULL,NULL)")))
                        CheckPacket(packet.Split('?')[1]);
                    else
                        //Now populate
                        startGame = true;
                    finalCount++;
                }
                if(startGame)
                {
                    StaticClass.Subscriber.OutputDataReceived -= new DataReceivedEventHandler(HandleSubscriberOutput);
                    label1.BeginInvoke(new Action(() => label1.Text = "Game Started. Choose your entity and begin"));
                    label1.BeginInvoke(new Action(() => label1.ForeColor = Color.White));
                    label1.BeginInvoke(new Action(() => label1.BackColor = Color.Green));
                    label4.BeginInvoke(new Action(() => label4.Visible = true));
                    SetEnable(comboBox1, true);
                    SetEnable(button2, true);

                    HandleControl(false);
                    UpdateComboBox();
                   // CheckFile(packet);
                }
               // SetText(richTextBox1, outLine.Data);

            }
        }

        private void UpdateComboBox()
        {
           
            if (initialCount != finalCount)
            {
                
                refreshList(entlist);
            }
        }
        private void CheckPacket(string str)
        {
            var entitylist = str.Split('>');
            var entName = entitylist[0];
            var varNames = entitylist[1];
            
           
            Entity newEntity = new Entity();
            newEntity.name = entName;
            newEntity.id = Entity.count++;



            List<string> variabellist = varNames.Split(' ').ToList();
            List<string> varnameslst = new List<string>();
            List<string> vartypelst = new List<string>();
            foreach (var variables in variabellist)
            {
                List<string> splitstring = variables.Split(',').ToList();

                string varname = splitstring[0].Substring(1);
                string vartype = splitstring[1].TrimEnd(')');

                varnameslst.Add(varname);
                vartypelst.Add(vartype);



            }

            newEntity.SetVarnames(varnameslst);
            newEntity.SetVartypes(vartypelst);

            entlist.Add(newEntity);
            
        }

        private void StartCommandPublisher()
        {
            Kill("Publisher");

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.UseShellExecute = false; //required to redirect standart input/output

        // redirects on your choice
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;
        startInfo.FileName = "RunPub.cmd";
  

        Process process2 = new Process();
        process2.StartInfo = startInfo;
       

        process2.Start();

        process2.BeginOutputReadLine();
        StaticClass.Publisher = process2;
        
            
          
        }
        private void UpdateConnection(string str)
        {
            string type = str.Substring(0, str.LastIndexOf('='));
            string result = str.Substring(str.LastIndexOf('=') + 1);
            int count = Int16.Parse(result);
            
            if(type=="defenders")
            {
                
                string text = "Defender joined";
                SetText(listBox2, "", 1);
                for(int i=0;i<count;i++)
                {
                    SetText(listBox2, text);
                }
                
                
            }
            else if(type=="ccc")
            {
                string text = "CCC joined";
                SetText(listBox1, "", 1);
                for (int i = 0; i < count; i++)
                {
                    SetText(listBox1, text);
                }
            }
            
        }
        private void HandleControl(bool success)
        {
            SetEnable(listBox1, success);
            SetEnable(listBox2, success);
            if(success)
            {

                ChangeColor(listBox1, Color.White);
                ChangeColor(listBox2, Color.White);
           
            }
            else
            {
                ChangeColor(listBox1, Color.Gray);
                ChangeColor(listBox2, Color.Gray);
            }
        }

        private void SetEnable(Control rt, bool success)
        {
            rt.BeginInvoke(new Action(() => rt.Enabled = success));
        }

        private void ChangeColor(ListBox rt, Color col)
        {
            rt.BeginInvoke(new Action(() => rt.BackColor = col));
        }

        //mode 0 for append
        private void SetText(ListBox rt, string text,int mode=0)
        {

          
            if(mode==1)
            {
               
                rt.BeginInvoke(new Action(() =>rt.Items.Clear()));
            
            }
            else
            {
                rt.BeginInvoke(new Action(() => rt.Items.Add(text)));
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            StaticClass.SelectedEntity = (Entity) comboBox1.SelectedItem;
            

            Form3 form = new Form3();
            
            Hide();
            form.ShowDialog();
            Close();
            
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (e.CloseReason == CloseReason.UserClosing)
            {
                ClearEverything();
                Application.Exit();
            }

        }

        private void ClearEverything()
        {
            Kill("Publisher");
            Kill("Subscriber");
        }

        public void refreshList(List<Entity> list)
        {
            comboBox1.BeginInvoke(new Action(() => comboBox1.DataSource = new List<Entity>()));

            comboBox1.BeginInvoke(new Action(() => comboBox1.DataSource = list));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //
        }

        private void DefenderGUI_Load(object sender, EventArgs e)
        {
            FormClosing += Form1_FormClosing;
            this.comboBox1.DataSource = entlist;
            
            this.comboBox1.DisplayMember = "name";
            this.comboBox1.ValueMember = "id";
        }
    }

    public class Entity
    {

  
        public static int count = 0;
      
        private List<string> varnames;
        private List<string> vartypes;

        public string name { get; set; }
        public int id { get; set; }

        public List<string> GetVarnames()
        {
            return varnames;
        }

        public List<string> GetVartypes()
        {
            return vartypes;
        }

        public void SetVarnames(List<string> lst)
        {
            varnames = lst;
        }

        public void SetVartypes(List<string> lst)
        {
            vartypes = lst;
        }
    }

   
}
