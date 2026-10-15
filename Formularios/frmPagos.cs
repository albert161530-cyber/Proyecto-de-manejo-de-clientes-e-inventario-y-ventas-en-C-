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
    public partial class frmPagos : Form
    {
        public frmPagos(string subtotal, string impuesto, string total)
        {
            InitializeComponent();
            _subtotal = subtotal;
            _impuesto = impuesto;
            _total = total;
        }

        string input = string.Empty;

        string _subtotal;
        string _impuesto;
        string _total;

        public bool EPago;
        public string vtaSubtotal;
        public string vtaImpuesto;
        public string vtaTotal;
        public string vtaPago;
        public string vtaDevuelta;
        
        private void frmPagos_Load(object sender, EventArgs e)
        {
            lblSubTotal.Text = _subtotal;
            lblImpuesto.Text = _impuesto;
            lblTotalPagar.Text = _total;
        }

        private void frmPagos_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad0: btnCero.PerformClick(); break;
                case Keys.NumPad1: btnUno.PerformClick(); break;
                case Keys.NumPad2: btnDos.PerformClick(); break;
                case Keys.NumPad3: btnTres.PerformClick(); break;
                case Keys.NumPad4: btnCuatro.PerformClick(); break;
                case Keys.NumPad5: btnCinco.PerformClick(); break;
                case Keys.NumPad6: btnSeis.PerformClick(); break;
                case Keys.NumPad7: btnSiete.PerformClick(); break;
                case Keys.NumPad8: btnOcho.PerformClick(); break;
                case Keys.NumPad9: btnNueve.PerformClick(); break;

                case Keys.Enter: btnVender.PerformClick(); break;
                case Keys.Escape: btnVender.PerformClick(); break;
            }
        }

        private void btnVender_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPago.Text))
            {
                EPago = true;
                double resultado = Convert.ToDouble(txtPago.Text) - Convert.ToDouble(lblTotalPagar.Text);
                lblDevuelta.Text = resultado.ToString();
                vtaDevuelta =  resultado.ToString();
                vtaPago = txtPago.Text;

            }

        }

        private void txtPago_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (!string.IsNullOrEmpty(txtPago.Text))
                {
                    btnVender.Focus();
                }
            }
        }

        private void btnUno_Click(object sender, EventArgs e)
        {
            txtPago.Text += "1";
        }

        private void btnDos_Click(object sender, EventArgs e)
        {
            txtPago.Text += "2";
        }

        private void btnTres_Click(object sender, EventArgs e)
        {
            txtPago.Text += "3";
        }

        private void btnCuatro_Click(object sender, EventArgs e)
        {
            txtPago.Text += "4";
        }

        private void btnCinco_Click(object sender, EventArgs e)
        {
            txtPago.Text += "5";
        }

        private void btnSeis_Click(object sender, EventArgs e)
        {
            txtPago.Text += "6";
        }

        private void btnSiete_Click(object sender, EventArgs e)
        {
            txtPago.Text += "7";
        }

        private void btnOcho_Click(object sender, EventArgs e)
        {
            txtPago.Text += "8";
        }

        private void btnNueve_Click(object sender, EventArgs e)
        {
            txtPago.Text += "9";
        }

        private void btnCero_Click(object sender, EventArgs e)
        {
            txtPago.Text += "0";
        }

        private void btnPunto_Click(object sender, EventArgs e)
        {
            txtPago.Text += ".";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtPago.Text.Length > 0)
            {
                txtPago.Text = txtPago.Text.Remove(txtPago.Text.Length - 1);


                txtPago.SelectionStart = txtPago.Text.Length;
                txtPago.Focus();
            }
        }

        private void lblImpuesto_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
