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
    public partial class frmVENSUP : Form
    {
        public frmVENSUP()
        {
            InitializeComponent();
            EstiloDGV();
        }

        public Boolean EData; // usada para verificar si transporta la data al form padre
        public string var1;   // Data transportada columna 0 al presionar doble clic o seleccionada
        public string var2;   // Data transportada columna 1

        private void frmVENSUP_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true; // activa las teclas de funciones
            this.Text = "Consulta";
            EData = false;
        }

        private void frmVENSUP_KeyDown(object sender, KeyEventArgs e)
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

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // este evento se ejecuta cuando el cursor sale del textBox
            // Metodo para buscar la data en la tabla, le envia el contenido del textBox
            BuscarData(txtBuscar.Text);
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0) // verifica que el dataGridView tenga lineas  
            {
                EData = true;
                var1 = dgv.CurrentRow.Cells[0].Value.ToString(); // asigna el valor de la columna 0 a la variable var1
                var2 = dgv.CurrentRow.Cells[1].Value.ToString(); // asigna el valor de la columna 0 a la variable var1

                this.Close(); // despues de seleccionada las columna cierra la ventana
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
            btnSelecciona.PerformClick(); // ejecuta el evento del boton 
        }

        private void BuscarData(string _strBuscar)
        {
            // limpiar el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;

            string _query =
                @"SELECT IDPROVEEDOR, NombreEmpresa 
                  FROM PROVEEDORES 
                 WHERE ESTATUS = 1 AND NombreEmpresa LIKE @A1
                 ORDER BY NombreEmpresa ASC";

            SqlConnection cxn = new SqlConnection(cnn.db);     
            cxn.Open();  

            SqlCommand cmd = new SqlCommand(_query, cxn); 
            cmd.Parameters.AddWithValue("@A1", "%" + _strBuscar + "%");

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();  
                int nRow = dgv.Rows.Count - 1;  

                dgv[0, nRow].Value = rdr["IDPROVEEDOR"].ToString();
                dgv[1, nRow].Value = rdr["NombreEmpresa"].ToString();
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

            this.dgv.Columns.Add("A", "ID");
            this.dgv.Columns.Add("B", "SUPLIDOR");

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
