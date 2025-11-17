namespace Reverse.Forms.FormsExpedicao
{
    partial class ExpedicaoFormBalanco
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpedicaoFormBalanco));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPesoRestante = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLancamentos = new System.Windows.Forms.Button();
            this.btnLaudo = new System.Windows.Forms.Button();
            this.btnCertificado = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtVolume = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPeso = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpData = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.cbEmpresa = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvTickets = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvTotal = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnCarregar = new System.Windows.Forms.Button();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCriarLinha = new System.Windows.Forms.Button();
            this.btnExcluirLinha = new System.Windows.Forms.Button();
            this.dgvBalanca = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotal)).BeginInit();
            this.panel4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanca)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel1.Controls.Add(this.lblPesoRestante);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 56);
            this.panel1.TabIndex = 2;
            // 
            // lblPesoRestante
            // 
            this.lblPesoRestante.AutoSize = true;
            this.lblPesoRestante.ForeColor = System.Drawing.Color.White;
            this.lblPesoRestante.Location = new System.Drawing.Point(1163, 19);
            this.lblPesoRestante.Name = "lblPesoRestante";
            this.lblPesoRestante.Size = new System.Drawing.Size(134, 25);
            this.lblPesoRestante.TabIndex = 3;
            this.lblPesoRestante.Text = "Peso Restante";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Balanço de Massa";
            // 
            // btnLancamentos
            // 
            this.btnLancamentos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.btnLancamentos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLancamentos.ForeColor = System.Drawing.Color.Black;
            this.btnLancamentos.Location = new System.Drawing.Point(5, 808);
            this.btnLancamentos.Name = "btnLancamentos";
            this.btnLancamentos.Size = new System.Drawing.Size(399, 41);
            this.btnLancamentos.TabIndex = 7;
            this.btnLancamentos.Text = "Lançamentos";
            this.btnLancamentos.UseVisualStyleBackColor = false;
            // 
            // btnLaudo
            // 
            this.btnLaudo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLaudo.ForeColor = System.Drawing.Color.Black;
            this.btnLaudo.Location = new System.Drawing.Point(5, 855);
            this.btnLaudo.Name = "btnLaudo";
            this.btnLaudo.Size = new System.Drawing.Size(399, 41);
            this.btnLaudo.TabIndex = 8;
            this.btnLaudo.Text = "Exportar Laudo";
            this.btnLaudo.UseVisualStyleBackColor = true;
            this.btnLaudo.Click += new System.EventHandler(this.btnLaudo_Click);
            // 
            // btnCertificado
            // 
            this.btnCertificado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCertificado.ForeColor = System.Drawing.Color.Black;
            this.btnCertificado.Location = new System.Drawing.Point(5, 902);
            this.btnCertificado.Name = "btnCertificado";
            this.btnCertificado.Size = new System.Drawing.Size(399, 41);
            this.btnCertificado.TabIndex = 9;
            this.btnCertificado.Text = "Exportar Certificado";
            this.btnCertificado.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtVolume);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.btnCertificado);
            this.panel2.Controls.Add(this.txtPeso);
            this.panel2.Controls.Add(this.btnLaudo);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.btnLancamentos);
            this.panel2.Controls.Add(this.dtpData);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cbEmpresa);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(410, 949);
            this.panel2.TabIndex = 3;
            // 
            // txtVolume
            // 
            this.txtVolume.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txtVolume.Location = new System.Drawing.Point(0, 263);
            this.txtVolume.Name = "txtVolume";
            this.txtVolume.Size = new System.Drawing.Size(410, 33);
            this.txtVolume.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(0, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(410, 35);
            this.label5.TabIndex = 10;
            this.label5.Text = "Volume Total";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPeso
            // 
            this.txtPeso.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.txtPeso.Location = new System.Drawing.Point(0, 189);
            this.txtPeso.Name = "txtPeso";
            this.txtPeso.Size = new System.Drawing.Size(410, 33);
            this.txtPeso.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(0, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(410, 35);
            this.label4.TabIndex = 4;
            this.label4.Text = "Peso Total";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpData
            // 
            this.dtpData.CalendarFont = new System.Drawing.Font("Segoe UI", 14.25F);
            this.dtpData.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpData.Location = new System.Drawing.Point(0, 115);
            this.dtpData.Name = "dtpData";
            this.dtpData.Size = new System.Drawing.Size(410, 33);
            this.dtpData.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(0, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(410, 35);
            this.label3.TabIndex = 2;
            this.label3.Text = "Data";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbEmpresa
            // 
            this.cbEmpresa.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbEmpresa.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbEmpresa.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbEmpresa.FormattingEnabled = true;
            this.cbEmpresa.Location = new System.Drawing.Point(0, 41);
            this.cbEmpresa.Name = "cbEmpresa";
            this.cbEmpresa.Size = new System.Drawing.Size(410, 33);
            this.cbEmpresa.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(0, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(410, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "Empresa";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dgvTickets, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(410, 56);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1510, 949);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // dgvTickets
            // 
            this.dgvTickets.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvTickets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTickets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTickets.Location = new System.Drawing.Point(3, 3);
            this.dgvTickets.Name = "dgvTickets";
            this.dgvTickets.Size = new System.Drawing.Size(749, 943);
            this.dgvTickets.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dgvTotal);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.dgvBalanca);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(758, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(749, 943);
            this.panel3.TabIndex = 1;
            // 
            // dgvTotal
            // 
            this.dgvTotal.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvTotal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTotal.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvTotal.Location = new System.Drawing.Point(0, 530);
            this.dgvTotal.Name = "dgvTotal";
            this.dgvTotal.Size = new System.Drawing.Size(749, 342);
            this.dgvTotal.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tableLayoutPanel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 878);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(749, 65);
            this.panel4.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancelar, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCarregar, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSalvar, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCriarLinha, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnExcluirLinha, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(749, 65);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.Black;
            this.btnCancelar.Location = new System.Drawing.Point(152, 3);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(143, 59);
            this.btnCancelar.TabIndex = 2;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnCarregar
            // 
            this.btnCarregar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCarregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarregar.ForeColor = System.Drawing.Color.Black;
            this.btnCarregar.Location = new System.Drawing.Point(3, 3);
            this.btnCarregar.Name = "btnCarregar";
            this.btnCarregar.Size = new System.Drawing.Size(143, 59);
            this.btnCarregar.TabIndex = 1;
            this.btnCarregar.Text = "Carregar";
            this.btnCarregar.UseVisualStyleBackColor = true;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.ForeColor = System.Drawing.Color.Black;
            this.btnSalvar.Location = new System.Drawing.Point(599, 3);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(147, 59);
            this.btnSalvar.TabIndex = 3;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            // 
            // btnCriarLinha
            // 
            this.btnCriarLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCriarLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCriarLinha.ForeColor = System.Drawing.Color.Black;
            this.btnCriarLinha.Location = new System.Drawing.Point(450, 3);
            this.btnCriarLinha.Name = "btnCriarLinha";
            this.btnCriarLinha.Size = new System.Drawing.Size(143, 59);
            this.btnCriarLinha.TabIndex = 4;
            this.btnCriarLinha.Text = "Criar Linha";
            this.btnCriarLinha.UseVisualStyleBackColor = true;
            this.btnCriarLinha.Click += new System.EventHandler(this.btnCriarLinha_Click);
            // 
            // btnExcluirLinha
            // 
            this.btnExcluirLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExcluirLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirLinha.ForeColor = System.Drawing.Color.Black;
            this.btnExcluirLinha.Location = new System.Drawing.Point(301, 3);
            this.btnExcluirLinha.Name = "btnExcluirLinha";
            this.btnExcluirLinha.Size = new System.Drawing.Size(143, 59);
            this.btnExcluirLinha.TabIndex = 5;
            this.btnExcluirLinha.Text = "Excluir Linha";
            this.btnExcluirLinha.UseVisualStyleBackColor = true;
            this.btnExcluirLinha.Click += new System.EventHandler(this.btnExcluirLinha_Click);
            // 
            // dgvBalanca
            // 
            this.dgvBalanca.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvBalanca.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBalanca.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvBalanca.Location = new System.Drawing.Point(0, 0);
            this.dgvBalanca.Name = "dgvBalanca";
            this.dgvBalanca.Size = new System.Drawing.Size(749, 530);
            this.dgvBalanca.TabIndex = 1;
            // 
            // ExpedicaoFormBalanco
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1920, 1005);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExpedicaoFormBalanco";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormDesc";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTickets)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotal)).EndInit();
            this.panel4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalanca)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLancamentos;
        private System.Windows.Forms.Button btnCertificado;
        private System.Windows.Forms.Button btnLaudo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvTickets;
        private System.Windows.Forms.DataGridView dgvBalanca;
        private System.Windows.Forms.ComboBox cbEmpresa;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpData;
        private System.Windows.Forms.TextBox txtPeso;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVolume;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPesoRestante;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnCarregar;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCriarLinha;
        private System.Windows.Forms.Button btnExcluirLinha;
        private System.Windows.Forms.DataGridView dgvTotal;
    }
}