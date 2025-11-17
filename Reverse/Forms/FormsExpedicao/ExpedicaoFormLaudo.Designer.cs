namespace Reverse.Forms.FormsExpedicao
{
    partial class ExpedicaoFormLaudo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpedicaoFormLaudo));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlEditor = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.btnExportarLaudo = new System.Windows.Forms.Button();
            this.tblPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.txtLODLD = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.txtIEDLD = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.txtCNPJDLD = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.txtEnderecoDLD = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtRazaoDLD = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.txtEstadoFisico = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.txtClasse = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtAcondicionamento = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtConama = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtNomeComum = new System.Windows.Forms.TextBox();
            this.pnlVisualizer = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlEditor.SuspendLayout();
            this.panel12.SuspendLayout();
            this.tblPanel.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(52)))), ((int)(((byte)(105)))));
            this.panel1.Controls.Add(this.btnSair);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1500, 56);
            this.panel1.TabIndex = 6;
            // 
            // btnSair
            // 
            this.btnSair.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.Location = new System.Drawing.Point(1439, 0);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(61, 56);
            this.btnSair.TabIndex = 3;
            this.btnSair.Text = "X";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 37);
            this.label1.TabIndex = 2;
            this.label1.Text = "Laudo";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pnlEditor, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlVisualizer, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 56);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1500, 794);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // pnlEditor
            // 
            this.pnlEditor.Controls.Add(this.panel12);
            this.pnlEditor.Controls.Add(this.tblPanel);
            this.pnlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEditor.Location = new System.Drawing.Point(3, 3);
            this.pnlEditor.Name = "pnlEditor";
            this.pnlEditor.Size = new System.Drawing.Size(744, 788);
            this.pnlEditor.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.btnExportarLaudo);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(0, 720);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(744, 68);
            this.panel12.TabIndex = 1;
            // 
            // btnExportarLaudo
            // 
            this.btnExportarLaudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportarLaudo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportarLaudo.ForeColor = System.Drawing.Color.Black;
            this.btnExportarLaudo.Location = new System.Drawing.Point(0, 0);
            this.btnExportarLaudo.Name = "btnExportarLaudo";
            this.btnExportarLaudo.Size = new System.Drawing.Size(744, 68);
            this.btnExportarLaudo.TabIndex = 0;
            this.btnExportarLaudo.Text = "Exportar Laudo";
            this.btnExportarLaudo.UseVisualStyleBackColor = true;
            // 
            // tblPanel
            // 
            this.tblPanel.ColumnCount = 2;
            this.tblPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.37634F));
            this.tblPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.62366F));
            this.tblPanel.Controls.Add(this.panel11, 1, 9);
            this.tblPanel.Controls.Add(this.label11, 0, 9);
            this.tblPanel.Controls.Add(this.panel10, 1, 8);
            this.tblPanel.Controls.Add(this.label10, 0, 8);
            this.tblPanel.Controls.Add(this.panel9, 1, 7);
            this.tblPanel.Controls.Add(this.label9, 0, 7);
            this.tblPanel.Controls.Add(this.panel8, 1, 6);
            this.tblPanel.Controls.Add(this.label8, 0, 6);
            this.tblPanel.Controls.Add(this.panel7, 1, 5);
            this.tblPanel.Controls.Add(this.label7, 0, 5);
            this.tblPanel.Controls.Add(this.panel6, 1, 4);
            this.tblPanel.Controls.Add(this.label5, 0, 4);
            this.tblPanel.Controls.Add(this.panel5, 1, 3);
            this.tblPanel.Controls.Add(this.label3, 0, 3);
            this.tblPanel.Controls.Add(this.panel4, 1, 2);
            this.tblPanel.Controls.Add(this.panel3, 1, 1);
            this.tblPanel.Controls.Add(this.label6, 0, 2);
            this.tblPanel.Controls.Add(this.label4, 0, 1);
            this.tblPanel.Controls.Add(this.label2, 0, 0);
            this.tblPanel.Controls.Add(this.panel2, 1, 0);
            this.tblPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblPanel.Location = new System.Drawing.Point(0, 0);
            this.tblPanel.Name = "tblPanel";
            this.tblPanel.RowCount = 10;
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tblPanel.Size = new System.Drawing.Size(744, 720);
            this.tblPanel.TabIndex = 0;
            // 
            // panel11
            // 
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Controls.Add(this.txtLODLD);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(228, 651);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(513, 66);
            this.panel11.TabIndex = 22;
            // 
            // txtLODLD
            // 
            this.txtLODLD.Location = new System.Drawing.Point(3, 17);
            this.txtLODLD.Name = "txtLODLD";
            this.txtLODLD.Size = new System.Drawing.Size(504, 33);
            this.txtLODLD.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 648);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(219, 72);
            this.label11.TabIndex = 21;
            this.label11.Text = "LO DLD";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.txtIEDLD);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(228, 579);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(513, 66);
            this.panel10.TabIndex = 20;
            // 
            // txtIEDLD
            // 
            this.txtIEDLD.Location = new System.Drawing.Point(3, 17);
            this.txtIEDLD.Name = "txtIEDLD";
            this.txtIEDLD.Size = new System.Drawing.Size(504, 33);
            this.txtIEDLD.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(3, 576);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(219, 72);
            this.label10.TabIndex = 19;
            this.label10.Text = "IE DLD";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.txtCNPJDLD);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(228, 507);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(513, 66);
            this.panel9.TabIndex = 18;
            // 
            // txtCNPJDLD
            // 
            this.txtCNPJDLD.Location = new System.Drawing.Point(3, 17);
            this.txtCNPJDLD.Name = "txtCNPJDLD";
            this.txtCNPJDLD.Size = new System.Drawing.Size(504, 33);
            this.txtCNPJDLD.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(3, 504);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(219, 72);
            this.label9.TabIndex = 17;
            this.label9.Text = "CNPJ DLD";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.txtEnderecoDLD);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(228, 435);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(513, 66);
            this.panel8.TabIndex = 16;
            // 
            // txtEnderecoDLD
            // 
            this.txtEnderecoDLD.Location = new System.Drawing.Point(3, 17);
            this.txtEnderecoDLD.Name = "txtEnderecoDLD";
            this.txtEnderecoDLD.Size = new System.Drawing.Size(504, 33);
            this.txtEnderecoDLD.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(3, 432);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(219, 72);
            this.label8.TabIndex = 15;
            this.label8.Text = "Endereço DLD";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtRazaoDLD);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(228, 363);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(513, 66);
            this.panel7.TabIndex = 14;
            // 
            // txtRazaoDLD
            // 
            this.txtRazaoDLD.Location = new System.Drawing.Point(3, 17);
            this.txtRazaoDLD.Name = "txtRazaoDLD";
            this.txtRazaoDLD.Size = new System.Drawing.Size(504, 33);
            this.txtRazaoDLD.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 360);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(219, 72);
            this.label7.TabIndex = 13;
            this.label7.Text = "Razão Social DLD";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.txtEstadoFisico);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(228, 291);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(513, 66);
            this.panel6.TabIndex = 12;
            // 
            // txtEstadoFisico
            // 
            this.txtEstadoFisico.Location = new System.Drawing.Point(3, 17);
            this.txtEstadoFisico.Name = "txtEstadoFisico";
            this.txtEstadoFisico.Size = new System.Drawing.Size(504, 33);
            this.txtEstadoFisico.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 288);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(219, 72);
            this.label5.TabIndex = 11;
            this.label5.Text = "Estado Fisico";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.txtClasse);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(228, 219);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(513, 66);
            this.panel5.TabIndex = 10;
            // 
            // txtClasse
            // 
            this.txtClasse.Location = new System.Drawing.Point(3, 17);
            this.txtClasse.Name = "txtClasse";
            this.txtClasse.Size = new System.Drawing.Size(504, 33);
            this.txtClasse.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(219, 72);
            this.label3.TabIndex = 9;
            this.label3.Text = "Classe";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.txtAcondicionamento);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(228, 147);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(513, 66);
            this.panel4.TabIndex = 8;
            // 
            // txtAcondicionamento
            // 
            this.txtAcondicionamento.Location = new System.Drawing.Point(3, 17);
            this.txtAcondicionamento.Name = "txtAcondicionamento";
            this.txtAcondicionamento.Size = new System.Drawing.Size(504, 33);
            this.txtAcondicionamento.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.txtConama);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(228, 75);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(513, 66);
            this.panel3.TabIndex = 7;
            // 
            // txtConama
            // 
            this.txtConama.Location = new System.Drawing.Point(3, 17);
            this.txtConama.Name = "txtConama";
            this.txtConama.Size = new System.Drawing.Size(504, 33);
            this.txtConama.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(219, 72);
            this.label6.TabIndex = 5;
            this.label6.Text = "Acondicionamento";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 72);
            this.label4.TabIndex = 2;
            this.label4.Text = "Conama 313";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(82)))), ((int)(((byte)(171)))));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(219, 72);
            this.label2.TabIndex = 0;
            this.label2.Text = "Nome Comum";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtNomeComum);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(228, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(513, 66);
            this.panel2.TabIndex = 6;
            // 
            // txtNomeComum
            // 
            this.txtNomeComum.Location = new System.Drawing.Point(3, 17);
            this.txtNomeComum.Name = "txtNomeComum";
            this.txtNomeComum.Size = new System.Drawing.Size(504, 33);
            this.txtNomeComum.TabIndex = 0;
            // 
            // pnlVisualizer
            // 
            this.pnlVisualizer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(243)))), ((int)(((byte)(244)))));
            this.pnlVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVisualizer.Location = new System.Drawing.Point(753, 3);
            this.pnlVisualizer.Name = "pnlVisualizer";
            this.pnlVisualizer.Size = new System.Drawing.Size(744, 788);
            this.pnlVisualizer.TabIndex = 1;
            // 
            // ExpedicaoFormLaudo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(233)))), ((int)(((byte)(235)))));
            this.ClientSize = new System.Drawing.Size(1500, 850);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ExpedicaoFormLaudo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExpedicaoFormLaudo";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlEditor.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.tblPanel.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlEditor;
        private System.Windows.Forms.Panel pnlVisualizer;
        private System.Windows.Forms.TableLayoutPanel tblPanel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btnExportarLaudo;
        private System.Windows.Forms.TextBox txtAcondicionamento;
        private System.Windows.Forms.TextBox txtConama;
        private System.Windows.Forms.TextBox txtNomeComum;
        private System.Windows.Forms.TextBox txtClasse;
        private System.Windows.Forms.TextBox txtLODLD;
        private System.Windows.Forms.TextBox txtIEDLD;
        private System.Windows.Forms.TextBox txtCNPJDLD;
        private System.Windows.Forms.TextBox txtEnderecoDLD;
        private System.Windows.Forms.TextBox txtRazaoDLD;
        private System.Windows.Forms.TextBox txtEstadoFisico;
    }
}