using System;
using System.Windows.Forms;

namespace Auto_Updater
{
    public partial class Application : Form
    {
        public Application()
        {
            InitializeComponent();
            // Onload Event
            Text = "v" + AutoUpdater.CheckForUpdate(1.0m, "http://pastebin.com/raw/MtCQPNAK");
                                                  // ^ current version
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
