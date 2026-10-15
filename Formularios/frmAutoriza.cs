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
    public partial class frmAutoriza : Form
    {
        public frmAutoriza()
        {
            InitializeComponent();
        }

        public bool EData;
        public string var1;
        public string var2;

        private void frmAutoriza_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "POSS";
        }

        private void frmAutoriza_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) 
            { 
                EData = false;  
                this.Close();  
            } 
        }

        private void txtAutoriza_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void txtAutoriza_Leave(object sender, EventArgs e)
        {
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // 1. Debemos marcar la variable EData como verdadera
            // para que el formulario anterior sepa que pusiste la clave bien
            this.EData = true;

            // 2. Ahora sí cerramos
            this.Close();
        }
    }
}
