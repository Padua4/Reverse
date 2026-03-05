namespace Reverse.Forms.FormsAtendimento
{
    partial class AtendimentoForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblAtrasado = new System.Windows.Forms.Label();
            this.lblResolvido = new System.Windows.Forms.Label();
            this.lblAguardando = new System.Windows.Forms.Label();
            this.lblAndamento = new System.Windows.Forms.Label();
            this.lblNovo = new System.Windows.Forms.Label();
            this.dgvAnalise = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.btnAnalise = new System.Windows.Forms.Button();
            this.btnConcluido = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnalise)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 40);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Atendimentos";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvAnalise, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.194805F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.57143F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.263736F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1920, 926);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.lblAtrasado, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblResolvido, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblAguardando, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblAndamento, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblNovo, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1914, 42);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // lblAtrasado
            // 
            this.lblAtrasado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(205)))), ((int)(((byte)(210)))));
            this.lblAtrasado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAtrasado.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAtrasado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblAtrasado.Location = new System.Drawing.Point(1531, 0);
            this.lblAtrasado.Name = "lblAtrasado";
            this.lblAtrasado.Size = new System.Drawing.Size(380, 42);
            this.lblAtrasado.TabIndex = 4;
            this.lblAtrasado.Text = "Atrasado";
            this.lblAtrasado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAtrasado.Click += new System.EventHandler(this.lblAtrasado_Click);
            // 
            // lblResolvido
            // 
            this.lblResolvido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(230)))), ((int)(((byte)(201)))));
            this.lblResolvido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblResolvido.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblResolvido.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.lblResolvido.Location = new System.Drawing.Point(1149, 0);
            this.lblResolvido.Name = "lblResolvido";
            this.lblResolvido.Size = new System.Drawing.Size(376, 42);
            this.lblResolvido.TabIndex = 3;
            this.lblResolvido.Text = "Resolvido";
            this.lblResolvido.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResolvido.Click += new System.EventHandler(this.lblResolvido_Click);
            // 
            // lblAguardando
            // 
            this.lblAguardando.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblAguardando.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAguardando.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAguardando.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.lblAguardando.Location = new System.Drawing.Point(767, 0);
            this.lblAguardando.Name = "lblAguardando";
            this.lblAguardando.Size = new System.Drawing.Size(376, 42);
            this.lblAguardando.TabIndex = 2;
            this.lblAguardando.Text = "Aguardando";
            this.lblAguardando.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAguardando.Click += new System.EventHandler(this.lblAguardando_Click);
            // 
            // lblAndamento
            // 
            this.lblAndamento.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(108)))), ((int)(((byte)(0)))));
            this.lblAndamento.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAndamento.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAndamento.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(178)))));
            this.lblAndamento.Location = new System.Drawing.Point(385, 0);
            this.lblAndamento.Name = "lblAndamento";
            this.lblAndamento.Size = new System.Drawing.Size(376, 42);
            this.lblAndamento.TabIndex = 1;
            this.lblAndamento.Text = "Em Andamento";
            this.lblAndamento.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAndamento.Click += new System.EventHandler(this.lblAndamento_Click);
            // 
            // lblNovo
            // 
            this.lblNovo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(251)))));
            this.lblNovo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNovo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNovo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblNovo.Location = new System.Drawing.Point(3, 0);
            this.lblNovo.Name = "lblNovo";
            this.lblNovo.Size = new System.Drawing.Size(376, 42);
            this.lblNovo.TabIndex = 0;
            this.lblNovo.Text = "Novo";
            this.lblNovo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNovo.Click += new System.EventHandler(this.lblNovo_Click);
            // 
            // dgvAnalise
            // 
            this.dgvAnalise.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvAnalise.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAnalise.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAnalise.Location = new System.Drawing.Point(3, 51);
            this.dgvAnalise.Name = "dgvAnalise";
            this.dgvAnalise.Size = new System.Drawing.Size(1914, 813);
            this.dgvAnalise.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.btnExcluir, 6, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnAnalise, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnConcluido, 7, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 870);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1914, 53);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // btnExcluir
            // 
            this.btnExcluir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExcluir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluir.ForeColor = System.Drawing.Color.Black;
            this.btnExcluir.Location = new System.Drawing.Point(1437, 3);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(233, 47);
            this.btnExcluir.TabIndex = 7;
            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.UseVisualStyleBackColor = true;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // btnAnalise
            // 
            this.btnAnalise.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAnalise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnalise.ForeColor = System.Drawing.Color.Black;
            this.btnAnalise.Location = new System.Drawing.Point(1198, 3);
            this.btnAnalise.Name = "btnAnalise";
            this.btnAnalise.Size = new System.Drawing.Size(233, 47);
            this.btnAnalise.TabIndex = 6;
            this.btnAnalise.Text = "Analise";
            this.btnAnalise.UseVisualStyleBackColor = true;
            this.btnAnalise.Click += new System.EventHandler(this.btnAnalise_Click);
            // 
            // btnConcluido
            // 
            this.btnConcluido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConcluido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConcluido.ForeColor = System.Drawing.Color.Black;
            this.btnConcluido.Location = new System.Drawing.Point(1676, 3);
            this.btnConcluido.Name = "btnConcluido";
            this.btnConcluido.Size = new System.Drawing.Size(235, 47);
            this.btnConcluido.TabIndex = 0;
            this.btnConcluido.Text = "Concluido";
            this.btnConcluido.UseVisualStyleBackColor = true;
            this.btnConcluido.Click += new System.EventHandler(this.btnConcluido_Click);
            // 
            // AtendimentoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 966);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "AtendimentoForm";
            this.Text = "AtendimentoForm";
            this.Load += new System.EventHandler(this.AtendimentoForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnalise)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dgvAnalise;
        private System.Windows.Forms.Label lblAtrasado;
        private System.Windows.Forms.Label lblResolvido;
        private System.Windows.Forms.Label lblAguardando;
        private System.Windows.Forms.Label lblAndamento;
        private System.Windows.Forms.Label lblNovo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnConcluido;
        private System.Windows.Forms.Button btnExcluir;
        private System.Windows.Forms.Button btnAnalise;
    }
}