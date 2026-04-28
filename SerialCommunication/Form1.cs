using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        private SerialPort serialPortArduino;
        public Form1()
        {
            InitializeComponent();

            // instantiate serial port and set timeouts (milliseconds)
            serialPortArduino = new SerialPort();
            serialPortArduino.ReadTimeout = 1000;
            serialPortArduino.WriteTimeout = 1000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    // we are connected — disconnect
                    serialPortArduino.Close();
                    buttonConnect.Text = "Connect";
                    buttonConnect.BackColor = System.Drawing.Color.Blue;
                }
                else
                {
                    // not connected — set necessary properties and open
                    if (comboBoxPoort.SelectedItem != null)
                        serialPortArduino.PortName = comboBoxPoort.SelectedItem.ToString();

                    int baud = 115200;
                    if (comboBoxBaudrate.SelectedItem != null)
                        int.TryParse(comboBoxBaudrate.SelectedItem.ToString(), out baud);
                    serialPortArduino.BaudRate = baud;

                    serialPortArduino.Open();
                    buttonConnect.Text = "Disconnect";
                    buttonConnect.BackColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij (dis)connect: " + ex.Message);
            }
        }
    }
}
