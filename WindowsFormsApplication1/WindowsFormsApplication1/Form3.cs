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
namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        Entity mEntity;
       enum Direction { UP=0, LEFT=1, DOWN=2, RIGHT=3,INC=4,DEC=5 };
       enum View {  vLEFT = 0,  vRIGHT = 1 };
        bool checkNow = false;
        bool alive = false;
        private Timer timer1;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            mEntity = StaticClass.SelectedEntity;
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            StaticClass.Publisher.OutputDataReceived += new DataReceivedEventHandler(AckOutputDataReceived);
            StaticClass.Subscriber.OutputDataReceived += new DataReceivedEventHandler(HandleScenerioData);
            textBox2.Text = mEntity.name;
            PopulateVariables();
        }
        void Form1_KeyDown(object sender,KeyEventArgs e)
        {
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            if(e.KeyCode == Keys.Left)
            {
                SendCommand(new string[] { time, mEntity.name, "VIEW", ((int)View.vLEFT).ToString() });
            }
            else if(e.KeyCode==Keys.Right)
            {
                SendCommand(new string[] { time, mEntity.name, "VIEW", ((int)View.vRIGHT).ToString() });
            }
        }
        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            bool smallLetter = (e.KeyChar >= 97 && e.KeyChar <= 122);
            bool capLetter = (e.KeyChar >= 65 && e.KeyChar <= 90);
            if (smallLetter || capLetter)
            {
                label9.Text = "Key pressed: " + e.KeyChar.ToString();
                DateTime src = DateTime.Now;
                string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
                if (e.KeyChar == (char)87 || e.KeyChar == (char)119)
                {//UP
                    
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.UP).ToString() });
                }
                if (e.KeyChar == (char)83 || e.KeyChar == (char)115)
                {//DOWN
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.DOWN).ToString() });
                }
                if (e.KeyChar == (char)65 || e.KeyChar == (char)97)
                {//Left
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.LEFT).ToString() });
                }
                if (e.KeyChar == (char)68 || e.KeyChar == (char)100)
                {//Right
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.RIGHT).ToString() });
                }
                if (e.KeyChar == (char)85 || e.KeyChar == (char)117)
                {//U
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.INC).ToString() });
                }
                if (e.KeyChar == (char)73 || e.KeyChar == (char)105)
                {//I
                    SendCommand(new string[] { time, mEntity.name, "MOVE", ((int)Direction.DEC).ToString() });
                }
                
            }
           
            e.Handled = true;
            
        }

        private void PopulateVariables()
        {
            List<string> varnames = mEntity.GetVarnames();
            
         
            foreach(var name in varnames)
            {
                comboBox1.Items.Add(name);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> varnames = mEntity.GetVarnames();
            List<string> vartype = mEntity.GetVartypes();
     
             String selectedVariable = comboBox1.SelectedItem as string;
             int count = 0;
            foreach (var name in varnames)
            {
                if(name.Equals(selectedVariable))
                {
                    textBox3.Text = vartype[count];
                }
                count++;
            }
        }

        private string GetFormatedTime()
        {
            string dt = "(" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ")";
            return dt;
        }

        private bool ValidateCommand(string entityname, string variable, string type, string value)
        {
            bool success = true;
            if (entityname.Equals(""))
            {
                success = success && false;
                richTextBox1.AppendText(GetFormatedTime() + Constants.Error.ENITITY_EMPTY + Environment.NewLine);
            }
            if (variable.Equals(""))
            {
                success = success && false;
                richTextBox1.AppendText(GetFormatedTime() + Constants.Error.VARIABLE_EMPTY + Environment.NewLine);
            }
            if (type.Equals(""))
            {
                success = success && false;
                richTextBox1.AppendText(GetFormatedTime() + Constants.Error.UNDEFINED_TYPE + Environment.NewLine);
            }
            if (value.Equals(""))
            {
                success = success && false;
                richTextBox1.AppendText(GetFormatedTime() + Constants.Error.VALUE_EMPTY + Environment.NewLine);
            }

            if (type.Equals(Constants.AllowedVariables.DOUBLE))
            {
                Double j;
                if (Double.TryParse(value, out j))
                {

                }
                else
                {
                    success = success && false;
                    richTextBox1.AppendText(GetFormatedTime() + Constants.Error.TYPE_MISMATCH + Environment.NewLine);
                }
            }

            if (type.Equals(Constants.AllowedVariables.INT))
            {
                int j;
                if (Int32.TryParse(value, out j))
                {

                }
                else
                {
                    success = success && false;
                    richTextBox1.AppendText(GetFormatedTime() + Constants.Error.TYPE_MISMATCH + Environment.NewLine);
                }
            }

            if (type.Equals(Constants.AllowedVariables.BOOL))
            {
                Boolean j;
                if (Boolean.TryParse(value, out j))
                {

                }
                else
                {
                    success = success && false;
                    richTextBox1.AppendText(GetFormatedTime() + Constants.Error.TYPE_MISMATCH + Environment.NewLine);
                }
            }

            if (type.Equals(Constants.AllowedVariables.FLOAT))
            {
                float j;
                if (float.TryParse(value, out j))
                {

                }
                else
                {
                    success = success && false;
                    richTextBox1.AppendText(GetFormatedTime() + Constants.Error.TYPE_MISMATCH + Environment.NewLine);
                }
            }

            return success;


        }

        private void button8_Click(object sender, EventArgs e)
        {

            string entityName = textBox2.Text;
            string variable = comboBox1.SelectedItem.ToString();
            string type = textBox3.Text;
            string value = textBox1.Text;

            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();

            if (ValidateCommand(entityName, variable, type, value))
            {

                richTextBox1.AppendText(DateTime.Now + " " + "Writing Command" + Environment.NewLine);

                string[] command = new string[] { time, entityName, variable, value };


              
                SendCommand(command);
                richTextBox1.AppendText(GetFormatedTime() + " Waiting for response from server" + Environment.NewLine);
                HandleCommandControls(false);

              
               


            }
            else
            {
                richTextBox1.AppendText(GetFormatedTime() + " Error Sending command "  + Environment.NewLine);
            }
            this.ActiveControl = null;
        }

        private void SendCommand(string[] command)
        {

            string publishText = "";
                
            foreach (string s in command)
            {
                publishText += s + " ";
            }

            StaticClass.Publisher.StandardInput.WriteLine(publishText);
            
        }

        public void HandleScenerioData(object sendingProcess, DataReceivedEventArgs outLine)
        {
          string packet = outLine.Data;
          if (packet != null)
          {
              if ((!checkNow) && packet.Contains((CONNECTION_CODE.SCENERIO_UPDATE).ToString("d")))
              {
                  
                  if (GetEntityName(packet).Equals(mEntity.name))
                      alive = true;
                  if ((packet.Split('?')[1].Contains("EOE>(NULL,NULL)")))
                      checkNow = true;
              }
              if(checkNow)
              {
                  if(!(alive))
                  {
                      Die();
                  }
                  else
                  {
                      checkNow = false;
                      alive = false;
                  }
              }
          }
        }

        private void Die()
        {
            StaticClass.Publisher.OutputDataReceived -= new DataReceivedEventHandler(AckOutputDataReceived);
            StaticClass.Subscriber.OutputDataReceived -= new DataReceivedEventHandler(HandleScenerioData);
            SetText(richTextBox1, "You are dead!");
            HandleCommandControls(false);
            label4.BeginInvoke(new Action(()=>label4.Visible = true));

        }

        private string GetEntityName(string packet)
        {
            return packet.Split('?')[1].Split('>')[0];
        }
        
        public void AckOutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            string packet = outLine.Data;
            if (packet != null)
            {
                Console.WriteLine(packet);
                if(packet.Contains("STATUS"))
                {
                    string[] words = packet.Split(' ');

                    if (words[0] == "STATUS")
                    {
                        SetText(richTextBox1, GetFormatedTime() + " " + packet);
                        
                        HandleCommandControls(true);
                       // timer1.Stop();
                    }
                }
            }
        }

        private void SetText(RichTextBox rt, string text, int mode = 0, int newLine = 1)
        {
            string newLineStr = "";
            if (newLine == 1)
                newLineStr = "\n";

            if (mode == 0)
            {
                rt.BeginInvoke(new Action(() => rt.Text = rt.Text + text + newLineStr));
            }
            else
            {
                rt.BeginInvoke(new Action(() => rt.Text = text + newLineStr));
            }
        }

        private void HandleCommandControls(bool enable)
        {
            foreach(Control ctrl in Controls)
            {
                Button b = ctrl as Button;
                
                if (b != null && b!= button10)
                {
                    Console.WriteLine(b.Text);
                    SetEnable(b, enable);
                }

                GroupBox g = ctrl as GroupBox;
                if (g != null)
                {
                    foreach (Control childBtn in g.Controls)
                    {
                        if(childBtn as Button != null)
                            SetEnable(childBtn, enable);
                    }

                }
            }
            SetEnable(comboBox1, enable);
            SetEnable(textBox1, enable);
            SetEnable(comboBox2, enable);



           
        }

        private void SetEnable(Control rt, bool success)
        {
            rt.BeginInvoke(new Action(() => rt.Enabled = success));
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Stop
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "ResumeTrajectory", "FALSE" });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Resume trajectory
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "ResumeTrajectory", "TRUE" });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //STOP tracking enemy
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "Fire", "FALSE" });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //STOP tracking enemy
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "Fire", "TRUE" });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //STOP tracking enemy
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "Track", "FALSE" });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Begin tracking enemy
            DateTime src = DateTime.Now;
            string time = src.Hour.ToString() + ":" + src.Minute.ToString() + ":" + src.Second.ToString();
            SendCommand(new string[] { time, mEntity.name, "Track", "TRUE" });
        }
    }
}
