namespace Reverse.Forms.FormsAtendimento
{
    partial class AtendimentoFormHub
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AtendimentoFormHub));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picChamado = new System.Windows.Forms.PictureBox();
            this.picAtendimento = new System.Windows.Forms.PictureBox();
            this.btnMinimizar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.panelConteudo = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChamado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAtendimento)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.picChamado);
            this.pnlHeader.Controls.Add(this.picAtendimento);
            this.pnlHeader.Controls.Add(this.btnMinimizar);
            this.pnlHeader.Controls.Add(this.btnSair);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1920, 34);
            this.pnlHeader.TabIndex = 3;
            // 
            // picChamado
            // 
            this.picChamado.BackColor = System.Drawing.Color.White;
            this.picChamado.Dock = System.Windows.Forms.DockStyle.Left;
            this.picChamado.Image = ((System.Drawing.Image)(resources.GetObject("picChamado.Image")));
            this.picChamado.Location = new System.Drawing.Point(56, 0);
            this.picChamado.Name = "picChamado";
            this.picChamado.Size = new System.Drawing.Size(56, 34);
            this.picChamado.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picChamado.TabIndex = 10;
            this.picChamado.TabStop = false;
            // 
            // picAtendimento
            // 
            this.picAtendimento.BackColor = System.Drawing.Color.White;
            this.picAtendimento.Dock = System.Windows.Forms.DockStyle.Left;
            this.picAtendimento.Image = ((System.Drawing.Image)(resources.GetObject("picAtendimento.Image")));
            this.picAtendimento.Location = new System.Drawing.Point(0, 0);
            this.picAtendimento.Name = "picAtendimento";
            this.picAtendimento.Size = new System.Drawing.Size(56, 34);
            this.picAtendimento.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAtendimento.TabIndex = 9;
            this.picAtendimento.TabStop = false;
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
            // panelConteudo
            // 
            this.panelConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConteudo.Location = new System.Drawing.Point(0, 34);
            this.panelConteudo.Name = "panelConteudo";
            this.panelConteudo.Size = new System.Drawing.Size(1920, 966);
            this.panelConteudo.TabIndex = 4;
            // 
            // AtendimentoFormHub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1000);
            this.Controls.Add(this.panelConteudo);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "AtendimentoFormHub";
            this.Text = "AtendimentoFormHub";
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picChamado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAtendimento)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnMinimizar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.PictureBox picAtendimento;
        private System.Windows.Forms.PictureBox picChamado;
        private System.Windows.Forms.Panel panelConteudo;
    }
}