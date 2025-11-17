namespace Reverse.Forms.FormsExpedicao
{
    partial class ExpedicaoFormExpHub
    {
        private System.ComponentModel.IContainer components = null;

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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpedicaoFormExpHub));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picEstoque = new System.Windows.Forms.PictureBox();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.picDesc = new System.Windows.Forms.PictureBox();
            this.picFrete = new System.Windows.Forms.PictureBox();
            this.picCadastro = new System.Windows.Forms.PictureBox();
            this.picControle = new System.Windows.Forms.PictureBox();
            this.btnSair = new System.Windows.Forms.Button();
            this.pnlConteudo = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEstoque)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDesc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFrete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCadastro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControle)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.picEstoque);
            this.pnlHeader.Controls.Add(this.btnMinimizar);
            this.pnlHeader.Controls.Add(this.picDesc);
            this.pnlHeader.Controls.Add(this.picFrete);
            this.pnlHeader.Controls.Add(this.picCadastro);
            this.pnlHeader.Controls.Add(this.picControle);
            this.pnlHeader.Controls.Add(this.btnSair);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1920, 34);
            this.pnlHeader.TabIndex = 2;
            // 
            // picEstoque
            // 
            this.picEstoque.BackColor = System.Drawing.Color.White;
            this.picEstoque.Dock = System.Windows.Forms.DockStyle.Left;
            this.picEstoque.Image = ((System.Drawing.Image)(resources.GetObject("picEstoque.Image")));
            this.picEstoque.Location = new System.Drawing.Point(224, 0);
            this.picEstoque.Name = "picEstoque";
            this.picEstoque.Size = new System.Drawing.Size(56, 34);
            this.picEstoque.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEstoque.TabIndex = 9;
            this.picEstoque.TabStop = false;
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
            this.btnMinimizar.Click += new System.EventHandler(this.btnMinimizar_Click);
            // 
            // picDesc
            // 
            this.picDesc.BackColor = System.Drawing.Color.White;
            this.picDesc.Dock = System.Windows.Forms.DockStyle.Left;
            this.picDesc.Image = ((System.Drawing.Image)(resources.GetObject("picDesc.Image")));
            this.picDesc.Location = new System.Drawing.Point(168, 0);
            this.picDesc.Name = "picDesc";
            this.picDesc.Size = new System.Drawing.Size(56, 34);
            this.picDesc.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDesc.TabIndex = 7;
            this.picDesc.TabStop = false;
            // 
            // picFrete
            // 
            this.picFrete.BackColor = System.Drawing.Color.White;
            this.picFrete.Dock = System.Windows.Forms.DockStyle.Left;
            this.picFrete.Image = ((System.Drawing.Image)(resources.GetObject("picFrete.Image")));
            this.picFrete.Location = new System.Drawing.Point(112, 0);
            this.picFrete.Name = "picFrete";
            this.picFrete.Size = new System.Drawing.Size(56, 34);
            this.picFrete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picFrete.TabIndex = 4;
            this.picFrete.TabStop = false;
            // 
            // picCadastro
            // 
            this.picCadastro.BackColor = System.Drawing.Color.White;
            this.picCadastro.Dock = System.Windows.Forms.DockStyle.Left;
            this.picCadastro.Image = ((System.Drawing.Image)(resources.GetObject("picCadastro.Image")));
            this.picCadastro.Location = new System.Drawing.Point(56, 0);
            this.picCadastro.Name = "picCadastro";
            this.picCadastro.Size = new System.Drawing.Size(56, 34);
            this.picCadastro.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCadastro.TabIndex = 2;
            this.picCadastro.TabStop = false;
            // 
            // picControle
            // 
            this.picControle.BackColor = System.Drawing.Color.White;
            this.picControle.Dock = System.Windows.Forms.DockStyle.Left;
            this.picControle.Image = ((System.Drawing.Image)(resources.GetObject("picControle.Image")));
            this.picControle.Location = new System.Drawing.Point(0, 0);
            this.picControle.Name = "picControle";
            this.picControle.Size = new System.Drawing.Size(56, 34);
            this.picControle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picControle.TabIndex = 1;
            this.picControle.TabStop = false;
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
            // pnlConteudo
            // 
            this.pnlConteudo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.pnlConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlConteudo.Location = new System.Drawing.Point(0, 34);
            this.pnlConteudo.Name = "pnlConteudo";
            this.pnlConteudo.Size = new System.Drawing.Size(1920, 966);
            this.pnlConteudo.TabIndex = 3;
            // 
            // ExpedicaoFormExpHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.Controls.Add(this.pnlConteudo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExpedicaoFormExpHub";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormExpHub";
            this.Load += new System.EventHandler(this.FormExpHub_Load);
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picEstoque)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDesc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFrete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCadastro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picControle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Panel pnlConteudo;
        private System.Windows.Forms.PictureBox picControle;
        private System.Windows.Forms.PictureBox picCadastro;
        private System.Windows.Forms.PictureBox picFrete;
        private System.Windows.Forms.PictureBox picDesc;
        private System.Windows.Forms.Button btnMinimizar;
        private System.Windows.Forms.PictureBox picEstoque;
    }
}