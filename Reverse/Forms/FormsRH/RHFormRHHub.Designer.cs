namespace Reverse.Forms.FormsRH
{
    partial class RHFormRHHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RHFormRHHub));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picFormCestaBasica = new System.Windows.Forms.PictureBox();
            this.picFormInatividade = new System.Windows.Forms.PictureBox();
            this.picFormFuncionarios = new System.Windows.Forms.PictureBox();
            this.panelConteudo = new System.Windows.Forms.Panel();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFormCestaBasica)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormInatividade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormFuncionarios)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(55)))), ((int)(((byte)(117)))));
            this.pnlHeader.Controls.Add(this.btnMinimizar);
            this.pnlHeader.Controls.Add(this.picFormCestaBasica);
            this.pnlHeader.Controls.Add(this.picFormInatividade);
            this.pnlHeader.Controls.Add(this.picFormFuncionarios);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1870, 34);
            this.pnlHeader.TabIndex = 0;
            // 
            // picFormCestaBasica
            // 
            this.picFormCestaBasica.BackColor = System.Drawing.Color.White;
            this.picFormCestaBasica.Dock = System.Windows.Forms.DockStyle.Left;
            this.picFormCestaBasica.Image = ((System.Drawing.Image)(resources.GetObject("picFormCestaBasica.Image")));
            this.picFormCestaBasica.Location = new System.Drawing.Point(118, 0);
            this.picFormCestaBasica.Name = "picFormCestaBasica";
            this.picFormCestaBasica.Size = new System.Drawing.Size(59, 34);
            this.picFormCestaBasica.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFormCestaBasica.TabIndex = 10;
            this.picFormCestaBasica.TabStop = false;
            // 
            // picFormInatividade
            // 
            this.picFormInatividade.BackColor = System.Drawing.Color.White;
            this.picFormInatividade.Dock = System.Windows.Forms.DockStyle.Left;
            this.picFormInatividade.Image = ((System.Drawing.Image)(resources.GetObject("picFormInatividade.Image")));
            this.picFormInatividade.Location = new System.Drawing.Point(59, 0);
            this.picFormInatividade.Name = "picFormInatividade";
            this.picFormInatividade.Size = new System.Drawing.Size(59, 34);
            this.picFormInatividade.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFormInatividade.TabIndex = 9;
            this.picFormInatividade.TabStop = false;
            // 
            // picFormFuncionarios
            // 
            this.picFormFuncionarios.BackColor = System.Drawing.Color.White;
            this.picFormFuncionarios.Dock = System.Windows.Forms.DockStyle.Left;
            this.picFormFuncionarios.Image = ((System.Drawing.Image)(resources.GetObject("picFormFuncionarios.Image")));
            this.picFormFuncionarios.Location = new System.Drawing.Point(0, 0);
            this.picFormFuncionarios.Name = "picFormFuncionarios";
            this.picFormFuncionarios.Size = new System.Drawing.Size(59, 34);
            this.picFormFuncionarios.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFormFuncionarios.TabIndex = 8;
            this.picFormFuncionarios.TabStop = false;
            // 
            // panelConteudo
            // 
            this.panelConteudo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelConteudo.BackgroundImage")));
            this.panelConteudo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConteudo.Location = new System.Drawing.Point(0, 34);
            this.panelConteudo.Name = "panelConteudo";
            this.panelConteudo.Size = new System.Drawing.Size(1870, 966);
            this.panelConteudo.TabIndex = 1;
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMinimizar.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMinimizar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizar.Location = new System.Drawing.Point(1816, 0);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(54, 34);
            this.btnMinimizar.TabIndex = 12;
            this.btnMinimizar.Text = "-";
            this.btnMinimizar.UseVisualStyleBackColor = true;
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // RHFormRHHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1870, 1000);
            this.Controls.Add(this.panelConteudo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RHFormRHHub";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormRHHub";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picFormCestaBasica)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormInatividade)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFormFuncionarios)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel panelConteudo;
        private System.Windows.Forms.PictureBox picFormFuncionarios;
        private System.Windows.Forms.PictureBox picFormInatividade;
        private System.Windows.Forms.PictureBox picFormCestaBasica;
        private System.Windows.Forms.Button btnMinimizar;
    }
}