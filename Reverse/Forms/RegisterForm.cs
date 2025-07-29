using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Reverse
{
    public partial class RegisterForm : Form
    {
        private const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=Reverse;Trusted_Connection=True;";
        private const string MasterKey = "dld01841";

        // 1) Construtor obrigatório para carregar o Designer
        public RegisterForm()
        {
            InitializeComponent();
        }

        // 2) Método para gerar hash SHA-256
        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // 3) Evento de clique do botão “Cadastrar”
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string user = txtNewUsuario.Text.Trim();
            string pass = txtNewSenha.Text;
            string confirm = txtConfirmSenha.Text;
            string masterKey = txtMasterKey.Text;

            // Validações
            if (string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(pass) ||
                string.IsNullOrEmpty(confirm) ||
                string.IsNullOrEmpty(masterKey))
            {
                MessageBox.Show("Preencha todos os campos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (masterKey != MasterKey)
            {
                MessageBox.Show("Chave-mestre incorreta.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMasterKey.Clear();
                txtMasterKey.Focus();
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Senhas não conferem.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNewSenha.Clear();
                txtConfirmSenha.Clear();
                txtNewSenha.Focus();
                return;
            }

            // Hash da senha
            string hashPass = ComputeSha256Hash(pass);

            // Inserção no banco
            const string sql = @"
                INSERT INTO Usuarios (Usuario, Senha, Setor)
                VALUES (@u, @s, 'Triagem')";

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@u", user);
                cmd.Parameters.AddWithValue("@s", hashPass);

                try
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    MessageBox.Show("Usuário já existe.", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNewUsuario.Clear();
                    txtNewUsuario.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao cadastrar: " + ex.Message, "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
