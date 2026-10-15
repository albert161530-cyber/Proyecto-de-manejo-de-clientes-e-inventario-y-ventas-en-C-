using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLUNES
{
    public partial class UserControl_Articulo : UserControl
    {
        public UserControl_Articulo()
        {
            InitializeComponent();

            // Bordes redondeados y sombra simulada
            this.BackColor = Color.White;
            this.Padding = new Padding(5);
            this.Margin = new Padding(10);
            this.BorderStyle = BorderStyle.FixedSingle;

            // PictureBox estilo tarjeta
            picImagen.SizeMode = PictureBoxSizeMode.Zoom;

            // Fuente y colores de labels
            lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(30, 30, 30);
            lblNombre.TextAlign = ContentAlignment.MiddleCenter;

            lblPrecio.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblPrecio.ForeColor = Color.FromArgb(50, 150, 50);
            lblPrecio.TextAlign = ContentAlignment.MiddleCenter;

            lblCodigo.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblCodigo.ForeColor = Color.FromArgb(50, 150, 50);
            lblCodigo.TextAlign = ContentAlignment.MiddleCenter;
            lblCodigo.Visible = false;

            // Boton 
            btnAgregar.Text = "Agregar al carrito";
            btnAgregar.BackColor = Color.FromArgb(30, 144, 255);
            btnAgregar.ForeColor = Color.White;
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Cursor = Cursors.Hand;
            btnAgregar.Height = 35;

            btnAgregar.Click += btnAgregar_Click;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            ProductoSeleccionado?.Invoke(this, EventArgs.Empty);
        }

        // Propiedades publicas
        public string NombreProducto
        {
            get { return lblNombre.Text; }
            set { lblNombre.Text = value; }
        }

        public string codigoArticulo
        {
            get { return lblCodigo.Text; }
            set { lblCodigo.Text = value; }
        }

        private decimal precio;
        public decimal Precio
        {
            get { return precio; }
            set
            {
                precio = value;
                lblPrecio.Text = precio.ToString("C");
            }
        }

        public Image ImagenProducto
        {
            get { return picImagen.Image; }
            set { picImagen.Image = value; }
        }

        public event EventHandler ProductoSeleccionado;
    }
}
