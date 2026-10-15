using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLUNES
{
    public partial class frmVerifica : Form
    {
        public frmVerifica()
        {
            InitializeComponent();
        }


        private void frmVerifica_Load(object sender, EventArgs e)
        {
            this.Text = "Verifica";
            this.KeyPreview = true;
        }

        private void frmVerifica_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
