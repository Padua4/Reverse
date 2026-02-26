using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse
{
    public partial class RegisterForm : Form
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["ReverseDB"].ConnectionString;

        private static readonly string MasterKey =
            ConfigurationManager.AppSettings["MasterKey"];

        private static readonly SHA256 Sha256 = SHA256.Create();

        public RegisterForm()
        {
            InitializeComponent();

            cmbSetor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSetor.Items.AddRange(new[]
            {
                "Triagem",
                "ControleTriagem",
                "Financeiro",
                "RH",
                "Expedição",
                "Almoxarifado",
                "ADM"
            });
            cmbSetor.SelectedIndex = -1;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse
);
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, this.Width, this.Height, 30, 30)
            );
        }

        private async Task<bool> UsuarioExisteAsync(string usuario)
        {
            const string sql = "SELECT COUNT(*) FROM Usuarios WITH (NOLOCK) WHERE UsuarioNome = @user";

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@user", usuario);

            await conn.OpenAsync();
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string user, out string pass, out string confirm,
                               out string key, out string setor))
                return;

            if (key != MasterKey)
            {
                ShowError("Chave-mestre incorreta.", txtMasterKey);
                return;
            }

            if (pass != confirm)
            {
                ShowError("Senhas não conferem.", txtNewSenha, txtConfirmSenha);
                return;
            }

            if (pass.Length < 6)
            {
                ShowError("A senha deve ter no mínimo 6 caracteres.", txtNewSenha);
                return;
            }

            SetUiBusy(true);

            try
            {
                if (await UsuarioExisteAsync(user))
                {
                    ShowError("Este usuário já existe no sistema.", txtNewUsuario);
                    return;
                }

                string hashPass = ComputeSha256Hash(pass);
                await RegisterUserAsync(user, hashPass, setor);

                MessageBox.Show(
                    "Usuário cadastrado com sucesso!",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                ShowError("Este usuário já existe no sistema.", txtNewUsuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao cadastrar usuário:\n" + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetUiBusy(false);
            }
        }

        #region Helpers

        private bool ValidateInputs(
            out string user,
            out string pass,
            out string confirm,
            out string key,
            out string setor)
        {
            user = txtNewUsuario.Text.Trim();
            pass = txtNewSenha.Text;
            confirm = txtConfirmSenha.Text;
            key = txtMasterKey.Text;
            setor = cmbSetor.SelectedItem as string;

            if (new[] { user, pass, confirm, key, setor }.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show(
                    "Preencha todos os campos, incluindo o setor.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            return true;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            byte[] bytes = Sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private async Task RegisterUserAsync(
            string user,
            string hashPass,
            string setor)
        {
            const string sql = @"
INSERT INTO Usuarios
  (UsuarioNome, PasswordHash, Setor)
VALUES
  (
    @user,
    CONVERT(varbinary(max), @passHex, 2),
    @setor
  );
";

            using var conn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.NVarChar, 50) { Value = user });
            cmd.Parameters.Add(new SqlParameter("@passHex", SqlDbType.NVarChar, 128) { Value = hashPass });
            cmd.Parameters.Add(new SqlParameter("@setor", SqlDbType.NVarChar, 50) { Value = setor });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private void ShowError(string message, params Control[] controlsToClear)
        {
            MessageBox.Show(message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            foreach (var ctl in controlsToClear)
            {
                if (ctl is TextBox tb)
                    tb.Clear();
                else
                    ctl.Text = string.Empty;

                ctl.Focus();
            }
        }

        private void SetUiBusy(bool isBusy)
        {
            btnSubmit.Enabled = !isBusy;
            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
        }

        #endregion

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
