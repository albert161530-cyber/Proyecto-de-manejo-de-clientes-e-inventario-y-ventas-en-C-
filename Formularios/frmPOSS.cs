using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PLUNES
{
    public partial class frmPOSS : Form
    {
        int _almacen = 0;
        public frmPOSS()
        {
            InitializeComponent();
            PropiedadesParaDgv();
            PropiedadesFlowLayOut();

            CargarCategoria();
            CargarArticulosFavoritos();
        }

        double _impuesto;
        string _CodigoArticulo = "";
        string _Descripcion = "";
        string _Precio = "";
        bool _autorizado;

        string _MetodoPago = "";
        string _NCFsecuencia = "";
        string _NCFtipo = "";

        double ft_Subtotal;
        double ft_Impuesto;
        double ft_Total;

        private Bitmap qrImagenGlobal;

        private void frmPOSS_Load(object sender, EventArgs e)
        {
            // REINICIA CONTADOR 
            // DBCC CHECKIDENT('VENTAS', RESEED, 999);

            this.KeyPreview = true;
            this.Text = "POSS";

            _autorizado = false;
            _impuesto = 0;

            _MetodoPago = "";
            _NCFsecuencia = "";
            _NCFtipo = "";

            // lblUsuario.Text = cnn.miUsuario;
            cnn.miCteContado = Busco.ClienteAlContado("1");

            PrintDocument pd = new PrintDocument();
        }

        private void frmPOSS_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: btnF1.PerformClick(); break;
                case Keys.F2: btnF2.PerformClick(); break;
                case Keys.F3: btnF3.PerformClick(); break;
                case Keys.F4: btnF4.PerformClick(); break;
                case Keys.F5: btnF5.PerformClick(); break;
                case Keys.F6: btnF6.PerformClick(); break;
                case Keys.F7: btnF7.PerformClick(); break;
                case Keys.F8: btnF8.PerformClick(); break;
                case Keys.F9: btnF9.PerformClick(); break;
                case Keys.F10: btnF10.PerformClick(); break;
                case Keys.Escape: this.Close(); break;
            }

            e.Handled = true;
        }

        // -----------------------------------------------
        //
        // -----------------------------------------------
        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Avanza(e, txtCantidad, txtProducto);
        }

        private void txtProducto_Leave(object sender, EventArgs e)
        {
            if (txtProducto.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtProducto.Text); // metodo para buscar el articulo en la tabla
            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnF4.PerformClick();
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Avanza(e, txtProducto, txtCantidad);
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (txtCantidad.Text.Trim() != string.Empty)
            {
                InsertaLinea();
                UpdateTotales();
            }
        }

        // -----------------------------------------------
        // BOTONES
        // -----------------------------------------------
        private void btnF1_Click(object sender, EventArgs e)
        {
            txtCantidad.Focus();
        }

        private void btnF2_Click(object sender, EventArgs e)
        {
            frmPagos frmPagos = new frmPagos(lblSubTotal.Text, lblImpuesto.Text, lblTotalPagar.Text);
            frmPagos.ShowDialog();

            if (frmPagos.EPago == true)
            {
                _MetodoPago = "EFECTIVO";
                lblPago.Text = Convert.ToDecimal(frmPagos.vtaPago).ToString("F2");
                lblDevuelta.Text = Convert.ToDecimal(frmPagos.vtaDevuelta).ToString("F2");
            }
        }

        private void btnF3_Click(object sender, EventArgs e)
        {
            frmVerifica frmVerifica = new frmVerifica();
            frmVerifica.ShowDialog();
            txtProducto.Focus();
        }

        private void btnF4_Click(object sender, EventArgs e)
        {
            frmVENART frmVENART = new frmVENART();
            frmVENART.ShowDialog();

            if (frmVENART.EData == true)
            {
                txtProducto.Text = frmVENART.var1;
                txtCantidad.Text = "1";

                _Descripcion = frmVENART.var2;
                _Precio = frmVENART.var3;

                // busca el impuesto para el articulo
                _impuesto = BuscarImpuesto(txtProducto.Text, _almacen);

                // inserta la linea en la cuadricula
                InsertaLinea();

                txtCantidad.Text = "";
                txtProducto.Text = "";
            }
        }

        private void btnF5_Click(object sender, EventArgs e)
        {
            SeleccionaCboCategorias();
        }

        private void btnF6_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnF7_Click(object sender, EventArgs e)
        {
            frmAutoriza frmAutoriza = new frmAutoriza();
            frmAutoriza.ShowDialog();

            if (frmAutoriza.EData == true)
            {
                _autorizado = true;

                // --- AGREGA ESTO PARA QUE ABRA LA VENTANA ---
                frmProducto maestro = new frmProducto();
                maestro.ShowDialog();
                // --------------------------------------------
            }
        }

        private void btnF8_Click(object sender, EventArgs e)
        {
            if (lblTotalPagar.Text.Trim() != string.Empty)
            {
                if (lblPago.Text.Trim() != string.Empty)
                {
                    Vender();
                }
            }
        }

        private void btnF9_Click(object sender, EventArgs e)
        {
            frmRNCCTE frmRNCCTE = new frmRNCCTE();
            frmRNCCTE.ShowDialog();

            if (frmRNCCTE.ERnc == true)
            {
                lblRNCcliente.Text = frmRNCCTE.varRNC;
                lblRNC_ClienteNombre.Text = frmRNCCTE.varRNCName;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_autorizado == false)
                return;

            if (dgv.RowCount > 0) 
            {
                _autorizado = false;

                txtProducto.Text = dgv.CurrentRow.Cells[4].Value.ToString();
                txtCantidad.Text = dgv.CurrentRow.Cells[1].Value.ToString();

                BorrarLinea();
                UpdateTotales();
            }
        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (_autorizado == false)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                if (dgv.CurrentRow != null && dgv.RowCount > 0)
                {
                    _autorizado = false;

                    BorrarLinea();
                    UpdateTotales();
                }
            }
        }

        // -----------------------------------------------
        // METODOS
        // -----------------------------------------------

        private void Avanza(KeyPressEventArgs e, Control siguiente, TextBox actual)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (!string.IsNullOrEmpty(actual.Text))
                {
                    siguiente?.Focus();
                }
            }
        }

        private void InsertaLinea()
        {
            if (string.IsNullOrEmpty(_Descripcion)) return;

            dgv.Rows.Add(); // agrega una linea al datagridview
            int nrow = dgv.Rows.Count - 1; // restamos uno al contador del dgv

            double _TotalLinea = CalcularLinea(txtCantidad.Text, _Precio);

            dgv[0, nrow].Value = _Descripcion;
            dgv[1, nrow].Value = txtCantidad.Text;
            dgv[2, nrow].Value = _Precio;
            dgv[3, nrow].Value = _TotalLinea;
            dgv[4, nrow].Value = txtProducto.Text;
            dgv[5, nrow].Value = _impuesto;         //  impuesto

            UpdateTotales();
        }

        private void UpdateTotales()
        {
            ft_Subtotal = 0;
            ft_Impuesto = 0;
            ft_Total = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                double subtotalLinea = 0;
                double tasaLinea = 0;

                double.TryParse(row.Cells["D"].Value.ToString(), out subtotalLinea);
                double.TryParse(row.Cells["F"].Value.ToString(), out tasaLinea);

                double impuestoCalculado = Math.Round(subtotalLinea * tasaLinea, 2);

                ft_Subtotal += subtotalLinea;
                ft_Impuesto += impuestoCalculado;
            }

            ft_Total = ft_Subtotal + ft_Impuesto;

            lblSubTotal.Text = ft_Subtotal.ToString("N2");
            lblImpuesto.Text = ft_Impuesto.ToString("N2");
            lblTotalPagar.Text = ft_Total.ToString("N2");
        }

        private void BorrarLinea()
        {
            int CuantasLineasTengo = Convert.ToInt32(dgv.RowCount);

            if (CuantasLineasTengo == 1)
            {
                dgv.Rows.RemoveAt(dgv.RowCount - 1); // se utiliza cuando te queda una sola linea
            }
            else
            {
                dgv.Rows.Remove(dgv.CurrentRow);  // utilizada cuando tienes varias linea
            }
        }

        // 1. Agregamos el parámetro "_almacen" aquí
        private double BuscarImpuesto(string _articulo, int _almacen)
        {
            // Cambiamos ProductoId por IDPRODUCTO y BarCode por BARCODE
            // Eliminamos AlmacenId porque no existe en tu tabla PRODUCTOS (según la captura de SQL)
            string _script = @"
        SELECT TOP 1 IMPUESTO FROM PRODUCTOS WHERE IDPRODUCTO = @A1
        UNION ALL 
        SELECT TOP 1 IMPUESTO FROM PRODUCTOS WHERE BARCODE = @A1";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(_script, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);

                SqlDataReader rcd = cmd.ExecuteReader();

                if (rcd.Read())
                {
                    // Verificamos que el valor no sea NULL antes de convertir
                    if (rcd["IMPUESTO"] != DBNull.Value)
                    {
                        return Convert.ToDouble(rcd["IMPUESTO"]);
                    }
                }
            }

            return 0;
        }
        private static double CalcularLinea(string _cant, string _prec)
        {
            double _ncant, _nprec;

            // convierte a un string a double, verifica que solo sean numeros
            double.TryParse(_cant, out _ncant);
            double.TryParse(_prec, out _nprec);
            return _ncant * _nprec;
        }

        private void BuscarArticulo(string _articulo)
        {
            _impuesto = 0;

            string script = @"
    SELECT TOP 1 T1.IdProducto,
                 T1.Descripcion,
                 T1.PrecioE,
                 T1.Impuesto,
                 T2.Cantidad
    FROM Productos T1
    INNER JOIN StockSucursal T2 ON T1.IDPRODUCTO = T2.PRODUCTOID
    WHERE T1.IDProducto = @A1 AND T2.SucursalID = @A2

    UNION ALL

    SELECT TOP 1 T1.IdProducto,
                 T1.Descripcion,
                 T1.PrecioE,
                 T1.Impuesto,
                 T2.Cantidad
    FROM Productos
    INNER JOIN StockSucursal T2 ON T1.IDPRODUCTO = T2.PRODUCTOID
    WHERE T1.BarCode = @A1 AND T2.SucursalID = @A2";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(script, cxn);
                cmd.Parameters.AddWithValue("@A1", _articulo);
                cmd.Parameters.AddWithValue("@A2", _almacen);
                SqlDataReader rcd = cmd.ExecuteReader();

                if (rcd.Read())
                {
                    double _stock = Convert.ToDouble(rcd["Cantidad"].ToString());
                    if (_stock > 0)
                    {
                        txtCantidad.Text = "1";
                        _CodigoArticulo = rcd["IDProducto"].ToString();
                        _Descripcion = rcd["Descripcion"].ToString();
                        _Precio = rcd["PrecioE"].ToString();

                        _impuesto = Convert.ToDouble(rcd["Impuesto"]);
                    }
                    else
                    {
                        MessageBox.Show("No tiene existencia este articulo", "POSS",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void SeleccionaCboCategorias()
        {
            int selectedCategoria = ((KeyValuePair<int, string>)cboCategorias.SelectedItem).Key;
            string nombre = txtBuscarNombre.Text;

            int? categoriaId = selectedCategoria == 0 ? (int?)null : selectedCategoria;

            string nombreCategoria = string.IsNullOrEmpty(nombre) ? null : nombre;

            CargarArticulosPorCategoria(categoriaId, nombreCategoria);
        }

        private void CargarArticulosPorCategoria(int? _categoriaId, string _nombreArticulo)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();

                StringBuilder query = new StringBuilder(
                    @"SELECT Descripcion,
                     PrecioE,
                     Foto,
                     IDproducto
              FROM Productos
              WHERE Estatus = '1'");

                List<SqlParameter> parametros = new List<SqlParameter>();

                if (_categoriaId.HasValue)
                {
                    query.Append(" AND Categoria = @CategoriaID");
                    parametros.Add(new SqlParameter("@CategoriaID", _categoriaId.Value));
                }

                if (!string.IsNullOrEmpty(_nombreArticulo))
                {
                    query.Append(" AND Descripcion LIKE @ProductoNombre");
                    parametros.Add(new SqlParameter("@ProductoNombre", "%" + _nombreArticulo + "%"));
                }

                using (SqlCommand cmd = new SqlCommand(query.ToString(), cxn))
                {
                    cmd.Parameters.AddRange(parametros.ToArray());
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        flowLayoutPanel1.Controls.Clear();

                        while (dr.Read())
                        {
                            UserControl_Articulo articulo = new UserControl_Articulo
                            {
                                NombreProducto = dr["Descripcion"].ToString(),
                                codigoArticulo = dr["IDProducto"].ToString(),
                                Precio = dr["PrecioE"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioE"]) : 0m,

                                Width = 165,
                                Height = 260
                            };

                            if (dr["Foto"] != DBNull.Value)
                            {
                                byte[] imagenByte = (byte[])dr["Foto"];

                                using (MemoryStream ms = new MemoryStream(imagenByte))
                                {
                                    articulo.ImagenProducto = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                articulo.ImagenProducto = SystemIcons.Application.ToBitmap();
                            }

                            // conectamos el evento para cuando seleccionen el articulo
                            articulo.ProductoSeleccionado += Producto_ProductoSeleccionado;

                            // lo agregamos al panel para que se muestre en pantalla
                            flowLayoutPanel1.Controls.Add(articulo);
                        }
                    }
                }
            }
        }

        private void CargarCategoria()
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string query =
                @"SELECT Categoria, CategoriaNombre
          FROM Categorias
          ORDER BY CategoriaNombre ASC";

                using (SqlCommand cmd = new SqlCommand(query, cxn))
                {
                    SqlDataReader rcd = cmd.ExecuteReader();

                    Dictionary<int, string> categorias = new Dictionary<int, string>();
                    categorias.Add(0, "Todas");

                    while (rcd.Read())
                    {
                        categorias.Add(Convert.ToInt32(rcd["Categoria"]), rcd["CategoriaNombre"].ToString());
                    }

                    cboCategorias.DataSource = new BindingSource(categorias, null);
                    cboCategorias.DisplayMember = "Value";
                    cboCategorias.ValueMember = "Key";
                }
            }
        }

        // -----------------------------------------------------------
        // dgv
        private void PropiedadesParaDgv()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("A", "Descripcion");
            this.dgv.Columns.Add("B", "Cantidad");
            this.dgv.Columns.Add("C", "Precio");
            this.dgv.Columns.Add("D", "Total Linea");
            this.dgv.Columns.Add("E", "Codigo");
            this.dgv.Columns.Add("F", "Impuesto");

            // Alinear header a la derecha
            dgv.Columns["C"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["D"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            DataGridViewColumn
            column = dgv.Columns[0]; column.Width = 320;
            column = dgv.Columns[1]; column.Width = 100;

            FormatoNumericoColumna(dgv, 2);
            FormatoNumericoColumna(dgv, 3);

            column = dgv.Columns[4]; column.Width = 0; column.Visible = false;
            column = dgv.Columns[5]; column.Width = 0; column.Visible = false;

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

        private void FormatoNumericoColumna(DataGridView dgv, int indiceColumna, int ancho = 150)
        {
            DataGridViewColumn column = dgv.Columns[indiceColumna];
            column.Width = ancho;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.DefaultCellStyle.Format = "F2"; // dos decimales sin coma
        }

        private void PropiedadesFlowLayOut()
        {
            // Estilo FlowLayoutPanel (catalogo)
            flowLayoutPanel1.BackColor = Color.White;
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle | BorderStyle.None;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Padding = new Padding(15);
        }

        private void CargarArticulosFavoritos()
        {
            string _script = @"
    SELECT IDProducto, Descripcion, PrecioE, Foto
    FROM Productos
    WHERE Estatus = 1
    AND Favorito = 1
    ORDER BY Descripcion ASC";

            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand(_script, cxn);
                SqlDataReader rcd = cmd.ExecuteReader();

                flowLayoutPanel1.Controls.Clear();

                while (rcd.Read())
                {
                    UserControl_Articulo producto = new UserControl_Articulo()
                    {
                        NombreProducto = rcd["Descripcion"].ToString(),
                        codigoArticulo = rcd["IDProducto"].ToString(),

                        Precio = rcd["PrecioE"] != DBNull.Value
                            ? Convert.ToDecimal(rcd["PrecioE"])
                            : 0m,

                        Width = 165,
                        Height = 260
                    };

                    if (rcd["Foto"] != DBNull.Value)
                    {
                        byte[] imagenByte = (byte[])rcd["Foto"];

                        using (MemoryStream ms = new MemoryStream(imagenByte))
                        {
                            producto.ImagenProducto = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        producto.ImagenProducto = SystemIcons.Application.ToBitmap();
                    }

                    // conectamos el evento para cuando seleccionen el articulo
                    producto.ProductoSeleccionado += Producto_ProductoSeleccionado;

                    // lo agregamos al panel para que se muestre en pantalla
                    flowLayoutPanel1.Controls.Add(producto);
                }
            }
        }

        // -------------------------------------------------------------------
        // este evento se dispara al presionar el boton AGREGAR en un articulo
        // -------------------------------------------------------------------
        private void Producto_ProductoSeleccionado(object sender, EventArgs e)
        {
            var producto = (UserControl_Articulo)sender;

            txtProducto.Text = producto.codigoArticulo;
            txtCantidad.Text = "1";

            _Descripcion = producto.NombreProducto;
            _Precio = producto.Precio.ToString();

            _impuesto = BuscarImpuesto(txtProducto.Text, _almacen);

            // inserta la linea en la cuadricula
            InsertaLinea();
        }

        private void Vender()
        {
            if (dgv.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow) == 0)
            {
                MessageBox.Show("No hay productos en la venta.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(cnn.db))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Aseguramos que los montos sean decimal (más preciso para dinero)
                        decimal subtotal = Convert.ToDecimal(ft_Subtotal);
                        decimal impuesto = Convert.ToDecimal(ft_Impuesto);
                        decimal total = Convert.ToDecimal(ft_Total);

                        // 2. CORRECCIÓN FACTURA: Si BuscaUltimoNumero falla o trae algo no numérico
                        string _factura = Busco.BuscaUltimoNumero("2");
                        if (string.IsNullOrWhiteSpace(_factura)) _factura = "1";

                        // Limpiamos cualquier carácter no numérico antes de convertir
                        string soloNumeros = new string(_factura.Where(char.IsDigit).ToArray());
                        long numFactura = long.Parse(soloNumeros);
                        string factura = numFactura.ToString("D10");

                        string fVenta = DateTime.Now.ToString("yyyyMMdd");

                        // INSERTAR VENTA PRINCIPAL
                        string insertVenta = @"
                    INSERT INTO Ventas (Fecha, Subtotal, Impuesto, Total, IDCLIENTE, MetodoPago, ClienteId, Factura, FechaVenta, UsuarioID) 
                    VALUES (GETDATE(), @subtotal, @impuesto, @total, @cte, @mtPago, @cteId, @vtaID, @fvta, @uid);
                    SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdVenta = new SqlCommand(insertVenta, conn, tran);
                        cmdVenta.Parameters.AddWithValue("@subtotal", subtotal);
                        cmdVenta.Parameters.AddWithValue("@impuesto", impuesto);
                        cmdVenta.Parameters.AddWithValue("@total", total);
                        cmdVenta.Parameters.AddWithValue("@cte", (object)cnn.miCteContado ?? DBNull.Value);
                        cmdVenta.Parameters.AddWithValue("@mtPago", (object)_MetodoPago ?? "Efectivo");
                        cmdVenta.Parameters.AddWithValue("@cteId", 1);
                        cmdVenta.Parameters.AddWithValue("@vtaID", factura);
                        cmdVenta.Parameters.AddWithValue("@fvta", fVenta);
                        cmdVenta.Parameters.AddWithValue("@uid", 1);

                        int recordID = Convert.ToInt32(cmdVenta.ExecuteScalar());

                        // INSERTAR DETALLES
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.IsNewRow) continue;

                            // --- BLOQUE DE CONVERSIÓN SEGURA ---
                            // Usamos funciones para limpiar el texto antes de convertir
                            int pId;
                            decimal cant, prec, subt;

                            // Intentamos convertir. Si falla, el valor será 0 y no se caerá el sistema.
                            int.TryParse(LimpiarParaNumero(row.Cells["E"].Value), out pId);
                            decimal.TryParse(LimpiarParaNumero(row.Cells["B"].Value), out cant);
                            decimal.TryParse(LimpiarParaNumero(row.Cells["C"].Value), out prec);
                            decimal.TryParse(LimpiarParaNumero(row.Cells["D"].Value), out subt);

                            // Validación extra: Si el ID de producto o cantidad es 0, saltamos la fila
                            if (pId <= 0 || cant <= 0) continue;

                            string insertDetalle = @"
        INSERT INTO DetalleVentas (VentaID, recordID, ProductoID, Cantidad, PrecioUnitario, Subtotal, Factura)
        VALUES (@ventaID_FK, @recordID, @productoID, @cantidad, @precio, @subtotal, @facturaTexto)";

                            SqlCommand cmdDetalle = new SqlCommand(insertDetalle, conn, tran);
                            cmdDetalle.Parameters.AddWithValue("@ventaID_FK", recordID);
                            cmdDetalle.Parameters.AddWithValue("@recordID", recordID);
                            cmdDetalle.Parameters.AddWithValue("@productoID", pId);
                            cmdDetalle.Parameters.AddWithValue("@cantidad", cant);
                            cmdDetalle.Parameters.AddWithValue("@precio", prec);
                            cmdDetalle.Parameters.AddWithValue("@subtotal", subt);
                            cmdDetalle.Parameters.AddWithValue("@facturaTexto", factura);
                            cmdDetalle.ExecuteNonQuery();

                            // ACTUALIZAR STOCK
                            string updateStock = @"UPDATE StockSucursal SET Cantidad = Cantidad - @cantidad WHERE ProductoID = @productoID";
                            SqlCommand cmdStock = new SqlCommand(updateStock, conn, tran);
                            cmdStock.Parameters.AddWithValue("@cantidad", cant);
                            cmdStock.Parameters.AddWithValue("@productoID", pId);
                            cmdStock.ExecuteNonQuery();
                        }

                        tran.Commit();

                        // Acciones post-transacción
                        ActualizaSecuenciaFactura(factura);
                        ImprimirTicketCompleto(recordID, "ncf", (double)total);
                        MessageBox.Show("Venta procesada correctamente.");
                        LimpiarFormulario();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Error de formato o datos: " + ex.Message + "\nVerifique que los precios y cantidades sean solo números.");
                    }
                }
            }
        }

        // Función auxiliar para limpiar strings antes de convertir
        private string LimpiarNumero(object valor)
        {
            if (valor == null || valor == DBNull.Value) return "0";
            string s = valor.ToString();
            // Elimina $, comas (dependiendo de tu región) y espacios
            return s.Replace("$", "").Replace(" ", "").Trim();
        }

        /*  private void Vender()
          {
              if (dgv.Rows.Count <= 1)
              {
                  MessageBox.Show("No hay productos en la venta.");
                  return;
              }

              using (SqlConnection conn = new SqlConnection(cnn.db))
              {
                  conn.Open();

                  using (SqlTransaction tran = conn.BeginTransaction())
                  {
                      try
                      {
                          double subtotal = ft_Subtotal;
                          double impuesto = ft_Impuesto;
                          double total = ft_Total;

                          string _factura = Busco.BuscaUltimoNumero("2");
                          string factura = Convert.ToInt32(_factura).ToString("D10");
                          string fVenta = DateTime.Now.ToString("yyyyMMdd");

                          // INSERTAR VENTA

                          string insertVenta = @"
      INSERT INTO Ventas (
          Fecha, 
          Subtotal, 
          Impuesto, 
          Total, 
          IDCLIENTE, 
          MetodoPago, 
          ClienteId, 
          Factura, 
          FechaVenta, 
          UsuarioID
      ) 
      VALUES (
          GETDATE(), 
          @subtotal, 
          @impuesto, 
          @total, 
          @cte, 
          @mtPago, 
          @cteId, 
          @vtaID, 
          @fvta, 
          @uid
      );
      SELECT SCOPE_IDENTITY();";

                          SqlCommand cmdVenta = new SqlCommand(insertVenta, conn, tran);

                          // 2. Mapeo de parámetros (asegúrate de que los tipos coincidan)
                          cmdVenta.Parameters.AddWithValue("@subtotal", subtotal);
                          cmdVenta.Parameters.AddWithValue("@impuesto", impuesto);
                          cmdVenta.Parameters.AddWithValue("@total", total);
                          cmdVenta.Parameters.AddWithValue("@cte", (object)cnn.miCteContado ?? DBNull.Value); 
                          cmdVenta.Parameters.AddWithValue("@mtPago", (object)_MetodoPago ?? DBNull.Value);
                          cmdVenta.Parameters.AddWithValue("@cteId", 1); 
                          cmdVenta.Parameters.AddWithValue("@vtaID", (object)factura ?? DBNull.Value);
                          cmdVenta.Parameters.AddWithValue("@fvta", (object)fVenta ?? DBNull.Value);
                          cmdVenta.Parameters.AddWithValue("@uid", 1);

                          // 3. Ejecutar y obtener el ID generado automáticamente
                          int recordID = Convert.ToInt32(cmdVenta.ExecuteScalar());

                          // INSERTAR DETALLES
                          foreach (DataGridViewRow row in dgv.Rows)
                          {
                              if (row.IsNewRow) continue;

                              // Convertimos los valores del Grid a los tipos correctos de C#
                              // Esto asegura que @productoID sea un int y no un string '1'
                              int pId = Convert.ToInt32(row.Cells["E"].Value);
                              decimal cant = Convert.ToDecimal(row.Cells["B"].Value);
                              decimal prec = Convert.ToDecimal(row.Cells["C"].Value);
                              decimal subt = Convert.ToDecimal(row.Cells["D"].Value);

                              string insertDetalle = @"
      INSERT INTO DetalleVentas (
          VentaID, recordID, ProductoID, Cantidad, PrecioUnitario, Subtotal, Factura
      )
      VALUES (
          @ventaID_FK, @recordID, @productoID, @cantidad, @precio, @subtotal, @facturaTexto
      )";

                              SqlCommand cmdDetalle = new SqlCommand(insertDetalle, conn, tran);
                              cmdDetalle.Parameters.AddWithValue("@ventaID_FK", recordID);
                              cmdDetalle.Parameters.AddWithValue("@recordID", recordID);
                              cmdDetalle.Parameters.AddWithValue("@productoID", pId); // Ahora es un int real
                              cmdDetalle.Parameters.AddWithValue("@cantidad", cant);
                              cmdDetalle.Parameters.AddWithValue("@precio", prec);
                              cmdDetalle.Parameters.AddWithValue("@subtotal", subt);
                              cmdDetalle.Parameters.AddWithValue("@facturaTexto", factura);

                              cmdDetalle.ExecuteNonQuery();

                              // ACTUALIZAR STOCK
                              string updateStock = @"
      UPDATE StockSucursal
      SET Cantidad = Cantidad - @cantidad
      WHERE ProductoID = @productoID";

                              SqlCommand cmdStock = new SqlCommand(updateStock, conn, tran);
                              cmdStock.Parameters.AddWithValue("@cantidad", cant);
                              cmdStock.Parameters.AddWithValue("@productoID", pId); // Enviamos el int

                              cmdStock.ExecuteNonQuery();
                          }
                          tran.Commit();

                          ActualizaSecuenciaFactura(factura);

                          // -------------------------------------------
                          // Factura electronica fuera de la transaccion
                          // -------------------------------------------
                          // FacturaElectronica(recordID, total);

                          ImprimirTicketCompleto(recordID, "ncf", total);

                          MessageBox.Show("Venta procesada correctamente.");

                          LimpiarFormulario();
                      }
                      catch (Exception ex)
                      {
                          tran.Rollback();
                          MessageBox.Show("Error al procesar venta: " + ex.Message);
                      }
                  }
              }
          }*/

        private void ActualizaSecuenciaFactura(string factura)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                string query = @"UPDATE SECUENCIAS SET CONTADOR = @A1 WHERE ID = @A2";

                SqlCommand cmd = new SqlCommand(query, cxn);
                cmd.Parameters.AddWithValue("@A1", factura);
                cmd.Parameters.AddWithValue("@A2", "2");
                cmd.ExecuteNonQuery();
            }
        }

        private void FacturaElectronica(int ventaID, double total)
        {
            try
            {
                int sucursalID = Convert.ToInt32(cnn.miSucursal);
                string tipoNCF = "B01";
                string cliente = "CONSUMIDOR FINAL";

                // Generar NCF
                string ncf = NCFService.GenerarNCF(sucursalID, tipoNCF);

                // Generar XML base
                string xmlGenerado = XMLFactura.GenerarXML(ncf, cliente, total);

                // Firmar XML
                string xmlFirmado = firmaDigital.FirmarXML(
                    xmlGenerado, @"C:\Certificado\certificado.pfx", "password" );

                // Guardar XML
                string rutaXML = Path.Combine(Application.StartupPath, $"eCF_{ncf}.xml");

                File.WriteAllText(rutaXML, xmlFirmado);

                // Enviar a DGII
                var respuestaDGII = DGIIService.Enviar(xmlFirmado);

                string trackId = respuestaDGII.TrackId;
                string estado = respuestaDGII.Estado;
                string mensaje = respuestaDGII.Mensaje;

                // Guardar en BD
                using (SqlConnection conn = new SqlConnection(cnn.db))
                {
                    conn.Open();

                    string insert = @"
                                INSERT INTO FacturaElectronica
                                (VentaID, NCF, Total, TrackID, EstadoDGII, RutaXML)
                                VALUES
                                (@ventaID, @ncf, @total, @trackId, @estado, @rutaXML)";

                    SqlCommand cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@ventaID", ventaID);
                    cmd.Parameters.AddWithValue("@ncf", ncf);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@trackId", trackId);
                    cmd.Parameters.AddWithValue("@estado", estado);
                    cmd.Parameters.AddWithValue("@rutaXML", rutaXML);

                    cmd.ExecuteNonQuery();
                }

                // Generar QR
                string qrData = $"https://ecf.dgii.gov.do/TesteCF/Consulta?t={trackId}";
                qrImagenGlobal = QRService.GenerarQR(qrData);


                // ImprimirTicketCompleto(ventaID, ncf, total);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en Factura Electrónica: " + ex.Message);
            }
        }


        private void ImprimirTicketCompleto(int ventaID, string ncf, double total)
        {
            PrintDocument pd = new PrintDocument();

            pd.PrintPage += (s, e) =>
            {
                float y = 10;
                Font font = new Font("Consolas", 9);

                e.Graphics.DrawString($"{ cnn.miRNCNombre }", font, Brushes.Black, 10, y);
                y += 18;
                e.Graphics.DrawString($"RNC: { cnn.miRNC }", font, Brushes.Black, 10, y);
                y += 18;
                e.Graphics.DrawString($"Direccion: { cnn.miDireccion }", font, Brushes.Black, 10, y);
                y += 18;
                e.Graphics.DrawString($"Telefono: { cnn.miTelefono }", font, Brushes.Black, 10, y);
                y += 18;

                e.Graphics.DrawString("NCF: " + ncf, font, Brushes.Black, 10, y);
                y += 18;
                e.Graphics.DrawString("Factura #: " + ventaID, font, Brushes.Black, 10, y);
                y += 18;
                e.Graphics.DrawString("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), font, Brushes.Black, 10, y);
                y += 18;

                e.Graphics.DrawString("--------------------------------", font, Brushes.Black, 10, y);
                y += 18;

                // recorrer datagridview
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    string desc = row.Cells[0].Value?.ToString();
                    string cant = row.Cells[1].Value?.ToString();
                    string precio = Convert.ToDouble(row.Cells[2].Value).ToString("F2");
                    string totalLinea = Convert.ToDouble(row.Cells[3].Value).ToString("F2");

                    string linea = $"{desc,-10} {cant,4} {precio,6} {totalLinea,7}";

                    e.Graphics.DrawString(linea, font, Brushes.Black, 10, y);
                    y += 18;
                }

                y += 10;
                e.Graphics.DrawString("--------------------------------", font, Brushes.Black, 10, y);
                y += 18;

                e.Graphics.DrawString("TOTAL: RD$ " + total.ToString("F2"), font, Brushes.Black, 10, y);
                y += 25;

                // QR
                if (qrImagenGlobal != null)
                {
                    e.Graphics.DrawImage(qrImagenGlobal, 40, y, 120, 120);
                    y += 130;
                }

                e.Graphics.DrawString("e-CF válido según DGII", font, Brushes.Black, 10, y);
                y += 18;

                e.Graphics.DrawString("Gracias por su compra", font, Brushes.Black, 10, y);
            };

            pd.Print();
        }

        private void LimpiarFormulario()
        {
            txtCantidad.Clear();
            txtProducto.Clear();
            txtBuscarNombre.Clear();

            _CodigoArticulo = "";
            _Descripcion = "";
            _Precio = "";

            lblDescuento.Text = "";
            lblDevuelta.Text = "";
            lblImpuesto.Text = "";
            lblOtros.Text = "";
            lblPago.Text = "";
            lblSubTotal.Text = "";
            lblTotalPagar.Text = "";

            _impuesto = 0;
            _autorizado = false;

            dgv.Rows.Clear();
            dgv.Refresh();
        }

        private string LimpiarParaNumero(object valor)
        {
            if (valor == null || valor == DBNull.Value) return "0";

            string texto = valor.ToString();

            // 1. Quitamos símbolo de moneda, espacios y comas (si usas punto decimal)
            texto = texto.Replace("$", "").Replace(" ", "").Trim();

            // 2. Si el texto queda vacío después de limpiar, devolvemos "0"
            return string.IsNullOrEmpty(texto) ? "0" : texto;
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void cboCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
