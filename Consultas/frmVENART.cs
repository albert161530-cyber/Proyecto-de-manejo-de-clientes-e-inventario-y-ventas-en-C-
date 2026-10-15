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
    public partial class frmVENART : Form
    {
        public frmVENART()
        {
            InitializeComponent();
            PropiedadesParaDgv();
        }

        public Boolean EData;
        public string var1;
        public string var2;
        public string var3;

        private void frmVENART_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Consulta";

            EData = false; // esta variable es para saber si enviaste datos al formulario padre
        }

        private void frmVENART_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) // esta preguntando si presionaste la tecla de ENTER
            {
                e.Handled = true;
                if (txtBuscar.Text.Trim() != string.Empty) // pregunta si el contenido del textbox es diferente de vacio
                {
                    btnBuscar.Focus(); // mueve el cursor hacia el textbox --> txtPassword
                }
            }
        }

        private void txtBuscar_Leave(object sender, EventArgs e)
        {
            btnBuscar.PerformClick();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarArticulo(txtBuscar.Text); // metodo para buscar usuarios
        }

        private void btnSelecciona_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0) // pregunta si el contador del DGV es mayor que 0, es porque tiene data
            {
                EData = true; // es verdadero porque seleccionaste un registro del DGV

                var1 = dgv.CurrentRow.Cells[0].Value.ToString();
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();
                var3 = dgv.CurrentRow.Cells[2].Value.ToString();
                this.Close();
            }
        }

        private void bntLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
            txtBuscar.Focus();
        }

        private void btSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSelecciona.PerformClick(); // ejecutara el evento del boton btnSelecciona
        }

        private void PropiedadesParaDgv()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("A", "CODIGO");
            this.dgv.Columns.Add("B", "DESCRIPCION");
            this.dgv.Columns.Add("C", "Precio");

            DataGridViewColumn
            column = dgv.Columns[0]; column.Width = 180;
            column = dgv.Columns[1]; column.Width = 500;
            column = dgv.Columns[2]; column.Width = 0;

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

        private void Limpiar()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            EData = false;
        }

        private void BuscarArticulo(string texto)
        {
            Limpiar();

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                // Corregimos los nombres de las columnas para que coincidan con tu tabla PRODUCTOS
                string query = $@"SELECT IDPRODUCTO, DESCRIPCION, PRECIOA 
                          FROM PRODUCTOS 
                          WHERE ESTATUS = 1 
                            AND DESCRIPCION LIKE '%{texto}%'
                        ORDER BY DESCRIPCION ASC";

                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dgv.Rows.Add();
                    int nrow = dgv.Rows.Count - 1;

                    // Mapeamos los resultados a las columnas del DataGridView
                    dgv[0, nrow].Value = dr["IDPRODUCTO"].ToString();
                    dgv[1, nrow].Value = dr["DESCRIPCION"].ToString();
                    dgv[2, nrow].Value = dr["PRECIOA"].ToString();
                }
            }
        }
    }
}
