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
using System.Xml.Linq;

namespace PLUNES
{
    public partial class frmRNCCTE : Form
    {
        public frmRNCCTE()
        {
            InitializeComponent();
        }

        public bool ERnc;
        public string varRNC;
        public string varRNCName;

        private void frmRNCCTE_Load(object sender, EventArgs e)
        {
            this.Text = "RNC Clientes";
            this.KeyPreview = true; 
            ERnc= false;
        }

        private void frmRNCCTE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            { 
                this.Close();
            }
        }

        private void txtRNC_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void txtRNC_Leave(object sender, EventArgs e)
        {
            
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtNombre_Leave(object sender, EventArgs e)
        {

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
