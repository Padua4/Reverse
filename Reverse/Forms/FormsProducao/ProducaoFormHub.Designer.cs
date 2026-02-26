namespace Reverse.Forms.FormsProducao
{
    partial class ProducaoFormHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProducaoFormHub));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.picProducao = new System.Windows.Forms.PictureBox();
            this.picGrafico = new System.Windows.Forms.PictureBox();
            this.pnlConteudo = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picProducao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGrafico)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.picGrafico);
            this.pnlHeader.Controls.Add(this.picProducao);
            this.pnlHeader.Controls.Add(this.btnMinimizar);
            this.pnlHeader.Controls.Add(this.btnSair);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1920, 34);
            this.pnlHeader.TabIndex = 3;
            // 
            // btnMinimizar
            // 
            this.btnMinimizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(160)))));
            this.btnMinimizar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnMinimizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimizar.ForeColor = System.Drawing.Color.White;
            this.btnMinimizar.Location = new System.Drawing.Point(1808, 0);
            this.btnMinimizar.Name = "btnMinimizar";
            this.btnMinimizar.Size = new System.Drawing.Size(56, 34);
            this.btnMinimizar.TabIndex = 8;
            this.btnMinimizar.Text = "-";
            this.btnMinimizar.UseVisualStyleBackColor = false;
            // 
            // btnSair
            // 
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(160)))));
            this.btnSair.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.ForeColor = System.Drawing.Color.White;
            this.btnSair.Location = new System.Drawing.Point(1864, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(56, 34);
            this.btnSair.TabIndex = 0;
            this.btnSair.Text = "x";
            this.btnSair.UseVisualStyleBackColor = false;
            // 
            // picProducao
            // 
            this.picProducao.BackColor = System.Drawing.Color.White;
            this.picProducao.Dock = System.Windows.Forms.DockStyle.Left;
            this.picProducao.Image = ((System.Drawing.Image)(resources.GetObject("picProducao.Image")));
            this.picProducao.Location = new System.Drawing.Point(0, 0);
            this.picProducao.Name = "picProducao";
            this.picProducao.Size = new System.Drawing.Size(56, 34);
            this.picProducao.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picProducao.TabIndex = 9;
            this.picProducao.TabStop = false;
            // 
            // picGrafico
            // 
            this.picGrafico.BackColor = System.Drawing.Color.White;
            this.picGrafico.Dock = System.Windows.Forms.DockStyle.Left;
            this.picGrafico.Image = ((System.Drawing.Image)(resources.GetObject("picGrafico.Image")));
            this.picGrafico.Location = new System.Drawing.Point(56, 0);
            this.picGrafico.Name = "picGrafico";
            this.picGrafico.Size = new System.Drawing.Size(56, 34);
            this.picGrafico.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGrafico.TabIndex = 10;
            this.picGrafico.TabStop = false;
            // 
            // pnlConteudo
            // 
            this.pnlConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConteudo.Location = new System.Drawing.Point(0, 34);
            this.pnlConteudo.Name = "pnlConteudo";
            this.pnlConteudo.Size = new System.Drawing.Size(1920, 966);
            this.pnlConteudo.TabIndex = 4;
            // 
            // ProducaoFormHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.Controls.Add(this.pnlConteudo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ProducaoFormHub";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProducaoFormHub";
            this.Load += new System.EventHandler(this.ProducaoFormHub_Load);
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picProducao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGrafico)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnMinimizar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.PictureBox picProducao;
        private System.Windows.Forms.PictureBox picGrafico;
        private System.Windows.Forms.Panel pnlConteudo;
    }
}