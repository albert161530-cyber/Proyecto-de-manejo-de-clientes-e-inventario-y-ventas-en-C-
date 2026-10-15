using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PLUNES
{
    public partial class frmProducto : Form
    {
        string ruta = Application.StartupPath + @"\Imagenes\";
        public frmProducto()
        {
            InitializeComponent();

            EstiloDGV();
            EstiloDGV_ART();
            EstiloDGV_STOCK();
            EstiloDGV_OFER();
            EstiloDGV_OFERTA();
        }

        bool ExisteData;

        string oCC;
        string cOF;
        string fDE;
        string fFN;
        string pOF;
        string Des;

        string suc;
        string can;

        private void frmProducto_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Articulos / Productos";

            // 1. Busca el siguiente código
            txtArticulo.Text = Busco.BuscaUltimoNumero("4");

            // 2. CORRECCIÓN DE LA IMAGEN:
            // Primero definimos una ruta real (ajusta esto a una imagen que tengas)
            string rutaImagenDefault = Application.StartupPath + @"\Imagenes\default.jpg";

            if (System.IO.File.Exists(rutaImagenDefault))
            {
                picProducto.Image = Image.FromFile(rutaImagenDefault);
            }
            // Si no existe, no cargamos nada para evitar que el programa se cierre

            txtAltura.Text = "40";
            txtCopia.Text = "2";

            MostrarCodigoBarra(txtArticulo.Text, txtAltura.Text);
        }

        private void frmProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtArticulo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnArticulo.PerformClick();
            }
        }

        private void txtArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)  
            {
                e.Handled = true;
                if (txtArticulo.Text.Trim() != string.Empty)  
                {
                    txtNombre.Focus(); 
                }
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text);
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtNombre.Text.Trim() != string.Empty)
                {
                    txtSuplidor.Focus();
                }
            }
        }


        private void txtSuplidor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtSuplidor.Text.Trim() != string.Empty)
                {
                    txtCosto.Focus();
                }
            }
        }

        private void txtSuplidor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnSuplidor.PerformClick();
            }
        }

        private void txtSuplidor_Leave(object sender, EventArgs e)
        {
            if (txtSuplidor.Text.Trim() != string.Empty)
            {
                // 1. Declaramos las variables locales para recibir los datos
                string _id = txtSuplidor.Text;
                string _nombre = "";
                bool found = false;

                // 2. Llamamos al método correcto (BuscarSuplidor) con ref y out
                // Nota: Si tu clase Busco no tiene 'out found', usa solo los ref
                Busco.BuscarSuplidor(ref _id, ref _nombre);

                // Asumimos que si el nombre vuelve vacío, es que no lo encontró
                if (string.IsNullOrEmpty(_nombre))
                {
                    MessageBox.Show("Suplidor ingresado no existe.", "Suplidor no encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSuplidor.Clear();
                    lblSuplidor.Text = "";
                    txtSuplidor.Focus();
                }
                else
                {
                    // Si lo encontró, actualizamos la etiqueta
                    lblSuplidor.Text = _nombre;
                }
            }
        }
        private void btnSuplidor_Click(object sender, EventArgs e)
        {
            frmVENSUP frm = new frmVENSUP();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtSuplidor.Text = frm.var1;
                lblSuplidor.Text = frm.var2;
            }
        }

        private void txtCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCosto.Text.Trim() != string.Empty)
                {
                    txtPorciento.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCosto.Text.Contains('.')
                && !txtCosto.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtCosto_Leave(object sender, EventArgs e)
        {
            string texto = txtCosto.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCosto.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtCosto.Text = valor.ToString("N2"); // 123,123.00
        }


        private void txtPorciento_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPorciento.Text.Trim() != string.Empty)
                {
                    txtBarCode.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPorciento.Text.Contains('.')
                && !txtPorciento.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPorciento_Leave(object sender, EventArgs e)
        {
            string texto = txtPorciento.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPorciento.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPorciento.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtBarCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtBarCode.Text.Trim() != string.Empty)
                {
                    txtReorden.Focus();
                }
            }
        }

        private void txtReorden_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtReorden.Text.Trim() != string.Empty)
                {
                    txtPrecioA.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtReorden.Text.Contains('.')
                && !txtReorden.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtReorden_Leave(object sender, EventArgs e)
        {
            string texto = txtReorden.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReorden.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtReorden.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioA.Text.Trim() != string.Empty)
                {
                    txtPrecioB.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioA.Text.Contains('.')
                && !txtPrecioA.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioA_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioB.Text.Trim() != string.Empty)
                {
                    txtPrecioC.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioB.Text.Contains('.')
                && !txtPrecioB.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioB_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioC.Text.Trim() != string.Empty)
                {
                    txtPrecioD.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioC.Text.Contains('.')
                && !txtPrecioC.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioC_LocationChanged(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioC_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioD.Text.Trim() != string.Empty)
                {
                    txtPrecioE.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioD.Text.Contains('.')
                && !txtPrecioD.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioD_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtPrecioE_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioE.Text.Trim() != string.Empty)
                {
                    txtCategoria.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioE.Text.Contains('.')
                && !txtPrecioE.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioE_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioA.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioA.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioA.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtSucursal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtSucursal.Text.Trim() != string.Empty)
                {
                    txtCantidad.Focus();
                }
            }
        }

        private void txtSucursal_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSucursal.Text))
            {
                lblSucursal.Text = Busco.BuscarSucursal(txtSucursal.Text, out bool found);

                if (!found)
                {
                    MessageBox.Show("La sucursal ingresada no existe.", "sucursal no encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSucursal.Clear();
                    lblSucursal.Text = "";
                    txtSucursal.Focus();
                }
            }
        }

        private void txtSucursal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnSucursal.PerformClick();
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    btnInserta.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCantidad.Text.Contains('.')
                && !txtCantidad.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (txtSucursal.Text.Trim() != string.Empty)
            {
                btnInserta.PerformClick();
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.RowCount > 0)
                btnEditarLinea.PerformClick();
        }

        private void txtAltura_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtAltura.Text.Trim() != string.Empty)
                {
                    txtCopia.Focus();
                }
            }
        }

        private void txtAltura_Leave(object sender, EventArgs e)
        {
            if (txtAltura.Text.Trim() != string.Empty)
            {
                btnActualiza.PerformClick();
            }
        }

        private void txtCopia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCopia.Text.Trim() != string.Empty)
                {
                    btnImprimir.Focus();
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarMaestros())
                return;

            if (!ValidarFormulario())
                return;

            if (!ExisteData)
            {
                InsertarArticulo();  // INSERTA EL NUEVO REGISTRO
                GuardarInventario(txtArticulo.Text, txtReorden.Text);
                GuardarLasOfertas(txtArticulo.Text);
                ActualizaSecuenciaArticulo("4", txtArticulo.Text); // ACTUALIZA LA NUEVA SECUENCIA
            }
            else
            {
                ActualizarArticulo();
                GuardarInventario(txtArticulo.Text, txtReorden.Text);
                GuardarLasOfertas(txtArticulo.Text);
            }

            btnLimpiar.PerformClick();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();  // LIMPIA EL FORM
            LimpiarDetalle();

            txtArticulo.Text = Busco.BuscaUltimoNumero("4"); // busca el siguiente registro a utilizar
            txtArticulo.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (ExisteData == true)
            {
                DialogResult dialogResult = MessageBox.Show("Deseas Borrar el Registro", "ITLA",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    BorrarRegistro(txtArticulo.Text);
                    btnLimpiar.PerformClick();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnArticulo_Click(object sender, EventArgs e)
        {
            frmVENART frm = new frmVENART();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtArticulo.Text = frm.var1;
                BuscarArticulo(txtArticulo.Text);
            }
        }

        /* COMENTAMOS TODO EL BLOQUE PARA QUE NO DÉ ERROR
   private void btnSucursal_Click(object sender, EventArgs e)
   {
       // frmVENSUC frm = new frmVENSUC();
       // frm.ShowDialog();

       // if (frm.EData == true)
       // {
       //     txtSucursal.Text = frm.var1;
       //     lblSucursal.Text = frm.var2;
       // }
   }
   */

        private void btnInserta_Click(object sender, EventArgs e)
        {
            if(txtSucursal.Text.Trim() !="")
            {
                if (txtCantidad.Text.Trim() != "")
                {
                    AgregarLinea();
                    ContarCantidaddes();

                    LimpiarDetalle();
                    txtSucursal.Focus();
                }
            }
        }

        private void btnLimpiarDetalle_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            txtSucursal.Focus();
        }

        private void btnBorrarLinea_Click(object sender, EventArgs e)
        {
            BorrarUnaLineaDgv();
            ContarCantidaddes();

            LimpiarDetalle();
            txtSucursal.Focus();
        }

        private void btnEditarLinea_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();

            txtSucursal.Text = dgv.CurrentRow.Cells[0].Value.ToString();
            lblSucursal.Text = dgv.CurrentRow.Cells[1].Value.ToString();
            txtCantidad.Text = dgv.CurrentRow.Cells[2].Value.ToString();

            BorrarUnaLineaDgv();
            ContarCantidaddes();
            txtSucursal.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void btnActualiza_Click(object sender, EventArgs e)
        {
            if (txtAltura.Text.Trim() != string.Empty)
            {
                long altura =  long.Parse(txtAltura.Text);
                string barra = txtArticulo.Text.ToString();

                picBarcode.SizeMode = PictureBoxSizeMode.CenterImage;
                picBarcode.BackColor = Color.White;
                picBarcode.Image = Code128(barra, PrintTextInCode: true, Height: altura);
            }
        }


        private void picProducto_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _imagen = openFileDialog1.FileName;
                picProducto.Image = Image.FromFile(_imagen);
            }
        }

        // --------------------------------------------------
        // BARCODE
        // --------------------------------------------------
        private void MostrarCodigoBarra(string _codigo, string _altura)
        {
            long nHeight = long.Parse(_altura);
            string sTexto = Convert.ToString(_codigo);

            picBarcode.SizeMode = PictureBoxSizeMode.CenterImage;
            picBarcode.BackColor = Color.White;
            picBarcode.Image = Code128(sTexto, PrintTextInCode: true, Height: nHeight);
        }

        public enum Code128SubTypes
        {
            CODE128 = iTextSharp.text.pdf.Barcode.CODE128,
            CODE128_RAW = iTextSharp.text.pdf.Barcode.CODE128_RAW,
            CODE128_UCC = iTextSharp.text.pdf.Barcode.CODE128_UCC,
        }

        public static Bitmap Code128(string _code, Code128SubTypes codeType = Code128SubTypes.CODE128, bool PrintTextInCode = false,
                                     float Height = 0, bool GenerateChecksum = true, bool ChecksumText = true)
        {
            if (_code.Trim() == "")
            {
                return null;
            }
            else
            {
                Barcode128 barcode = new Barcode128();

                barcode.CodeType = (int)codeType;
                barcode.StartStopText = true;
                barcode.GenerateChecksum = GenerateChecksum;
                barcode.ChecksumText = ChecksumText;
                if (Height != 0) barcode.BarHeight = Height;
                barcode.Code = _code;
                try
                {
                    System.Drawing.Bitmap bm = new System.Drawing.Bitmap(barcode.CreateDrawingImage
                        (System.Drawing.Color.Black, System.Drawing.Color.White));
                    if (PrintTextInCode == false)
                    {
                        return bm;
                    }
                    else
                    {
                        Bitmap bmT;
                        bmT = new Bitmap(bm.Width, bm.Height + 14);
                        Graphics g = Graphics.FromImage(bmT);
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height + 14);

                        Font drawFont = new Font("Arial", 8);
                        SolidBrush drawBrush = new SolidBrush(Color.Black);

                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(_code, drawFont);
                        float xCenter = (bm.Width - stringSize.Width) / 2;
                        float x = xCenter;
                        float y = bm.Height;

                        StringFormat drawFormat = new StringFormat();
                        drawFormat.FormatFlags = StringFormatFlags.NoWrap;

                        g.DrawImage(bm, 0, 0);
                        g.DrawString(_code, drawFont, drawBrush, x, y, drawFormat);

                        return bmT;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Codigo De Barra Code128. Desc:" + ex.Message);
                }
            }
        }


        // --------------------------------------------------
        // METODOS
        // --------------------------------------------------
        private void EstiloDGV()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("A", "Sucursal / Almacen");
            this.dgv.Columns.Add("B", "Nombre Sucursal / Almacen");
            this.dgv.Columns.Add("C", "Cantidad");

            DataGridViewColumn
            column = dgv.Columns[0]; column.Width = 160;
            column = dgv.Columns[1]; column.Width = 420;
            column = dgv.Columns[2]; column.Width = 125;
            dgv.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

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

        private void EstiloDGV_STOCK()
        {
            this.dgvStock.EnableHeadersVisualStyles = false;
            this.dgvStock.AllowUserToAddRows = false;
            this.dgvStock.AllowUserToDeleteRows = false;
            this.dgvStock.ColumnHeadersVisible = true;
            this.dgvStock.RowHeadersVisible = false;

            this.dgvStock.Columns.Add("A", "Sucursal / Almacen");
            this.dgvStock.Columns.Add("B", "Nombre Sucursal / Almacen");
            this.dgvStock.Columns.Add("B", "Cantidad");

            DataGridViewColumn
            column = dgvStock.Columns[0]; column.Width = 100;
            column = dgvStock.Columns[1]; column.Width = 340;
            column = dgvStock.Columns[2]; column.Width = 100;
            dgvStock.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvStock.BorderStyle = BorderStyle.FixedSingle;
            this.dgvStock.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvStock.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvStock.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvStock.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvStock.BackgroundColor = Color.White;

            this.dgvStock.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvStock.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvStock.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvStock.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void EstiloDGV_OFER()
        {
            this.dgvOFER.EnableHeadersVisualStyles = false;
            this.dgvOFER.AllowUserToAddRows = false;
            this.dgvOFER.AllowUserToDeleteRows = false;
            this.dgvOFER.ColumnHeadersVisible = false;
            this.dgvOFER.RowHeadersVisible = false;

            this.dgvOFER.Columns.Add("A", "Cant Oferta");
            this.dgvOFER.Columns.Add("B", "Cant Entregar");
            this.dgvOFER.Columns.Add("B", "Fecha Desde");
            this.dgvOFER.Columns.Add("C", "Fecha Fin");
            this.dgvOFER.Columns.Add("D", "Precio Oferta");
            this.dgvOFER.Columns.Add("E", "Descuento Oferta");

            DataGridViewColumn
            column = dgvOFER.Columns[0]; column.Width = 125;
            column = dgvOFER.Columns[1]; column.Width = 125;
            column = dgvOFER.Columns[2]; column.Width = 125;
            column = dgvOFER.Columns[3]; column.Width = 125;
            column = dgvOFER.Columns[4]; column.Width = 125; dgvOFER.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column = dgvOFER.Columns[5]; column.Width = 125; dgvOFER.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOFER.BorderStyle = BorderStyle.FixedSingle;
            this.dgvOFER.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvOFER.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvOFER.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvOFER.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvOFER.BackgroundColor = Color.White;

            this.dgvOFER.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvOFER.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvOFER.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvOFER.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void EstiloDGV_OFERTA()
        {
            this.dgvOferta.EnableHeadersVisualStyles = false;
            this.dgvOferta.AllowUserToAddRows = false;
            this.dgvOferta.AllowUserToDeleteRows = false;
            this.dgvOferta.ColumnHeadersVisible = true;
            this.dgvOferta.RowHeadersVisible = false;

            this.dgvOferta.Columns.Add("A", "Cant Oferta");
            this.dgvOferta.Columns.Add("B", "Cant Entregar");
            this.dgvOferta.Columns.Add("B", "Fecha Desde");
            this.dgvOferta.Columns.Add("C", "Fecha Fin");
            this.dgvOferta.Columns.Add("D", "Precio Oferta");
            this.dgvOferta.Columns.Add("E", "Descuento Oferta");

            DataGridViewColumn
            column = dgvOferta.Columns[0]; column.Width = 125;
            column = dgvOferta.Columns[1]; column.Width = 125;
            column = dgvOferta.Columns[2]; column.Width = 125;
            column = dgvOferta.Columns[3]; column.Width = 125;
            column = dgvOferta.Columns[4]; column.Width = 125; dgvOferta.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column = dgvOferta.Columns[5]; column.Width = 125; dgvOferta.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvOferta.BorderStyle = BorderStyle.FixedSingle;
            this.dgvOferta.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvOferta.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvOferta.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvOferta.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvOferta.BackgroundColor = Color.White;

            this.dgvOferta.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvOferta.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvOferta.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvOferta.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void EstiloDGV_ART()
        {
            this.dgvArt.EnableHeadersVisualStyles = false;
            this.dgvArt.AllowUserToAddRows = false;
            this.dgvArt.AllowUserToDeleteRows = false;
            this.dgvArt.ColumnHeadersVisible = true;
            this.dgvArt.RowHeadersVisible = false;

            this.dgvArt.Columns.Add("A", "Producto");
            this.dgvArt.Columns.Add("B", "Nombre");
            this.dgvArt.Columns.Add("B", "Costo");
            this.dgvArt.Columns.Add("C", "Precio A");
            this.dgvArt.Columns.Add("D", "Precio B");
            this.dgvArt.Columns.Add("E", "Precio C");
            this.dgvArt.Columns.Add("G", "Precio D");
            this.dgvArt.Columns.Add("H", "Precio E");
            this.dgvArt.Columns.Add("I", "Categoria");
            this.dgvArt.Columns.Add("J", "Existencia");

            DataGridViewColumn
            column = dgvArt.Columns[0]; column.Width = 125;
            column = dgvArt.Columns[1]; column.Width = 125;
            column = dgvArt.Columns[2]; column.Width = 125;
            column = dgvArt.Columns[3]; column.Width = 125;
            column = dgvArt.Columns[4]; column.Width = 125; dgvArt.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column = dgvArt.Columns[5]; column.Width = 125; dgvArt.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.dgvArt.BorderStyle = BorderStyle.FixedSingle;
            this.dgvArt.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgvArt.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvArt.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgvArt.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgvArt.BackgroundColor = Color.White;

            this.dgvArt.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgvArt.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgvArt.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgvArt.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void LimpiarFormulario()
        {
            txtArticulo.Clear();
            txtNombre.Clear();

            txtCosto.Clear();
            txtPorciento.Clear();
            txtBarCode.Clear();
            txtReorden.Clear();

            txtPrecioA.Clear();
            txtPrecioB.Clear();
            txtPrecioC.Clear();
            txtPrecioD.Clear();
            txtPrecioE.Clear();

            txtSucursal.Clear();
            lblSucursal.Text = "";
            txtCantidad.Clear();

            picProducto.Image = null;
            picBarcode.Image = null;

            txtAltura.Clear();
            txtCopia.Clear();

            txtAltura.Text = "40";
            txtCopia.Text = "2";

            LimpiarDetalleOfertas();

            // limpiar el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            this.dgvOFER.Rows.Clear();
            this.dgvOFER.Refresh();
        }

        private void BuscarArticulo(string _articulo)
        {
            // Verificamos si no hay imagen cargada
            if (picProducto.Image == null)
            {
                // Validamos que el archivo realmente exista antes de intentar abrirlo
                if (System.IO.File.Exists(ruta))
                {
                    picProducto.Image = Image.FromFile(ruta);
                }
                else
                {
                    // Opcional: Cargar una imagen por defecto si la ruta no existe
                    // picProducto.Image = Properties.Resources.ImagenNoDisponible;
                }
            }

            ExisteData = false;

            LimpiarFormulario();

            txtArticulo.Text = _articulo;

            // Cambiamos INNER JOIN PROVEEDORES por un ajuste para que no explote
            // Si no tienes una tabla de nombres de suplidores, traemos solo el código
            string query = @"SELECT A.IDPRODUCTO, A.DESCRIPCION, A.COSTO, A.PRECIOA, A.PRECIOB, A.PRECIOC, A.PRECIOD, A.PRECIOE,
                        A.IMPUESTO, A.STOCK, A.BARCODE, A.FOTO, A.REORDEN, A.SUPLIDOR, 
                        A.SUPLIDOR AS NOMBREEMPRESA -- Usamos el código como nombre temporalmente
                 FROM PRODUCTOS A 
                 WHERE A.IDPRODUCTO = @A1 AND A.ESTATUS = 1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                if (rcd.Read())
                {
                    txtArticulo.Text = rcd["IDPRODUCTO"].ToString();
                    txtNombre.Text = rcd["DESCRIPCION"].ToString();
                    txtSuplidor.Text = rcd["SUPLIDOR"].ToString();
                    lblSuplidor.Text = rcd["NOMBREEMPRESA"].ToString();
                    txtCosto.Text = rcd["COSTO"].ToString();
                    txtPorciento.Text = rcd["IMPUESTO"].ToString();
                    txtBarCode.Text = rcd["BARCODE"].ToString();
                    txtReorden.Text = rcd["REORDEN"].ToString();

                    txtPrecioA.Text = rcd["PRECIOA"].ToString();
                    txtPrecioB.Text = rcd["PRECIOB"].ToString();
                    txtPrecioC.Text = rcd["PRECIOC"].ToString();
                    txtPrecioD.Text = rcd["PRECIOD"].ToString();
                    txtPrecioE.Text = rcd["PRECIOE"].ToString();

                    lblExistencia.Text = rcd["STOCK"].ToString();
                }

                // --- BLOQUE CORREGIDO ---
                try
                {
                    // Si el campo FOTO no es nulo, lo convertimos
                    if (rcd["FOTO"] != DBNull.Value)
                    {
                        picProducto.Image = ConvertImage.ByteArrayToImage((byte[])rcd["FOTO"]);
                    }
                }
                catch
                {
                    // Si falla la conversión o no hay foto en BD, intentamos cargar la de la carpeta ruta
                    if (picProducto.Image == null && System.IO.File.Exists(ruta))
                    {
                        picProducto.Image = Image.FromFile(ruta);
                    }
                }
                // -----------------------

                MostrarCodigoBarra(txtArticulo.Text, txtAltura.Text);
                MostrarInventario(txtArticulo.Text);
                MostrarOfertas(txtArticulo.Text);
                ContarCantidaddes();
            }
        }

        private void MostrarInventario(string _articulo)
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            string query = @"SELECT B.IDSUCURSAL, B.NOMBREDESUCURSAL, A.STOCK 
                             FROM ALMACENSTOCK A 
                             LEFT JOIN SUCURSALES B ON B.IDSUCURSAL = A.IDALMACEN 
                             WHERE A.IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read()) 
                {
                    dgv.Rows.Add();
                    int nRow = dgv.Rows.Count - 1;

                    dgv[0, nRow].Value = rcd["IDSUCURSAL"].ToString();
                    dgv[1, nRow].Value = rcd["NOMBREDESUCURSAL"].ToString();
                    // Convertir STOCK antes de formatear
                    decimal stock = Convert.ToDecimal(rcd["STOCK"]);
                    dgv[2, nRow].Value = stock.ToString("N2");
                }
            }
        }

        private void Mostrar_Inventario(string _articulo)
        {
            dgv.Rows.Clear();

            string query = @"
                    SELECT B.IDSUCURSAL, B.NOMBREDESUCURSAL, A.STOCK 
                    FROM ALMACENSTOCK A 
                    LEFT JOIN SUCURSALES B ON B.IDSUCURSAL = A.IDALMACEN 
                    WHERE A.IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            using (SqlCommand cmd = new SqlCommand(query, cxn))
            {
                cmd.Parameters.Add("@A1", SqlDbType.VarChar).Value = _articulo;
                cxn.Open();

                using (SqlDataReader rcd = cmd.ExecuteReader())
                {
                    while (rcd.Read())
                    {
                        dgv.Rows.Add(
                            rcd["IDSUCURSAL"],
                            rcd["NOMBREDESUCURSAL"],
                            Convert.ToDouble(rcd["STOCK"])
                        );
                    }
                }
            }
        }

        private void MostrarOfertas(string _articulo)
        {
            this.dgvOFER.Rows.Clear();
            this.dgvOFER.Refresh();

            string query = @"SELECT * FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read())
                {
                    dgvOFER.Rows.Add();
                    int nRow = dgvOFER.Rows.Count - 1;

                    // Convertir correctamente STOCK antes de formatear
                    double P1 = ToDoubleSeguro(rcd["OfertaCantPorCompra"]);
                    double P2 = ToDoubleSeguro(rcd["CantidadOfertada"]);
                    double P3 = ToDoubleSeguro(rcd["PrecioOferta"]);
                    double P4 = ToDoubleSeguro(rcd["Descuento"]);

                    dgvOferta[0, nRow].Value = P1.ToString("N2"); // Cantidad compra
                    dgvOferta[1, nRow].Value = P2.ToString("N2"); // Cantidad ofertada
                    dgvOferta[2, nRow].Value = rcd["FechaDesde"].ToString();
                    dgvOferta[3, nRow].Value = rcd["FechaFin"].ToString(); // Cambié el 4 por 3
                    dgvOferta[4, nRow].Value = P3.ToString("N2");           // Cambié el 5 por 4
                    dgvOferta[5, nRow].Value = P4.ToString("N2");           // Cambié el 6 por 5
                }
            }
        }

        private void AgregarLinea()
        {
            if (!ValidarAntesDeAgregarLinea())
                return;

            dgv.Rows.Add();
            int nRow = dgv.Rows.Count - 1;

            dgv[0, nRow].Value = txtSucursal.Text;
            dgv[1, nRow].Value = lblSucursal.Text;
            dgv[2, nRow].Value = txtCantidad.Text;
        }

        private void ContarCantidaddes()
        {
            double _nCant = 0;
            double _xCant = 0;

            lblExistencia.Text = "";

            foreach (DataGridViewRow cRow in dgv.Rows)
            {
                if (cRow.Cells[2].Value != null && cRow.Cells[2].Value.ToString() != "")
                {
                    _xCant = Convert.ToDouble(cRow.Cells[2].Value); // ← usar cRow, no CurrentRow
                    _nCant += _xCant;
                }
            }

            lblExistencia.Text = _nCant.ToString("N2"); // ← mostrar total
        }

        private void LimpiarDetalle()
        {
            lblExistencia.Text = "";
            txtSucursal.Clear();
            txtCantidad.Clear();
        }

        private bool ValidarAntesDeAgregarLinea()
        {
            if (!string.IsNullOrWhiteSpace(txtSucursal.Text))
            {
                lblSucursal.Text = Busco.BuscarSucursal(txtSucursal.Text, out bool found);

                if (!found)
                {
                    MostrarMensaje_Maestro("sucursal / almacen");
                    LimpiarDetalle();
                    return false;
                }
            }
            return true;
        }

        private bool ValidarMaestros()
        {
            if (!string.IsNullOrWhiteSpace(txtSuplidor.Text))
            {
                // 1. Declaramos las variables para recibir la info
                string _id = txtSuplidor.Text;
                string _nombre = "";

                // 2. Llamamos al método usando 'ref'
                Busco.BuscarSuplidor(ref _id, ref _nombre);

                // 3. Validamos si encontró algo (si el nombre no volvió vacío)
                if (string.IsNullOrEmpty(_nombre))
                {
                    MostrarMensaje_Maestro("suplidor / proveedor");
                    return false;
                }
                else
                {
                    lblSuplidor.Text = _nombre;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                // 1. Preparamos las variables (sobres vacíos)
                string _idCat = txtCategoria.Text;
                string _nomCat = "";

                // 2. Llamamos al método con 'ref' para que las llene
                Busco.BuscarCategoria(ref _idCat, ref _nomCat);

                // 3. Si el nombre regresó vacío, es que no existe esa categoría
                if (string.IsNullOrEmpty(_nomCat))
                {
                    MostrarMensaje_Maestro("categoria");
                    return false;
                }
                else
                {
                    // 4. Si todo está bien, mostramos el nombre en el label
                    lblCategoria.Text = _nomCat;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                if (!string.IsNullOrWhiteSpace(txtSubcategoria.Text))
                {
                    // 1. Preparamos las variables
                    string _idSub = txtSubcategoria.Text;
                    string _nomSub = "";
                    string _idCat = txtCategoria.Text; // La categoría padre
                    bool found = false;

                    // 2. Llamamos al método. 
                    // Nota: Pasamos el ID de categoría para que solo busque subcategorías de esa familia
                    Busco.BuscarSubCategoria(ref _idSub, ref _nomSub, _idCat);

                    // 3. Verificamos si regresó un nombre
                    if (string.IsNullOrEmpty(_nomSub))
                    {
                        MostrarMensaje_Maestro("Subcategoria");
                        return false;
                    }
                    else
                    {
                        lblSubcategoria.Text = _nomSub;
                    }
                }
            }
            return true;
        }

        private void MostrarMensaje_Maestro(string campo)
        {
            MessageBox.Show($"{campo} no existe", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtArticulo.Text))
            {
                MostrarMensaje("Articulo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("Nombre");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCosto.Text))
            {
                MostrarMensaje("Correo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPorciento.Text))
            {
                MostrarMensaje("% Impuesto");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtBarCode.Text))
            {
                MostrarMensaje("BarCode");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtReorden.Text))
            {
                MostrarMensaje("Reorden");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrecioA.Text))
            {
                MostrarMensaje("Precio A");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecioB.Text))
            {
                MostrarMensaje("Precio B");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecioC.Text))
            {
                MostrarMensaje("Precio C");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecioD.Text))
            {
                MostrarMensaje("Precio D");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPrecioE.Text))
            {
                MostrarMensaje("Precio E");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSuplidor.Text))
            {
                MostrarMensaje("Suplidor");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MostrarMensaje("Categoria");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSubcategoria.Text))
            {
                MostrarMensaje("Subcategoria");
                return false;
            }

            return true;
        }

        private void MostrarMensaje(string campo)
        {
            MessageBox.Show($"Falta el {campo}", "Miapp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ActualizaSecuenciaArticulo(string Id, string articulo)
        {
            string query = @"UPDATE SECUENCIAS SET CONTADOR = @A2 WHERE ID = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", Id);
                cmd.Parameters.AddWithValue("@A2", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void InsertarArticulo()
        {
            // 1. CORRECCIÓN DE IMAGEN: Evitamos el error .imagenDefinida
            if (picProducto.Image == null)
            {
                if (System.IO.File.Exists(ruta))
                {
                    picProducto.Image = Image.FromFile(ruta);
                }
                else
                {
                    // Si no hay imagen seleccionada ni imagen por defecto, 
                    // creamos un mapa de bits vacío para que no explote al convertir
                    picProducto.Image = new Bitmap(100, 100);
                }
            }

            // 2. CONVERSIÓN: Asegúrate que en ConvertImage el método se llame ImageToByteArray
            byte[] byteArray = ConvertImage.ImageToByteArray(picProducto.Image);

            string query = @"INSERT INTO PRODUCTOS ( IDPRODUCTO, DESCRIPCION, COSTO, PRECIOA, PRECIOB, PRECIOC, PRECIOD, PRECIOE,
                                         BARCODE, REORDEN, ESTATUS, FOTO, STOCK, IMPUESTO  )
                     VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8, @A9, @A10, @A11, @A12, @A13, @A14)";

            // ... el resto de tu código del using SqlConnection está perfecto ...

            using (SqlConnection cxn =  new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);

                cmd.Parameters.AddWithValue("@A1", txtArticulo.Text);
                cmd.Parameters.AddWithValue("@A2", txtNombre.Text);

                cmd.Parameters.AddWithValue("@A3", ToDoubleSeguroText(txtCosto.Text));
                cmd.Parameters.AddWithValue("@A4", ToDoubleSeguroText(txtPrecioA.Text));
                cmd.Parameters.AddWithValue("@A5", ToDoubleSeguroText(txtPrecioB.Text));
                cmd.Parameters.AddWithValue("@A6", ToDoubleSeguroText(txtPrecioC.Text));
                cmd.Parameters.AddWithValue("@A7", ToDoubleSeguroText(txtPrecioD.Text));
                cmd.Parameters.AddWithValue("@A8", ToDoubleSeguroText(txtPrecioE.Text));

                cmd.Parameters.AddWithValue("@A9", txtBarCode.Text);  // si no es numérico no uses Convert
                cmd.Parameters.AddWithValue("@A10", ToDoubleSeguroText(txtReorden.Text));

                cmd.Parameters.AddWithValue("@A11", "1");
                cmd.Parameters.AddWithValue("@A12", byteArray);
                cmd.Parameters.AddWithValue("@A13", ToDoubleSeguroText(lblExistencia.Text));
                cmd.Parameters.AddWithValue("@A14", ToDoubleSeguroText(txtPorciento.Text));

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void ActualizarArticulo()
        {
            // --- CAMBIO AQUÍ ---
            if (picProducto.Image == null)
            {
                // Verificamos si el archivo físico existe antes de intentar cargarlo
                if (System.IO.File.Exists(ruta))
                {
                    picProducto.Image = Image.FromFile(ruta);
                }
                else
                {
                    // Si no existe, creamos un cuadro blanco temporal
                    picProducto.Image = new Bitmap(100, 100);
                }
            }
            // -------------------

            byte[] byteArray = ConvertImage.ImageToByteArray(picProducto.Image);

            string query = @"UPDATE PRODUCTOS
                     SET DESCRIPCION = @A2 ,
                     // ... el resto de tu código sigue igual ...
                                 COSTO = @A3,
                                 PRECIOA = @A4,
                                 PRECIOB = @A5,
                                 PRECIOC = @A6,
                                 PRECIOD = @A7,
                                 PRECIOE = @A8,
                                 REORDEN = @A9,
                                 BARCODE = @A10,
                                 IMPUESTO = @A11,
                                 FOTO = @A12,
                                 STOCK = @A13
                             WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(query, cxn);

                cmd.Parameters.AddWithValue("@A1", txtArticulo.Text);
                cmd.Parameters.AddWithValue("@A2", txtNombre.Text);
                cmd.Parameters.AddWithValue("@A3", ToDoubleSeguroText(txtCosto.Text));
                cmd.Parameters.AddWithValue("@A4", ToDoubleSeguroText(txtPrecioA.Text));
                cmd.Parameters.AddWithValue("@A5", ToDoubleSeguroText(txtPrecioB.Text));
                cmd.Parameters.AddWithValue("@A6", ToDoubleSeguroText(txtPrecioC.Text));
                cmd.Parameters.AddWithValue("@A7", ToDoubleSeguroText(txtPrecioD.Text));
                cmd.Parameters.AddWithValue("@A8", ToDoubleSeguroText(txtPrecioE.Text));
                cmd.Parameters.AddWithValue("@A9", ToDoubleSeguroText(txtReorden.Text));
                cmd.Parameters.AddWithValue("@A10", txtBarCode.Text);  // si no es numérico no uses Convert
                cmd.Parameters.AddWithValue("@A11", ToDoubleSeguroText(txtPorciento.Text));
                cmd.Parameters.AddWithValue("@A12", byteArray);
                cmd.Parameters.AddWithValue("@A13", ToDoubleSeguroText(lblExistencia.Text));
                
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void BorrarRegistro(string articulo)
        {
            string query = @"UPDATE PRODUCTOS SET ESTATUS = 0 WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A2", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void BorrarUnaLineaDgv()
        {
            int cuantaSon = Convert.ToInt32(dgv.RowCount);

            if (cuantaSon == 1)
                dgv.Rows.RemoveAt(dgv.RowCount - 1);
            else
                dgv.Rows.Remove(dgv.CurrentRow);
        }

        // ----------------------------------------------
        // SE ASEGURA QUE NO ESTE VACIO
        // ----------------------------------------------
        private double ToDoubleSeguro(object value)
        {
            if (value == DBNull.Value || value == null)
                return 0;

            string s = value.ToString().Trim();

            if (string.IsNullOrEmpty(s))
                return 0;

            double result;

            if (double.TryParse(s, out result))
                return result;

            return 0; // si no se puede convertir, también devuelve 0
        }

        private double ToDoubleSeguroText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            double result;
            if (double.TryParse(value, out result))
                return result;

            return 0; // si no se pudo convertir, retorna 0
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.Graphics.DrawImage(picBarcode.Image, 0, 0);
            }
            catch
            {

            }
        }

        private void BorrarExistencia_OFER(string articulo)
        {
            string queri;

            queri = @"DELETE FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(queri, cxn);
                cmd.Parameters.AddWithValue("@A1", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void Borrar_Stock(string articulo)
        {
            string queri;

            queri = @"DELETE FROM ALMACEN_STOCK WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(queri, cxn);
                cmd.Parameters.AddWithValue("@A1", articulo);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        private void GuardarInventario(string articulo, string reorden)
        {
            string queri;

            Borrar_Stock(articulo);

            foreach (DataGridViewRow row in dgv.Rows)
            {
                queri = "";

                suc = "0";
                can = "0";

                if (row.Cells[0].Value.ToString() != "") suc = row.Cells[0].Value.ToString();
                if (row.Cells[2].Value.ToString() != "") can = row.Cells[2].Value.ToString();

                queri = @"INSERT INTO ALMACEN_STOCK (IDPRODUCTO, IDALMACEN, STOCK, REORDEN) VALUES (@A1, @A2, @A3, @A4)";

                using (SqlConnection cxn = new SqlConnection(cnn.db))
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(queri, cxn);
                    cmd.Parameters.AddWithValue("@A1", articulo);
                    cmd.Parameters.AddWithValue("@A2", suc);
                    cmd.Parameters.AddWithValue("@A3", can);
                    cmd.Parameters.AddWithValue("@A4", reorden);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }

        private void GuardarLasOfertas(string articulo)
        {
            string queri;

            BorrarExistencia_OFER(articulo);

            foreach (DataGridViewRow row in dgvOFER.Rows) 
            {
                queri = "";

                oCC = "0";
                cOF = "0";
                fDE = "0";
                fFN = "0";
                pOF = "0";
                Des = "0";

                if (row.Cells[0].Value.ToString() != "") oCC = row.Cells[0].Value.ToString();
                if (row.Cells[1].Value.ToString() != "") cOF = row.Cells[1].Value.ToString();
                if (row.Cells[2].Value.ToString() != "") fDE = row.Cells[2].Value.ToString();
                if (row.Cells[3].Value.ToString() != "") fFN = row.Cells[3].Value.ToString();
                if (row.Cells[4].Value.ToString() != "") pOF = row.Cells[4].Value.ToString();
                if (row.Cells[5].Value.ToString() != "") Des = row.Cells[5].Value.ToString();

                queri = @"INSERT INTO ARTICULOS_OFERTAS (IDPRODUCTO, OfertaCantidadPorCompra, CantidadOfertada,
                            FechaDesde, FechaFin, PrecioOferta, Descuento) VALUES (@A1, @A2, @A3, @A4, @A5, @A6, @A7)";

                using (SqlConnection cxn = new SqlConnection(cnn.db))
                {
                    cxn.Open();
                    SqlCommand cmd = new SqlCommand(queri, cxn);
                    cmd.Parameters.AddWithValue("@A1", articulo);
                    cmd.Parameters.AddWithValue("@A2", oCC);
                    cmd.Parameters.AddWithValue("@A3", cOF);
                    cmd.Parameters.AddWithValue("@A4", fDE);
                    cmd.Parameters.AddWithValue("@A5", fFN);
                    cmd.Parameters.AddWithValue("@A6", pOF);
                    cmd.Parameters.AddWithValue("@A7", Des);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();  
                }
            }
        }

        // --------------------------------------------------------------
        // OFERTAS
        // --------------------------------------------------------------
        private void txtOfetaCantPorCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtOfetaCantPorCompra.Text.Trim() != string.Empty)
                {
                    txtCantidadOfertada.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtOfetaCantPorCompra.Text.Contains('.')
                && !txtOfetaCantPorCompra.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtOfetaCantPorCompra_Leave(object sender, EventArgs e)
        {
            string texto = txtOfetaCantPorCompra.Text;
            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOfetaCantPorCompra.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtOfetaCantPorCompra.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtCantidadOfertada_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidadOfertada.Text.Trim() != string.Empty)
                {
                    txtFechaInicio.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtCantidadOfertada.Text.Contains('.')
                && !txtCantidadOfertada.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtCantidadOfertada_Leave(object sender, EventArgs e)
        {
            string texto = txtCantidadOfertada.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidadOfertada.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtCantidadOfertada.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtFechaInicio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si presiona Enter
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtFechaInicio.Text.Trim() != string.Empty)
                {
                    txtFechaFin.Focus();
                }
                return;
            }

            // Permite dígitos y teclas de control (borrar, retroceso, etc.)
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                return;
            }

            // Permite punto o coma decimal (solo uno)
            if ((e.KeyChar == '.' || e.KeyChar == ',')
                && !txtFechaInicio.Text.Contains('.')
                && !txtFechaInicio.Text.Contains(','))
            {
                return;
            }

            // Si no cumple nada de lo anterior, se bloquea la tecla
            e.Handled = true;
        }

        private void txtFechaInicio_Leave(object sender, EventArgs e)
        {
            string texto = txtFechaInicio.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // Validar que sean exactamente 8 dígitos (sin contar puntos o comas)
            string soloDigitos = new string(texto.Where(char.IsDigit).ToArray());

            if (soloDigitos.Length != 8)
            {
                MessageBox.Show("Debe tener exactamente 8 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaInicio.Focus();
                txtFechaInicio.SelectAll();
                return;
            }

            // Extraer partes
            int dia = int.Parse(soloDigitos.Substring(0, 2));   // posiciones 1-2
            int mes = int.Parse(soloDigitos.Substring(2, 2));   // posiciones 3-4

            // Validar día y mes
            if (dia < 1 || dia > 31)
            {
                MessageBox.Show("El día debe estar entre 01 y 31.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaInicio.Focus();
                txtFechaInicio.SelectAll();
                return;
            }

            if (mes < 1 || mes > 12)
            {
                MessageBox.Show("El mes debe estar entre 01 y 12.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaInicio.Focus();
                txtFechaInicio.SelectAll();
                return;
            }

            // Si todo está bien, continúa...
        }

        private void txtFechaFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si presiona Enter
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtFechaFin.Text.Trim() != string.Empty)
                {
                    txtPrecioOferta.Focus();
                }
                return;
            }

            // Permite dígitos y teclas de control (borrar, retroceso, etc.)
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                return;
            }

            // Permite punto o coma decimal (solo uno)
            if ((e.KeyChar == '.' || e.KeyChar == ',')
                && !txtFechaFin.Text.Contains('.')
                && !txtFechaFin.Text.Contains(','))
            {
                return;
            }

            // Si no cumple nada de lo anterior, se bloquea la tecla
            e.Handled = true;
        }

        private void txtFechaFin_Leave(object sender, EventArgs e)
        {
            string texto = txtFechaFin.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // Validar que sean exactamente 8 dígitos (sin contar puntos o comas)
            string soloDigitos = new string(texto.Where(char.IsDigit).ToArray());

            if (soloDigitos.Length != 8)
            {
                MessageBox.Show("Debe tener exactamente 8 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }

            // Extraer partes
            int dia = int.Parse(soloDigitos.Substring(0, 2));   // posiciones 1-2
            int mes = int.Parse(soloDigitos.Substring(2, 2));   // posiciones 3-4

            // Validar día y mes
            if (dia < 1 || dia > 31)
            {
                MessageBox.Show("El día debe estar entre 01 y 31.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }

            if (mes < 1 || mes > 12)
            {
                MessageBox.Show("El mes debe estar entre 01 y 12.", "QLab", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFechaFin.Focus();
                txtFechaFin.SelectAll();
                return;
            }

            // Si todo está bien, continúa...
        }

        private void txtPrecioOferta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioOferta.Text.Trim() != string.Empty)
                {
                    txtDescuentoOferta.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtPrecioOferta.Text.Contains('.')
                && !txtPrecioOferta.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtPrecioOferta_Leave(object sender, EventArgs e)
        {
            string texto = txtPrecioOferta.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecioOferta.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtPrecioOferta.Text = valor.ToString("N2"); // 123,123.00
        }

        private void txtDescuentoOferta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtDescuentoOferta.Text.Trim() != string.Empty)
                {
                    btnInsertaOferta.Focus();
                }
                return;
            }

            // Permite digitos, teclas de control y un punto decimal
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                // pertimido
                return;
            }

            // permite un solo punto o coma decimal, pero no ambos
            if ((e.KeyChar == '.'
                || e.KeyChar == ',')
                && !txtDescuentoOferta.Text.Contains('.')
                && !txtDescuentoOferta.Text.Contains(','))
            {
                return;
            }

            e.Handled = true;
        }

        private void txtDescuentoOferta_Leave(object sender, EventArgs e)
        {
            string texto = txtDescuentoOferta.Text.Trim();

            if (string.IsNullOrEmpty(texto))
                return;

            // reemplaza ',' por '.'
            string textoNormalizado = texto.Replace(',', '.');

            // Intentar convertir a decimal con punto como separador
            if (!decimal.TryParse(textoNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un número válido.", "ITLA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescuentoOferta.Focus();
                return;
            }

            // Muestra con 2 decimales, usa coma o punto según cultura local
            txtDescuentoOferta.Text = valor.ToString("N2"); // 123,123.00
        }

        private void dgvOFER_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnInsertaOferta.PerformClick();
        }

        private void btnInsertaOferta_Click(object sender, EventArgs e)
        {
            if (txtFechaInicio.Text.Trim() != "")
            {
                if (txtFechaFin.Text.Trim() != "")
                {
                    AgregarLineaOferta();

                    LimpiarDetalleOfertas();
                    txtOfetaCantPorCompra.Focus();
                }
            }
        }

        private void btnLimpiarOferta_Click(object sender, EventArgs e)
        {
            LimpiarDetalleOfertas();
            txtOfetaCantPorCompra.Focus();
        }

        private void btnBorrarLineaOferta_Click(object sender, EventArgs e)
        {
            BorrarUnaLineaDgvOFER();

            LimpiarDetalleOfertas();
            txtOfetaCantPorCompra.Focus();
        }

        private void btnEditaLineaOferta_Click(object sender, EventArgs e)
        {
            LimpiarDetalleOfertas();

            txtOfetaCantPorCompra.Text = dgvOFER.CurrentRow.Cells[0].Value.ToString();
            txtCantidadOfertada.Text = dgvOFER.CurrentRow.Cells[1].Value.ToString();
            txtFechaInicio.Text = dgvOFER.CurrentRow.Cells[2].Value.ToString();
            txtFechaFin.Text = dgvOFER.CurrentRow.Cells[3].Value.ToString();
            txtPrecioOferta.Text = dgvOFER.CurrentRow.Cells[4].Value.ToString();
            txtDescuentoOferta.Text = dgvOFER.CurrentRow.Cells[5].Value.ToString();

            BorrarUnaLineaDgvOFER();
            txtOfetaCantPorCompra.Focus();
        }


        private void AgregarLineaOferta()
        {
            dgvOFER.Rows.Add();
            int nRow = dgvOFER.Rows.Count - 1;

            dgvOFER[0, nRow].Value = txtOfetaCantPorCompra.Text;
            dgvOFER[1, nRow].Value = txtCantidadOfertada.Text;
            dgvOFER[2, nRow].Value = txtFechaInicio.Text;
            dgvOFER[3, nRow].Value = txtFechaFin.Text;
            dgvOFER[4, nRow].Value = txtPrecioOferta.Text;
            dgvOFER[5, nRow].Value = txtDescuentoOferta.Text;
        }

        private void LimpiarDetalleOfertas()
        {
            txtOfetaCantPorCompra.Clear();
            txtCantidadOfertada.Clear();
            txtFechaInicio.Clear();
            txtFechaFin.Clear();
            txtPrecioOferta.Clear();
            txtDescuentoOferta.Clear();
        }

        private void BorrarUnaLineaDgvOFER()
        {
            int cuantaSon = Convert.ToInt32(dgvOFER.RowCount);

            if (cuantaSon == 1)
                dgvOFER.Rows.RemoveAt(dgvOFER.RowCount - 1);
            else
                dgvOFER.Rows.Remove(dgvOFER.CurrentRow);
        }


        // ----------------------------------------------------------------
        // MANEJO DE LAS CONSULTAS
        // ----------------------------------------------------------------
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
            btnBuscar.PerformClick();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarTodosLosArticulo(txtBuscar.Text);
        }

        private void btnLimpiarConsulta_Click(object sender, EventArgs e)
        {
            // limpiar el DataGridView
            this.dgvArt.Rows.Clear();
            this.dgvArt.Refresh();

            this.dgvStock.Rows.Clear();
            this.dgvStock.Refresh();

            this.dgvOferta.Rows.Clear();
            this.dgvOferta.Refresh();
        }

        private void btnEditarArticulo_Click(object sender, EventArgs e)
        {
            if (dgvArt.RowCount > 0) // verifica que el dataGridView tenga lineas  
            {
                txtArticulo.Text = dgvArt.CurrentRow.Cells[0].Value.ToString();

                BuscarArticulo(txtArticulo.Text);

                tabControl1.SelectedTab = tabPage2;
            }
        }

        private void dgvArt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvArt.RowCount > 0) // verifica que el dataGridView tenga lineas  
            {
                string articulo = dgvArt.CurrentRow.Cells[0].Value.ToString();

                MostrarExistenciasSucursales(articulo);
                ConsultarLasOfertas(articulo);
            }
        }

        private void dgvArt_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEditarArticulo.PerformClick();
        }

        private void MostrarExistenciasSucursales(string _articulo)
        {
            this.dgvStock.Rows.Clear();
            this.dgvStock.Refresh();

            string query = @"SELECT B.IDSUCURSAL, B.NOMBREDESUCURSAL, A.STOCK 
                             FROM ALMACENSTOCK A 
                             LEFT JOIN SUCURSALES B ON B.IDSUCURSAL = A.IDALMACEN 
                             WHERE A.IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read())
                {
                    dgvStock.Rows.Add();
                    int nRow = dgvStock.Rows.Count - 1;

                    double P1 = ToDoubleSeguro(rcd["STOCK"]);

                    dgvStock[0, nRow].Value = rcd["IDSUCURSAL"].ToString();
                    dgvStock[1, nRow].Value = rcd["NOMBREDESUCURSAL"].ToString();
                    dgvStock[2, nRow].Value = P1.ToString("N2");
                }
            }
        }

        private void ConsultarLasOfertas(string _articulo)
        {
            this.dgvOferta.Rows.Clear();
            this.dgvOferta.Refresh();

            string query = @"SELECT * FROM ARTICULOS_OFERTAS WHERE IDPRODUCTO = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                SqlDataReader rcd = cmd.ExecuteReader();

                while (rcd.Read())
                {
                    dgvOferta.Rows.Add();
                    int nRow = dgvOferta.Rows.Count - 1;

                    double P1 = ToDoubleSeguro(rcd["OfertaCantPorCompra"]);
                    double P2 = ToDoubleSeguro(rcd["CantidadOfertada"]);
                    double P3 = ToDoubleSeguro(rcd["PrecioOferta"]);
                    double P4 = ToDoubleSeguro(rcd["Descuento"]);

                    dgvOferta[0, nRow].Value = P1.ToString("N2"); // Cantidad compra
                    dgvOferta[1, nRow].Value = P2.ToString("N2"); // Cantidad ofertada
                    dgvOferta[2, nRow].Value = rcd["FechaDesde"].ToString();
                    dgvOferta[3, nRow].Value = rcd["FechaFin"].ToString(); // Cambié el 4 por 3
                    dgvOferta[4, nRow].Value = P3.ToString("N2");           // Cambié el 5 por 4
                    dgvOferta[5, nRow].Value = P4.ToString("N2");           // Cambié el 6 por 5
                }
            }
        }

        private void BuscarTodosLosArticulo(string buscar)
        {
            // limpiar el DataGridView
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            string _query =
                @"SELECT A.IDPRODUCTO, A.DESCRIPCION, A.COSTO, A.PRECIOA, A.PRECIOB, A.PRECIOC, A.PRECIOD, 
                         A.PRECIOE, B.CategoriaNombre, A.Stock
                  FROM PRODUCTOS A LEFT JOIN Categorias B ON A.Categoria = B.categoria 
                 WHERE A.ESTATUS = 1 AND A.DESCRIPCION LIKE '%' + @A1 + '%' ORDER BY A.DESCRIPCION ASC";

            SqlConnection cxn = new SqlConnection(cnn.db);  cxn.Open();  
            SqlCommand cmd = new SqlCommand(_query, cxn);
            cmd.Parameters.AddWithValue("@A1", buscar);

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgvArt.Rows.Add();  
                int nRow = dgvArt.Rows.Count - 1;

                double P1 = ToDoubleSeguro(rdr["COSTO"]);
                double P2 = ToDoubleSeguro(rdr["PRECIOA"]);
                double P3 = ToDoubleSeguro(rdr["PRECIOB"]);
                double P4 = ToDoubleSeguro(rdr["PRECIOC"]);
                double P5 = ToDoubleSeguro(rdr["PRECIOD"]);
                double P6 = ToDoubleSeguro(rdr["PRECIOE"]);
                double P7 = ToDoubleSeguro(rdr["Stock"]);

                dgvArt[0, nRow].Value = rdr["IDPRODUCTO"].ToString();
                dgvArt[1, nRow].Value = rdr["DESCRIPCION"].ToString();
                dgvArt[2, nRow].Value = P1.ToString("N2");
                dgvArt[3, nRow].Value = P2.ToString("N2");
                dgvArt[4, nRow].Value = P3.ToString("N2");
                dgvArt[5, nRow].Value = P4.ToString("N2");
                dgvArt[6, nRow].Value = P5.ToString("N2");
                dgvArt[7, nRow].Value = P6.ToString("N2");
                dgvArt[8, nRow].Value = rdr["CategoriaNombre"].ToString();
                dgvArt[9, nRow].Value = P7.ToString("N2");
            }

            while (rdr.Read())
            {
                dgvArt.Rows.Add();
                int nRow = dgvArt.Rows.Count - 1;

                double P1 = ToDoubleSeguro(rdr["COSTO"]);
                double P2 = ToDoubleSeguro(rdr["PRECIOA"]);
                double P3 = ToDoubleSeguro(rdr["PRECIOB"]);
                double P4 = ToDoubleSeguro(rdr["PRECIOC"]);
                double P5 = ToDoubleSeguro(rdr["PRECIOD"]);
                double P6 = ToDoubleSeguro(rdr["PRECIOE"]);
                double P7 = ToDoubleSeguro(rdr["Stock"]);

                dgvArt[0, nRow].Value = rdr["IDPRODUCTO"]?.ToString();
                dgvArt[1, nRow].Value = rdr["DESCRIPCION"]?.ToString();
                dgvArt[2, nRow].Value = P1.ToString("N2");
                dgvArt[3, nRow].Value = P2.ToString("N2");
                dgvArt[4, nRow].Value = P3.ToString("N2");
                dgvArt[5, nRow].Value = P4.ToString("N2");
                dgvArt[6, nRow].Value = P5.ToString("N2");
                dgvArt[7, nRow].Value = P6.ToString("N2");
                dgvArt[8, nRow].Value = rdr["CategoriaNombre"]?.ToString();
                dgvArt[9, nRow].Value = P7.ToString("N2");
            }

            cmd.Dispose();
            cxn.Close();

            // ----------------------------
            // Llenar las grillas faltantes
            // ----------------------------
            if (dgvArt.RowCount > 0)
            {
                string art = dgvArt.Rows[0].Cells[0].Value?.ToString();

                MostrarExistenciasSucursales(art);
                ConsultarLasOfertas(art);
            }

        }

        private void txtCategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCategoria.Text.Trim() != string.Empty)
                {
                    txtSubcategoria.Focus();
                }
            }
        }

        private void txtCategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F4)
                btnCategoria.PerformClick();
        }

        private void txtCategoria_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                // 1. Declaramos variables para recibir la respuesta
                string _id = txtCategoria.Text;
                string _nombre = "";

                // 2. Llamamos al método con ref (como pide el error CS1620)
                Busco.BuscarCategoria(ref _id, ref _nombre);

                // 3. Validamos: si el nombre volvió vacío, es que no existe
                if (string.IsNullOrEmpty(_nombre))
                {
                    MessageBox.Show("La categoria ingresada no existe.", "Categoria no encontrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCategoria.Clear();
                    lblCategoria.Text = "";
                    txtCategoria.Focus();
                }
                else
                {
                    lblCategoria.Text = _nombre;
                }
            }
        }

        private void btnCategoria_Click(object sender, EventArgs e)
        {
            frmVENCAT frm = new frmVENCAT();
            frm.ShowDialog();

            if (frm.EData == true)
            {
                txtCategoria.Text = frm.var1;
                lblCategoria.Text = frm.var2;
            }
        }

        private void txtSubcategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtSubcategoria.Text.Trim() != string.Empty)
                {
                    txtSucursal.Focus();
                }
            }
        }

        private void txtSubcategoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
                btnSubcategoria.PerformClick();
        }

        private void txtSubcategoria_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                if (!string.IsNullOrWhiteSpace(txtSubcategoria.Text))
                {
                    // 1. Declaramos las variables necesarias
                    string _idCat = txtCategoria.Text;
                    string _idSub = txtSubcategoria.Text;
                    string _nomSub = "";

                    // 2. Llamamos al método usando 'ref' para el ID y el Nombre
                    // Pasamos el ID de la categoría padre para filtrar bien
                    Busco.BuscarSubCategoria(ref _idSub, ref _nomSub, _idCat);

                    // 3. Si el nombre regresó vacío, es que no existe
                    if (string.IsNullOrEmpty(_nomSub))
                    {
                        MessageBox.Show("La subcategoria ingresada no existe.", "Subcategoria no encontrada",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        txtSubcategoria.Clear(); // Corregido: Limpiamos la sub, no la categoría padre
                        lblSubcategoria.Text = "";
                        txtSubcategoria.Focus();
                    }
                    else
                    {
                        // 4. Si la encontró, mostramos el nombre
                        lblSubcategoria.Text = _nomSub;
                    }
                }
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        private void btnSubcategoria_Click(object sender, EventArgs e)
        {
            if (txtCategoria.Text.Trim() != string.Empty)
            {
                frmVENSUBCAT frm = new frmVENSUBCAT(txtCategoria.Text);
                frm.ShowDialog();

                if (frm.EData == true)
                {
                    txtSubcategoria.Text = frm.var1;
                    lblSubcategoria.Text = frm.var2;
                }
            }
            else
            {
                MessageBox.Show("Debe ingresar o seleccionar una categoría antes de elegir una subcategoría.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoria.Focus();
            }
        }
    }
}
