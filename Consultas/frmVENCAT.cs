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
    public partial class frmVENCAT : Form
    {
        public frmVENCAT()
        {
            InitializeComponent();
            EstiloDGV();
        }

        public Boolean EData;
        public string var1;
        public string var2;

        private void frmVENCAT_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; // activa las teclas de funciones
            this.Text = "Consulta";
        }

        private void frmVENCAT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // aqui pregunta si presionaste la tecla de escape
            {
                this.Close(); // cierra el formulario
            }
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)  // aqui pregunta si presionaste la tecla enter
            {
                e.Handled = true;
                if (txtBuscar.Text.Trim() != string.Empty)  // aqui pregunta si es diferente de vacio el textbox
                {
                    btnBuscar.Focus(); // mueve el cursor hacia el textbox indicado
                }
            }
        }

        private void txtBuscar_Leave(object sender, EventArgs e)
        {
            BuscarData(txtBuscar.Text);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarData(txtBuscar.Text);
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                EData = true;
                var1 = dgv.CurrentRow.Cells[0].Value.ToString();
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();

                this.Close();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // limpiar el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSelecciona.PerformClick();
        }

        private void BuscarData(string _strBuscar)
        {
            // limpiar el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;

            string _query =
                @"SELECT CATEGORIA, CategoriaNombre 
                    FROM CATEGORIAS 
                   WHERE CategoriaNombre LIKE @A1 
                   ORDER BY CategoriaNombre ASC";

            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand(_query, cxn);
            cmd.Parameters.AddWithValue("@A1", "%" + _strBuscar + "%");

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();  // le agrega una linea a la grilla
                int nRow = dgv.Rows.Count - 1;  // obtiene la linea anterior

                dgv[0, nRow].Value = rdr["CATEGORIA"].ToString();
                dgv[1, nRow].Value = rdr["CategoriaNombre"].ToString();
            }

            cmd.Dispose();
            cxn.Close();
        }

        private void EstiloDGV()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("A", "CATEGORIAS");
            this.dgv.Columns.Add("B", "NOMBRES");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 180;
            column = dgv.Columns[01]; column.Width = 500;

            this.dgv.BorderStyle = BorderStyle.FixedSingle;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgv.BackgroundColor = Color.White;

            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
    }
}
