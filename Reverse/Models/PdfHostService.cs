using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Reverse
{
    public class PdfHostService : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly string _senhaDownload;
        private readonly string _baseUrl;

        private readonly Dictionary<string, string> _pdfs
            = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly object _pdfsLock = new object();

        private readonly Dictionary<string, DateTime> _sessoes
            = new Dictionary<string, DateTime>();
        private readonly object _sessoesLock = new object();

        private readonly Dictionary<string, (int tentativas, DateTime bloqueadoAte)> _bloqueios
            = new Dictionary<string, (int, DateTime)>();
        private readonly object _bloqueiosLock = new object();

        private Thread _thread;
        private bool _disposed;

        public PdfHostService()
        {
            _senhaDownload = System.Configuration.ConfigurationManager
                                   .AppSettings["PdfDownloadSenha"];
            if (string.IsNullOrWhiteSpace(_senhaDownload))
                throw new InvalidOperationException(
                    "A chave 'PdfDownloadSenha' nao esta configurada no App.config.\n" +
                    "Adicione: <add key=\"PdfDownloadSenha\" value=\"SuaSenha\" />");

            string portaStr = System.Configuration.ConfigurationManager
                                    .AppSettings["PdfPorta"];
            int porta = int.TryParse(portaStr, out int p) && p > 0 && p < 65536 ? p : 8765;

            _listener = new TcpListener(IPAddress.Any, porta);
            _listener.Start();

            _baseUrl = "http://" + ObterIpLocal() + ":" + porta;

            new Timer(_ =>
            {
                lock (_sessoesLock)
                {
                    var expiradas = _sessoes
                        .Where(kv => DateTime.Now >= kv.Value)
                        .Select(kv => kv.Key).ToList();
                    foreach (var k in expiradas) _sessoes.Remove(k);
                }
            }, null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
        }

        public string RegistrarPdf(string filePath)
        {
            string id = GerarId(filePath);
            lock (_pdfsLock)
                _pdfs[id] = filePath;
            return _baseUrl + "/?id=" + id;
        }

        public void Iniciar()
        {
            _thread = new Thread(Processar) { IsBackground = true };
            _thread.Start();
        }

        private void Processar()
        {
            while (!_disposed)
            {
                try
                {
                    if (!_listener.Pending()) { Thread.Sleep(50); continue; }
                    TcpClient client = _listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(_ => Tratar(client));
                }
                catch (SocketException) { break; }
                catch { }
            }
        }

        private void Tratar(TcpClient client)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    stream.ReadTimeout = 8000;

                    var sb = new StringBuilder();
                    while (true)
                    {
                        var linha = LerLinha(stream);
                        if (linha == null || linha == "") break;
                        sb.AppendLine(linha);
                    }

                    string headersRaw = sb.ToString();
                    string metodo = ExtrairMetodo(headersRaw);
                    string path = ExtrairPath(headersRaw);
                    string queryStr = ExtrairQuery(headersRaw);
                    string cookies = ExtrairHeader(headersRaw, "Cookie");
                    string clienteIp = ((IPEndPoint)client.Client.RemoteEndPoint)
                                           ?.Address.ToString() ?? "desconhecido";
                    string pdfId = ExtrairQueryParam(queryStr, "id");

                    string body = "";
                    if (metodo == "POST")
                    {
                        string clStr = ExtrairHeader(headersRaw, "Content-Length");
                        if (int.TryParse(clStr, out int cl) && cl > 0 && cl < 4096)
                        {
                            byte[] buf = new byte[cl];
                            int lidos = 0;
                            while (lidos < cl)
                            {
                                int n = stream.Read(buf, lidos, cl - lidos);
                                if (n == 0) break;
                                lidos += n;
                            }
                            body = Encoding.UTF8.GetString(buf, 0, lidos);
                        }
                    }

                    byte[] resposta;

                    if (metodo == "GET" && path == "/")
                    {
                        if (!string.IsNullOrEmpty(pdfId) && !PdfRegistrado(pdfId))
                            resposta = RespostaErroIdDesconhecido();
                        else
                        {
                            string token = ExtrairCookie(cookies, "rv_session");
                            resposta = SessaoValida(token)
                                ? RespostaRedirect("/download?id=" + pdfId, null)
                                : RespostaPaginaLogin(pdfId, false);
                        }
                    }
                    else if (metodo == "POST" && path == "/login")
                    {
                        string idNoBody = ExtrairFormField(body, "id");
                        if (EstaBloqueado(clienteIp))
                        {
                            resposta = RespostaPaginaLogin(idNoBody, true, true);
                        }
                        else
                        {
                            string senha = ExtrairFormField(body, "senha");
                            if (ValidarSenha(senha, clienteIp))
                            {
                                string novoToken = Guid.NewGuid().ToString("N");
                                lock (_sessoesLock)
                                    _sessoes[novoToken] = DateTime.Now.AddHours(8);
                                resposta = RespostaRedirect("/download?id=" + idNoBody, novoToken);
                            }
                            else
                            {
                                resposta = RespostaPaginaLogin(idNoBody, true, false);
                            }
                        }
                    }
                    else if (metodo == "GET" && path == "/download")
                    {
                        string token = ExtrairCookie(cookies, "rv_session");
                        if (!SessaoValida(token))
                            resposta = RespostaRedirect("/?id=" + pdfId, null);
                        else
                            resposta = RespostaPdf(pdfId);
                    }
                    else
                    {
                        resposta = RespostaRedirect("/", null);
                    }

                    stream.Write(resposta, 0, resposta.Length);
                    stream.Flush();
                }
            }
            catch { }
        }

        private static string GerarId(string filePath)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(
                    filePath.ToLowerInvariant().Replace('\\', '/')));
                return Convert.ToBase64String(hash)
                    .Replace('+', '-').Replace('/', '_')
                    .Substring(0, 16);
            }
        }

        private bool PdfRegistrado(string id)
        {
            lock (_pdfsLock) return _pdfs.ContainsKey(id);
        }

        private string ResolverCaminho(string id)
        {
            lock (_pdfsLock)
                return _pdfs.TryGetValue(id, out var path) ? path : null;
        }

        private bool ValidarSenha(string senhaDigitada, string clienteIp)
        {
            if (string.IsNullOrWhiteSpace(senhaDigitada))
            {
                RegistrarTentativaFalha(clienteIp);
                return false;
            }
            bool ok = string.Equals(senhaDigitada, _senhaDownload, StringComparison.Ordinal);
            if (ok) LimparBloqueio(clienteIp);
            else RegistrarTentativaFalha(clienteIp);
            return ok;
        }

        private bool EstaBloqueado(string ip)
        {
            lock (_bloqueiosLock)
            {
                if (_bloqueios.TryGetValue(ip, out var est))
                {
                    if (DateTime.Now < est.bloqueadoAte) return true;
                    if (est.tentativas >= 5) _bloqueios.Remove(ip);
                }
            }
            return false;
        }

        private void RegistrarTentativaFalha(string ip)
        {
            lock (_bloqueiosLock)
            {
                _bloqueios.TryGetValue(ip, out var est);
                int novas = est.tentativas + 1;
                DateTime bl = novas >= 5 ? DateTime.Now.AddMinutes(5) : DateTime.MinValue;
                _bloqueios[ip] = (novas, bl);
            }
        }

        private void LimparBloqueio(string ip)
        {
            lock (_bloqueiosLock) _bloqueios.Remove(ip);
        }

        private bool SessaoValida(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            lock (_sessoesLock)
            {
                if (_sessoes.TryGetValue(token, out var val))
                {
                    if (DateTime.Now < val) return true;
                    _sessoes.Remove(token);
                }
            }
            return false;
        }

        // ── Respostas HTTP ────────────────────────────────────────────────────

        private byte[] RespostaPaginaLogin(string pdfId, bool erro, bool bloqueado = false)
        {
            string msgErro = "";
            if (bloqueado)
                msgErro = "<p class='erro'>Muitas tentativas incorretas. Aguarde 5 minutos.</p>";
            else if (erro)
                msgErro = "<p class='erro'>Senha incorreta. Tente novamente.</p>";

            // id extraido para variavel antes da string — necessario porque chamar
            // metodos dentro de $@"..." com aspas simples HTML ao redor causa erro de compilacao
            string idSeguro = HtmlEncode(pdfId ?? "");

            string html =
                "<!DOCTYPE html>\n" +
                "<html lang='pt-BR'>\n" +
                "<head>\n" +
                "  <meta charset='UTF-8'>\n" +
                "  <meta name='viewport' content='width=device-width, initial-scale=1'>\n" +
                "  <title>Reverse - Download de Palete</title>\n" +
                "  <style>\n" +
                "    * { box-sizing: border-box; margin: 0; padding: 0; }\n" +
                "    body {\n" +
                "      font-family: 'Segoe UI', sans-serif;\n" +
                "      background: linear-gradient(135deg, #34495e, #2980b9);\n" +
                "      min-height: 100vh;\n" +
                "      display: flex; align-items: center; justify-content: center;\n" +
                "    }\n" +
                "    .card {\n" +
                "      background: white; border-radius: 12px;\n" +
                "      padding: 40px 36px; width: 100%; max-width: 360px;\n" +
                "      box-shadow: 0 8px 32px rgba(0,0,0,0.25);\n" +
                "    }\n" +
                "    h1 { font-size: 22px; color: #2c3e50; margin-bottom: 6px; }\n" +
                "    .sub { font-size: 13px; color: #7f8c8d; margin-bottom: 28px; }\n" +
                "    label { font-size: 13px; font-weight: 600; color: #34495e; display: block; margin-bottom: 5px; }\n" +
                "    input[type=password] {\n" +
                "      width: 100%; padding: 11px 14px;\n" +
                "      border: 1.5px solid #dde1e7; border-radius: 8px;\n" +
                "      font-size: 15px; margin-bottom: 18px;\n" +
                "      outline: none; transition: border .2s;\n" +
                "    }\n" +
                "    input[type=password]:focus { border-color: #2980b9; }\n" +
                "    button {\n" +
                "      width: 100%; padding: 13px;\n" +
                "      background: #2980b9; color: white; border: none;\n" +
                "      border-radius: 8px; font-size: 15px; font-weight: 600; cursor: pointer;\n" +
                "    }\n" +
                "    button:active { background: #2471a3; }\n" +
                "    .erro { background: #fdecea; color: #c0392b; border-radius: 7px; padding: 10px 14px; font-size: 13px; margin-bottom: 18px; }\n" +
                "    .lock { font-size: 40px; text-align: center; margin-bottom: 16px; }\n" +
                "  </style>\n" +
                "</head>\n" +
                "<body>\n" +
                "  <div class='card'>\n" +
                "    <div class='lock'>&#128274;</div>\n" +
                "    <h1>Download da Palete</h1>\n" +
                "    <p class='sub'>Digite a senha para baixar o PDF.</p>\n" +
                "    " + msgErro + "\n" +
                "    <form method='POST' action='/login'>\n" +
                "      <input type='hidden' name='id' value='" + idSeguro + "'>\n" +
                "      <label>Senha</label>\n" +
                "      <input type='password' name='senha' autocomplete='current-password' required autofocus>\n" +
                "      <button type='submit'>Baixar PDF</button>\n" +
                "    </form>\n" +
                "  </div>\n" +
                "</body>\n" +
                "</html>";

            return MontarRespostaHtml(200, "OK", html);
        }

        private byte[] RespostaPdf(string pdfId)
        {
            string filePath = ResolverCaminho(pdfId);

            if (filePath == null || !File.Exists(filePath))
            {
                string html404 =
                    "<!DOCTYPE html><html lang='pt-BR'><head><meta charset='UTF-8'>" +
                    "<meta name='viewport' content='width=device-width, initial-scale=1'>" +
                    "<title>PDF nao encontrado</title><style>" +
                    "* { box-sizing: border-box; margin: 0; padding: 0; }" +
                    "body { font-family: 'Segoe UI', sans-serif; background: linear-gradient(135deg, #34495e, #2980b9); min-height: 100vh; display: flex; align-items: center; justify-content: center; }" +
                    ".card { background: white; border-radius: 12px; padding: 40px 36px; text-align: center; max-width: 380px; width: 100%; box-shadow: 0 8px 32px rgba(0,0,0,0.25); }" +
                    ".icon { font-size: 48px; margin-bottom: 16px; }" +
                    "h2 { color: #c0392b; margin-bottom: 10px; }" +
                    "p { color: #7f8c8d; font-size: 14px; line-height: 1.6; }" +
                    "</style></head><body><div class='card'>" +
                    "<div class='icon'>&#9888;&#65039;</div>" +
                    "<h2>PDF nao disponivel</h2>" +
                    "<p>O arquivo nao foi encontrado no servidor.<br><br>" +
                    "Gere o PDF novamente usando o botao <strong>Exportar PDF</strong> no sistema Reverse.</p>" +
                    "</div></body></html>";
                return MontarRespostaHtml(404, "Not Found", html404);
            }

            string fileName = Path.GetFileName(filePath);
            byte[] pdfBytes = File.ReadAllBytes(filePath);

            string header =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: application/pdf\r\n" +
                "Content-Length: " + pdfBytes.Length + "\r\n" +
                "Content-Disposition: attachment; filename=\"" + fileName + "\"\r\n" +
                "Cache-Control: no-store\r\n" +
                "Connection: close\r\n\r\n";

            byte[] hb = Encoding.UTF8.GetBytes(header);
            byte[] resp = new byte[hb.Length + pdfBytes.Length];
            Buffer.BlockCopy(hb, 0, resp, 0, hb.Length);
            Buffer.BlockCopy(pdfBytes, 0, resp, hb.Length, pdfBytes.Length);
            return resp;
        }

        private byte[] RespostaErroIdDesconhecido()
        {
            string html =
                "<!DOCTYPE html><html lang='pt-BR'><head><meta charset='UTF-8'>" +
                "<meta name='viewport' content='width=device-width, initial-scale=1'>" +
                "<title>QR Code invalido</title><style>" +
                "* { box-sizing: border-box; margin: 0; padding: 0; }" +
                "body { font-family: 'Segoe UI', sans-serif; background: linear-gradient(135deg, #34495e, #2980b9); min-height: 100vh; display: flex; align-items: center; justify-content: center; }" +
                ".card { background: white; border-radius: 12px; padding: 40px 36px; text-align: center; max-width: 380px; width: 100%; box-shadow: 0 8px 32px rgba(0,0,0,0.25); }" +
                ".icon { font-size: 48px; margin-bottom: 16px; }" +
                "h2 { color: #c0392b; margin-bottom: 10px; }" +
                "p { color: #7f8c8d; font-size: 14px; line-height: 1.6; }" +
                "</style></head><body><div class='card'>" +
                "<div class='icon'>&#128279;</div>" +
                "<h2>QR Code expirado</h2>" +
                "<p>Este QR Code nao e mais valido nesta sessao do programa.<br><br>" +
                "Reabra a palete no sistema Reverse e exporte o PDF novamente.</p>" +
                "</div></body></html>";
            return MontarRespostaHtml(404, "Not Found", html);
        }

        private static byte[] RespostaRedirect(string destino, string novoToken)
        {
            string setCookie = novoToken != null
                ? "Set-Cookie: rv_session=" + novoToken + "; Path=/; HttpOnly\r\n"
                : "";

            string header =
                "HTTP/1.1 302 Found\r\n" +
                "Location: " + destino + "\r\n" +
                setCookie +
                "Content-Length: 0\r\n" +
                "Connection: close\r\n\r\n";

            return Encoding.UTF8.GetBytes(header);
        }

        private static byte[] MontarRespostaHtml(int status, string statusText, string html)
        {
            byte[] body = Encoding.UTF8.GetBytes(html);
            string header =
                "HTTP/1.1 " + status + " " + statusText + "\r\n" +
                "Content-Type: text/html; charset=utf-8\r\n" +
                "Content-Length: " + body.Length + "\r\n" +
                "Connection: close\r\n\r\n";
            byte[] hb = Encoding.UTF8.GetBytes(header);
            byte[] resp = new byte[hb.Length + body.Length];
            Buffer.BlockCopy(hb, 0, resp, 0, hb.Length);
            Buffer.BlockCopy(body, 0, resp, hb.Length, body.Length);
            return resp;
        }

        private static string HtmlEncode(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("'", "&#39;");
        }

        private static string LerLinha(NetworkStream stream)
        {
            var sb2 = new StringBuilder();
            int b;
            while ((b = stream.ReadByte()) != -1)
            {
                if (b == '\n') return sb2.ToString().TrimEnd('\r');
                sb2.Append((char)b);
            }
            return sb2.Length > 0 ? sb2.ToString() : null;
        }

        private static string ExtrairMetodo(string raw)
        {
            try { return raw.Split(' ')[0].ToUpper().Trim(); } catch { return "GET"; }
        }

        private static string ExtrairPath(string raw)
        {
            try
            {
                string pathWithQuery = raw.Split(' ')[1];
                int q = pathWithQuery.IndexOf('?');
                return q >= 0 ? pathWithQuery.Substring(0, q) : pathWithQuery;
            }
            catch { return "/"; }
        }

        private static string ExtrairQuery(string raw)
        {
            try
            {
                string pathWithQuery = raw.Split(' ')[1];
                int q = pathWithQuery.IndexOf('?');
                return q >= 0 ? pathWithQuery.Substring(q + 1) : "";
            }
            catch { return ""; }
        }

        private static string ExtrairQueryParam(string query, string param)
        {
            if (string.IsNullOrEmpty(query)) return "";
            foreach (var par in query.Split('&'))
            {
                var kv = par.Split('=');
                if (kv.Length == 2 && kv[0] == param)
                    return Uri.UnescapeDataString(kv[1]);
            }
            return "";
        }

        private static string ExtrairHeader(string raw, string nome)
        {
            foreach (var linha in raw.Split('\n'))
            {
                if (linha.StartsWith(nome + ":", StringComparison.OrdinalIgnoreCase))
                    return linha.Substring(nome.Length + 1).Trim();
            }
            return "";
        }

        private static string ExtrairFormField(string body, string campo)
        {
            foreach (var par in body.Split('&'))
            {
                var kv = par.Split('=');
                if (kv.Length == 2 && Uri.UnescapeDataString(kv[0]) == campo)
                    return Uri.UnescapeDataString(kv[1].Replace("+", " "));
            }
            return "";
        }

        private static string ExtrairCookie(string cookieHeader, string nome)
        {
            if (string.IsNullOrEmpty(cookieHeader)) return null;
            foreach (var par in cookieHeader.Split(';'))
            {
                var kv = par.Trim().Split('=');
                if (kv.Length == 2 && kv[0].Trim() == nome)
                    return kv[1].Trim();
            }
            return null;
        }

        private static string ObterIpLocal()
        {
            try
            {
                using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.Connect("8.8.8.8", 65530);
                    return ((IPEndPoint)s.LocalEndPoint).Address.ToString();
                }
            }
            catch { return "127.0.0.1"; }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _listener.Stop(); } catch { }
        }
    }
}