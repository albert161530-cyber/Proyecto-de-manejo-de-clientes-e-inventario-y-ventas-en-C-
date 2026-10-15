namespace PLUNES
{
    partial class frmAutoriza
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtAutoriza = new System.Windows.Forms.TextBox();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(72, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = "Autorizacion";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAutoriza
            // 
            this.txtAutoriza.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAutoriza.Location = new System.Drawing.Point(72, 111);
            this.txtAutoriza.Multiline = true;
            this.txtAutoriza.Name = "txtAutoriza";
            this.txtAutoriza.PasswordChar = '*';
            this.txtAutoriza.Size = new System.Drawing.Size(216, 54);
            this.txtAutoriza.TabIndex = 1;
            this.txtAutoriza.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAutoriza.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAutoriza_KeyPress);
            this.txtAutoriza.Leave += new System.EventHandler(this.txtAutoriza_Leave);
            // 
            // btnAceptar
            // 
            this.btnAceptar.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAceptar.Location = new System.Drawing.Point(72, 171);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(216, 66);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // frmAutoriza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 323);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.txtAutoriza);
            this.Controls.Add(this.label1);
            this.Name = "frmAutoriza";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAutoriza";
            this.Load += new System.EventHandler(this.frmAutoriza_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAutoriza_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAutoriza;
        private System.Windows.Forms.Button btnAceptar;
    }
}