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
        private Timer timer1;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            mEntity = StaticClass.SelectedEntity;
            StaticClass.Publisher.OutputDataReceived += new DataReceivedEventHandler(AckOutputDataReceived);
            StaticClass.Subscriber.OutputDataReceived += new DataReceivedEventHandler(HandleScenerioData);
            textBox2.Text = mEntity.name;
            PopulateVariables();
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
              if (packet.Contains((CONNECTION_CODE.SCENERIO_UPDATE).ToString("d")))
              {
                  Console.WriteLine("Updating Scenerio");
              }
          }
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
    }
}
