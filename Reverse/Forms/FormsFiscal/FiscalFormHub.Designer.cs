namespace Reverse.Forms.FormsFiscal
{
    partial class FiscalFormHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FiscalFormHub));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picFiscalPedidos = new System.Windows.Forms.PictureBox();
            this.pnlConteudo = new System.Windows.Forms.Panel();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFiscalPedidos)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(55)))), ((int)(((byte)(117)))));
            this.pnlHeader.Controls.Add(this.btnMinimizar);
            this.pnlHeader.Controls.Add(this.picFiscalPedidos);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1870, 34);
            this.pnlHeader.TabIndex = 4;
            // 
            // picFiscalPedidos
            // 
            this.picFiscalPedidos.BackColor = System.Drawing.Color.White;
            this.picFiscalPedidos.Dock = System.Windows.Forms.DockStyle.Left;
            this.picFiscalPedidos.Image = ((System.Drawing.Image)(resources.GetObject("picFiscalPedidos.Image")));
            this.picFiscalPedidos.Location = new System.Drawing.Point(0, 0);
            this.picFiscalPedidos.Name = "picFiscalPedidos";
            this.picFiscalPedidos.Size = new System.Drawing.Size(56, 34);
            this.picFiscalPedidos.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFiscalPedidos.TabIndex = 9;
            this.picFiscalPedidos.TabStop = false;
            // 
            // pnlConteudo
            // 
            this.pnlConteudo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlConteudo.BackgroundImage")));
            this.pnlConteudo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConteudo.Location = new System.Drawing.Point(0, 34);
            this.pnlConteudo.Name = "pnlConteudo";
            this.pnlConteudo.Size = new System.Drawing.Size(1870, 966);
            this.pnlConteudo.TabIndex = 5;
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMinimizar.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMinimizar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizar.ForeColor = System.Drawing.Color.White;
            this.btnMinimizar.Location = new System.Drawing.Point(1816, 0);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(54, 34);
            this.btnMinimizar.TabIndex = 12;
            this.btnMinimizar.Text = "-";
            this.btnMinimizar.UseVisualStyleBackColor = true;
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // FiscalFormHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1870, 1000);
            this.Controls.Add(this.pnlConteudo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FiscalFormHub";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FiscalFormHub";
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picFiscalPedidos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.PictureBox picFiscalPedidos;
        private System.Windows.Forms.Panel pnlConteudo;
        private System.Windows.Forms.Button btnMinimizar;
    }
}