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
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    buttonConnect.BackColor = System.Drawing.Color.Blue;
                    labelStatus.Text = "Disconnected";
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

                    // data bits
                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    // parity
                    if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

                    // stop bits
                    if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                    else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;

                    // handshake
                    if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                    else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;

                    // RTS / DTR
                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    // open and verify device responds to ping
                    serialPortArduino.Open();
                    serialPortArduino.DiscardInBuffer();
                    serialPortArduino.WriteLine("ping");

                    string response = string.Empty;
                    try
                    {
                        response = serialPortArduino.ReadLine().Trim();
                    }
                    catch (TimeoutException)
                    {
                        // no response within timeout
                    }

                    if (string.Equals(response, "pong", StringComparison.OrdinalIgnoreCase))
                    {
                        radioButtonVerbonden.Checked = true;
                        buttonConnect.Text = "Disconnect";
                        buttonConnect.BackColor = System.Drawing.Color.Red;
                        labelStatus.Text = "Connected";
                    }
                    else
                    {
                        serialPortArduino.Close();
                        MessageBox.Show("Geen geldig antwoord ontvangen van apparaat. Antwoord: " + (string.IsNullOrEmpty(response) ? "(geen)" : response));
                        labelStatus.Text = "Disconnected";
                        radioButtonVerbonden.Checked = false;
                        buttonConnect.Text = "Connect";
                        buttonConnect.BackColor = System.Drawing.Color.Blue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij (dis)connect: " + ex.Message);
                labelStatus.Text = "Error: " + ex.Message;
            }
        }
    }
}
