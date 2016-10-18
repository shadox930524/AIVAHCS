using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.IO.Ports;

namespace Voice_Bot
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;

        SerialPort port = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);

        Choices list = new Choices();

        public Form1()
        {

            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

            list.Add(new string[] { "hi", "how are you", "wake", "sleep", "master bedroom light", "light off" });

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
            textBox4.AppendText(h + "\n");
        }

        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;
            if (r == "wake")
            {
                wake = true;
                label3.Text = "State : Awake mode";
            }

            if (r == "sleep")
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




        private void Form1_Load(object sender, EventArgs e)
        {

        }

      
    }
}
