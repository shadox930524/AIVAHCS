using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.IO.Ports;

namespace generateWaveFile
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;
        String TESTING_SAMPLE = "100    ";
        SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);

        Choices list = new Choices();

        public Form1()
        {
     
            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

            list.Add(new string[] { "hi", "how are you", "wake", "hibernate", "master bedroom light", "light off" });

            Grammar gr = new Grammar(new GrammarBuilder(list));

            try
            {
                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeechRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch { return; }

            InitializeComponent();
        }

        public void say(String h)
        {
            s.Speak(h);
            textBox1.AppendText(h + "\n");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            List<NAudio.Wave.WaveInCapabilities> sources = new List<NAudio.Wave.WaveInCapabilities>();

            for(int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                sources.Add(NAudio.Wave.WaveIn.GetCapabilities(i));
            }

            SourceList.Items.Clear();

            foreach(var source in sources)
            {
                ListViewItem item = new ListViewItem(source.ProductName);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, source.Channels.ToString()));
                SourceList.Items.Add(item);
            }
        }

        private NAudio.Wave.WaveIn sourceStream = null;
        private NAudio.Wave.DirectSoundOut waveOut = null;
        private NAudio.Wave.WaveFileWriter waveWriter = null;

        private void button2_Click(object sender, EventArgs e)
        {
          
            if (SourceList.SelectedItems.Count == 0) return;

            int deviceNumber = SourceList.SelectedItems[0].Index;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);

            NAudio.Wave.WaveInProvider waveIn = new NAudio.Wave.WaveInProvider(sourceStream);

            waveOut = new NAudio.Wave.DirectSoundOut();
            waveOut.Init(waveIn);

            sourceStream.StartRecording();
            waveOut.Play();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (SourceList.SelectedItems.Count == 0) return;
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Wave File (*.wav|*.wav;";
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            int deviceNumber = SourceList.SelectedItems[0].Index;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);

            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            waveWriter = new NAudio.Wave.WaveFileWriter(save.FileName, sourceStream.WaveFormat);

            sourceStream.StartRecording();
        }

        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;
            {
                waveWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
                waveWriter.Flush();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if(waveWriter !=null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
            this.Close();
        }

        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;
            if (r == "wake")
            {
                wake = true;
                label3.Text = "State : Awake mode";
            }

            if (r == "hibernate")
            {
                wake = false;
                label3.Text = "State : Sleep mode";

            }

            //adding code here even the mode is "sleep", it will receive the input and give output

            if (wake == true)
            {
                if (r == "master bedroom light")
                {
                    port.Open();
                    port.WriteLine("A");
                    say("light is on");
                    port.Close();
                }
                if (r == "light off")
                {
                    port.Open();
                    port.WriteLine("B");
                    say("light is off");
                    port.Close();
                }


                if (r == "hi")
                {
                    say("Hi");
                }
                if (r == "how are you")
                {
                    say("Im Fine");
                }
            }
            textBox3.AppendText(r + "\n");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
