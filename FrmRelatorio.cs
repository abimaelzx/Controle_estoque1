﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace controle_de_estoque1
{
    public partial class FrmRelatorio : Form
    {
        Thread nt;
        public FrmRelatorio()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            nt = new Thread(novofrmMenu);
            nt.SetApartmentState(ApartmentState.STA);
            nt.Start();
        }

        private void novofrmMenu()
        {
            Application.Run(new FrmMenu());
        }
    }
}
