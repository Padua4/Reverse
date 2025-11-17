namespace Reverse.Forms.FormsExpedicao
{
    partial class ExpedicaoFormFrete
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpedicaoFormFrete));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LayoutMenu = new System.Windows.Forms.TableLayoutPanel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.cmbTransportadora = new System.Windows.Forms.ComboBox();
            this.panel12 = new System.Windows.Forms.Panel();
            this.txtGerador = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.rbFOB = new System.Windows.Forms.RadioButton();
            this.rbSIF = new System.Windows.Forms.RadioButton();
            this.panel9 = new System.Windows.Forms.Panel();
            this.dtpBaixa = new System.Windows.Forms.DateTimePicker();
            this.panel8 = new System.Windows.Forms.Panel();
            this.dtpVencimento = new System.Windows.Forms.DateTimePicker();
            this.panel7 = new System.Windows.Forms.Panel();
            this.dtpOcorrencia = new System.Windows.Forms.DateTimePicker();
            this.panel6 = new System.Windows.Forms.Panel();
            this.txtDestinoFinal = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtDestino = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtOrigem = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.txtValorFrete = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnFinalizado = new System.Windows.Forms.Button();
            this.btnExcluirLinha = new System.Windows.Forms.Button();
            this.btnNovaLinha = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFrete = new System.Windows.Forms.DataGridView();
            this.panel15 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.txtFiltro = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.dgvFretesFiltrados = new System.Windows.Forms.DataGridView();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.LayoutMenu.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFrete)).BeginInit();
            this.panel15.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFretesFiltrados)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 56);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Controle Frete";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 56);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1920, 949);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.LayoutMenu);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(954, 943);
            this.panel2.TabIndex = 1;
            // 
            // LayoutMenu
            // 
            this.LayoutMenu.ColumnCount = 2;
            this.LayoutMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.0587F));
            this.LayoutMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.9413F));
            this.LayoutMenu.Controls.Add(this.panel13, 1, 1);
            this.LayoutMenu.Controls.Add(this.panel12, 1, 0);
            this.LayoutMenu.Controls.Add(this.label2, 0, 0);
            this.LayoutMenu.Controls.Add(this.label3, 0, 1);
            this.LayoutMenu.Controls.Add(this.label9, 0, 10);
            this.LayoutMenu.Controls.Add(this.label8, 0, 9);
            this.LayoutMenu.Controls.Add(this.label10, 0, 8);
            this.LayoutMenu.Controls.Add(this.label7, 0, 7);
            this.LayoutMenu.Controls.Add(this.label6, 0, 6);
            this.LayoutMenu.Controls.Add(this.label11, 0, 5);
            this.LayoutMenu.Controls.Add(this.label5, 0, 4);
            this.LayoutMenu.Controls.Add(this.label4, 0, 3);
            this.LayoutMenu.Controls.Add(this.panel11, 1, 10);
            this.LayoutMenu.Controls.Add(this.panel10, 1, 9);
            this.LayoutMenu.Controls.Add(this.panel9, 1, 8);
            this.LayoutMenu.Controls.Add(this.panel8, 1, 7);
            this.LayoutMenu.Controls.Add(this.panel7, 1, 6);
            this.LayoutMenu.Controls.Add(this.panel6, 1, 5);
            this.LayoutMenu.Controls.Add(this.panel4, 1, 4);
            this.LayoutMenu.Controls.Add(this.panel5, 1, 3);
            this.LayoutMenu.Controls.Add(this.label12, 0, 2);
            this.LayoutMenu.Controls.Add(this.panel14, 1, 2);
            this.LayoutMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutMenu.Location = new System.Drawing.Point(0, 0);
            this.LayoutMenu.Name = "LayoutMenu";
            this.LayoutMenu.RowCount = 11;
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.LayoutMenu.Size = new System.Drawing.Size(954, 866);
            this.LayoutMenu.TabIndex = 10;
            // 
            // panel13
            // 
            this.panel13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel13.Controls.Add(this.cmbTransportadora);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(347, 81);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(604, 72);
            this.panel13.TabIndex = 23;
            // 
            // cmbTransportadora
            // 
            this.cmbTransportadora.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmbTransportadora.FormattingEnabled = true;
            this.cmbTransportadora.Location = new System.Drawing.Point(3, 28);
            this.cmbTransportadora.Name = "cmbTransportadora";
            this.cmbTransportadora.Size = new System.Drawing.Size(598, 29);
            this.cmbTransportadora.TabIndex = 4;
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.txtGerador);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(347, 3);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(604, 72);
            this.panel12.TabIndex = 22;
            // 
            // txtGerador
            // 
            this.txtGerador.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGerador.Location = new System.Drawing.Point(2, 23);
            this.txtGerador.Name = "txtGerador";
            this.txtGerador.Size = new System.Drawing.Size(598, 29);
            this.txtGerador.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(338, 78);
            this.label2.TabIndex = 1;
            this.label2.Text = "Gerador";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(338, 78);
            this.label3.TabIndex = 3;
            this.label3.Text = "Transportadora";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(3, 780);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(338, 86);
            this.label9.TabIndex = 11;
            this.label9.Text = "Status";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(3, 702);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(338, 78);
            this.label8.TabIndex = 9;
            this.label8.Text = "SIF ou FOB";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(3, 624);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(338, 78);
            this.label10.TabIndex = 12;
            this.label10.Text = "Data Baixa";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(3, 546);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(338, 78);
            this.label7.TabIndex = 8;
            this.label7.Text = "Data de vencimento";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 468);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(338, 78);
            this.label6.TabIndex = 7;
            this.label6.Text = "Data de Ocorrencia";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(3, 390);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(338, 78);
            this.label11.TabIndex = 13;
            this.label11.Text = "Destinatario Final";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(3, 312);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(338, 78);
            this.label5.TabIndex = 6;
            this.label5.Text = "Destino";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(3, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(338, 78);
            this.label4.TabIndex = 5;
            this.label4.Text = "Origem";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel11
            // 
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Controls.Add(this.cmbStatus);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(347, 783);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(604, 80);
            this.panel11.TabIndex = 21;
            // 
            // cmbStatus
            // 
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(3, 28);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(598, 29);
            this.cmbStatus.TabIndex = 0;
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.rbFOB);
            this.panel10.Controls.Add(this.rbSIF);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(347, 705);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(604, 72);
            this.panel10.TabIndex = 20;
            // 
            // rbFOB
            // 
            this.rbFOB.AutoSize = true;
            this.rbFOB.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.rbFOB.Location = new System.Drawing.Point(66, 26);
            this.rbFOB.Name = "rbFOB";
            this.rbFOB.Size = new System.Drawing.Size(58, 25);
            this.rbFOB.TabIndex = 1;
            this.rbFOB.TabStop = true;
            this.rbFOB.Text = "FOB";
            this.rbFOB.UseVisualStyleBackColor = true;
            // 
            // rbSIF
            // 
            this.rbSIF.AutoSize = true;
            this.rbSIF.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSIF.Location = new System.Drawing.Point(3, 26);
            this.rbSIF.Name = "rbSIF";
            this.rbSIF.Size = new System.Drawing.Size(51, 25);
            this.rbSIF.TabIndex = 0;
            this.rbSIF.TabStop = true;
            this.rbSIF.Text = "CIF";
            this.rbSIF.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.dtpBaixa);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(347, 627);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(604, 72);
            this.panel9.TabIndex = 19;
            // 
            // dtpBaixa
            // 
            this.dtpBaixa.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpBaixa.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBaixa.Location = new System.Drawing.Point(3, 24);
            this.dtpBaixa.Name = "dtpBaixa";
            this.dtpBaixa.Size = new System.Drawing.Size(598, 29);
            this.dtpBaixa.TabIndex = 1;
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.dtpVencimento);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(347, 549);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(604, 72);
            this.panel8.TabIndex = 18;
            // 
            // dtpVencimento
            // 
            this.dtpVencimento.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpVencimento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVencimento.Location = new System.Drawing.Point(3, 24);
            this.dtpVencimento.Name = "dtpVencimento";
            this.dtpVencimento.Size = new System.Drawing.Size(598, 29);
            this.dtpVencimento.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.dtpOcorrencia);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(347, 471);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(604, 72);
            this.panel7.TabIndex = 17;
            // 
            // dtpOcorrencia
            // 
            this.dtpOcorrencia.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.dtpOcorrencia.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpOcorrencia.Location = new System.Drawing.Point(3, 22);
            this.dtpOcorrencia.Name = "dtpOcorrencia";
            this.dtpOcorrencia.Size = new System.Drawing.Size(598, 29);
            this.dtpOcorrencia.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.txtDestinoFinal);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(347, 393);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(604, 72);
            this.panel6.TabIndex = 16;
            // 
            // txtDestinoFinal
            // 
            this.txtDestinoFinal.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtDestinoFinal.Location = new System.Drawing.Point(3, 24);
            this.txtDestinoFinal.Name = "txtDestinoFinal";
            this.txtDestinoFinal.Size = new System.Drawing.Size(598, 29);
            this.txtDestinoFinal.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.txtDestino);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(347, 315);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(604, 72);
            this.panel4.TabIndex = 14;
            // 
            // txtDestino
            // 
            this.txtDestino.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtDestino.Location = new System.Drawing.Point(3, 24);
            this.txtDestino.Name = "txtDestino";
            this.txtDestino.Size = new System.Drawing.Size(598, 29);
            this.txtDestino.TabIndex = 3;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.txtOrigem);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(347, 237);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(604, 72);
            this.panel5.TabIndex = 15;
            // 
            // txtOrigem
            // 
            this.txtOrigem.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtOrigem.Location = new System.Drawing.Point(3, 28);
            this.txtOrigem.Name = "txtOrigem";
            this.txtOrigem.Size = new System.Drawing.Size(598, 29);
            this.txtOrigem.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(3, 156);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(338, 78);
            this.label12.TabIndex = 24;
            this.label12.Text = "Valor Frete";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel14
            // 
            this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel14.Controls.Add(this.txtValorFrete);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel14.Location = new System.Drawing.Point(347, 159);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(604, 72);
            this.panel14.TabIndex = 25;
            // 
            // txtValorFrete
            // 
            this.txtValorFrete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValorFrete.Location = new System.Drawing.Point(3, 23);
            this.txtValorFrete.Name = "txtValorFrete";
            this.txtValorFrete.Size = new System.Drawing.Size(598, 29);
            this.txtValorFrete.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.panel3.Controls.Add(this.tableLayoutPanel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 866);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(954, 77);
            this.panel3.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.btnSalvar, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnFinalizado, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnExcluirLinha, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnNovaLinha, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(954, 77);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // btnSalvar
            // 
            this.btnSalvar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSalvar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvar.Location = new System.Drawing.Point(717, 3);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(234, 71);
            this.btnSalvar.TabIndex = 3;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            // 
            // btnFinalizado
            // 
            this.btnFinalizado.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFinalizado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinalizado.Location = new System.Drawing.Point(479, 3);
            this.btnFinalizado.Name = "btnFinalizado";
            this.btnFinalizado.Size = new System.Drawing.Size(232, 71);
            this.btnFinalizado.TabIndex = 2;
            this.btnFinalizado.Text = "Finalizado";
            this.btnFinalizado.UseVisualStyleBackColor = true;
            // 
            // btnExcluirLinha
            // 
            this.btnExcluirLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExcluirLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcluirLinha.Location = new System.Drawing.Point(241, 3);
            this.btnExcluirLinha.Name = "btnExcluirLinha";
            this.btnExcluirLinha.Size = new System.Drawing.Size(232, 71);
            this.btnExcluirLinha.TabIndex = 1;
            this.btnExcluirLinha.Text = "Excluir Linha";
            this.btnExcluirLinha.UseVisualStyleBackColor = true;
            // 
            // btnNovaLinha
            // 
            this.btnNovaLinha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNovaLinha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNovaLinha.Location = new System.Drawing.Point(3, 3);
            this.btnNovaLinha.Name = "btnNovaLinha";
            this.btnNovaLinha.Size = new System.Drawing.Size(232, 71);
            this.btnNovaLinha.TabIndex = 0;
            this.btnNovaLinha.Text = "Nova Linha";
            this.btnNovaLinha.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.panel15, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(963, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.559915F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.44009F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(954, 943);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // dgvFrete
            // 
            this.dgvFrete.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvFrete.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFrete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFrete.Location = new System.Drawing.Point(3, 3);
            this.dgvFrete.Name = "dgvFrete";
            this.dgvFrete.Size = new System.Drawing.Size(942, 422);
            this.dgvFrete.TabIndex = 0;
            // 
            // panel15
            // 
            this.panel15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(52)))), ((int)(((byte)(105)))));
            this.panel15.Controls.Add(this.label13);
            this.panel15.Controls.Add(this.txtFiltro);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(3, 3);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(948, 37);
            this.panel15.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.Location = new System.Drawing.Point(3, 3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 30);
            this.label13.TabIndex = 1;
            this.label13.Text = "Filtrar";
            // 
            // txtFiltro
            // 
            this.txtFiltro.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFiltro.Location = new System.Drawing.Point(72, 3);
            this.txtFiltro.Name = "txtFiltro";
            this.txtFiltro.Size = new System.Drawing.Size(870, 29);
            this.txtFiltro.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.dgvFrete, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel16, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.dgvFretesFiltrados, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 46);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.88874F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.88903F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.222235F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(948, 894);
            this.tableLayoutPanel4.TabIndex = 2;
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(52)))), ((int)(((byte)(105)))));
            this.panel16.Controls.Add(this.lblPeriodo);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel16.Location = new System.Drawing.Point(3, 859);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(942, 32);
            this.panel16.TabIndex = 1;
            // 
            // dgvFretesFiltrados
            // 
            this.dgvFretesFiltrados.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.dgvFretesFiltrados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFretesFiltrados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFretesFiltrados.Location = new System.Drawing.Point(3, 431);
            this.dgvFretesFiltrados.Name = "dgvFretesFiltrados";
            this.dgvFretesFiltrados.Size = new System.Drawing.Size(942, 422);
            this.dgvFretesFiltrados.TabIndex = 2;
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPeriodo.ForeColor = System.Drawing.Color.White;
            this.lblPeriodo.Location = new System.Drawing.Point(3, 1);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(238, 30);
            this.lblPeriodo.TabIndex = 0;
            this.lblPeriodo.Text = "Lançamento de 30 dias";
            // 
            // ExpedicaoFormFrete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1920, 1005);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExpedicaoFormFrete";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormFrete";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.LayoutMenu.ResumeLayout(false);
            this.LayoutMenu.PerformLayout();
            this.panel13.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel11.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFrete)).EndInit();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.panel16.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFretesFiltrados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvFrete;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtGerador;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel LayoutMenu;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbTransportadora;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DateTimePicker dtpOcorrencia;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox txtDestinoFinal;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txtOrigem;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtDestino;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.RadioButton rbFOB;
        private System.Windows.Forms.RadioButton rbSIF;
        private System.Windows.Forms.DateTimePicker dtpBaixa;
        private System.Windows.Forms.DateTimePicker dtpVencimento;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnFinalizado;
        private System.Windows.Forms.Button btnExcluirLinha;
        private System.Windows.Forms.Button btnNovaLinha;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.TextBox txtValorFrete;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtFiltro;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.DataGridView dgvFretesFiltrados;
        private System.Windows.Forms.Label lblPeriodo;
    }
}