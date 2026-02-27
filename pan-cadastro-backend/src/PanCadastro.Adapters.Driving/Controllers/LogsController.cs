using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace PanCadastro.Adapters.Driving.Controllers;

// Lê arquivos do Serilog (logs/pan-cadastro-*.log).
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LogsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly string _logPath;

    public LogsController(IWebHostEnvironment env)
    {
        _env = env;
        _logPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    }

    // Lista entradas de log com filtros opcionais com filtros.
    [HttpGet]
    [ProducesResponseType(typeof(LogPageResponse), StatusCodes.Status200OK)]
    public IActionResult ObterLogs(
        [FromQuery] string? nivel = null,
        [FromQuery] string? busca = null,
        [FromQuery] string? data = null,
        [FromQuery] int limite = 200)
    {
        if (!_env.IsDevelopment())
            return NotFound("Endpoint disponível apenas em ambiente de desenvolvimento.");

        var dataFiltro = data != null
            ? DateOnly.Parse(data)
            : DateOnly.FromDateTime(DateTime.Now);

        var logFile = Path.Combine(_logPath, $"pan-cadastro-{dataFiltro:yyyyMMdd}.log");

        if (!System.IO.File.Exists(logFile))
        {
            // Tenta encontrar o arquivo com formato alternativo do Serilog
            var pattern = $"pan-cadastro-*{dataFiltro:yyyyMMdd}*";
            var files = Directory.Exists(_logPath)
                ? Directory.GetFiles(_logPath, "pan-cadastro-*.log")
                : Array.Empty<string>();

            logFile = files
                .Where(f => f.Contains(dataFiltro.ToString("yyyyMMdd")))
                .FirstOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(logFile))
            {
                return Ok(new LogPageResponse(
                    Array.Empty<LogEntry>(),
                    0,
                    dataFiltro.ToString("yyyy-MM-dd"),
                    ListarDatasDisponiveis()
                ));
            }
        }

        var entries = ParseLogFile(logFile);

        // Filtro por nível
        if (!string.IsNullOrWhiteSpace(nivel))
        {
            var niveis = nivel.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(n => n.Trim().ToUpper())
                              .ToHashSet();
            entries = entries.Where(e => niveis.Contains(e.Nivel)).ToList();
        }

        // Filtro por texto
        if (!string.IsNullOrWhiteSpace(busca))
        {
            entries = entries
                .Where(e => e.Mensagem.Contains(busca, StringComparison.OrdinalIgnoreCase)
                          || e.Origem.Contains(busca, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Mais recentes primeiro, com limite
        entries = entries.OrderByDescending(e => e.Timestamp).Take(limite).ToList();

        return Ok(new LogPageResponse(
            entries,
            entries.Count,
            dataFiltro.ToString("yyyy-MM-dd"),
            ListarDatasDisponiveis()
        ));
    }

    // Retorna estatísticas resumidas dos logs do dia
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(LogResumo), StatusCodes.Status200OK)]
    public IActionResult ObterResumo([FromQuery] string? data = null)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        var dataFiltro = data != null
            ? DateOnly.Parse(data)
            : DateOnly.FromDateTime(DateTime.Now);

        var logFile = BuscarArquivoLog(dataFiltro);

        if (logFile == null)
        {
            return Ok(new LogResumo(0, 0, 0, 0, dataFiltro.ToString("yyyy-MM-dd")));
        }

        var entries = ParseLogFile(logFile);

        return Ok(new LogResumo(
            entries.Count(e => e.Nivel == "INF"),
            entries.Count(e => e.Nivel == "WRN"),
            entries.Count(e => e.Nivel == "ERR"),
            entries.Count(e => e.Nivel == "FTL"),
            dataFiltro.ToString("yyyy-MM-dd")
        ));
    }

    #region "Métodos auxiliares para leitura e parsing dos arquivos de log do Serilog."

    // Este método tenta extrair o timestamp, nível, mensagem e origem (se possível)
    private List<LogEntry> ParseLogFile(string filePath)
    {
        var entries = new List<LogEntry>();
        var lines = System.IO.File.ReadAllLines(filePath);

        // Pattern Serilog: [HH:mm:ss LEVEL] Message
        var regex = new Regex(@"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\.\d+\s+\+\d{2}:\d{2})\s+\[(INF|WRN|ERR|FTL|DBG|VRB)\]\s*(.+)$");
        LogEntry? current = null;

        foreach (var line in lines)
        {
            var match = regex.Match(line);
            if (match.Success)
            {
                if (current != null)
                    entries.Add(current);

                var hora = match.Groups[1].Value.Substring(11, 8);
                var nivel = match.Groups[2].Value;
                var mensagem = match.Groups[3].Value;

                // Tenta extrair a origem (namespace/classe) da mensagem
                var origem = ExtrairOrigem(mensagem);

                current = new LogEntry(hora, nivel, mensagem, origem, null);
            }
            else if (current != null)
            {
                // Linha de continuação (stack trace, etc.)
                current = current with
                {
                    Detalhes = (current.Detalhes ?? "") + "\n" + line
                };
            }
        }

        if (current != null)
            entries.Add(current);

        return entries;
    }

    // Tenta identificar a origem da mensagem de log com base em palavras-chave comuns.
    private static string ExtrairOrigem(string mensagem)
    {
        // Tenta extrair patterns comuns do Serilog
        if (mensagem.Contains("HTTP"))
            return "HTTP Request";
        if (mensagem.Contains("Pessoa Física") || mensagem.Contains("PessoaFisica"))
            return "PessoaFísica";
        if (mensagem.Contains("Pessoa Jurídica") || mensagem.Contains("PessoaJuridica"))
            return "PessoaJurídica";
        if (mensagem.Contains("Endereço") || mensagem.Contains("Endereco"))
            return "Endereço";
        if (mensagem.Contains("CEP") || mensagem.Contains("ViaCep"))
            return "ViaCEP";
        if (mensagem.Contains("Exceção de domínio") || mensagem.Contains("DomainException"))
            return "Validação";
        if (mensagem.Contains("Erro não tratado"))
            return "Sistema";
        if (mensagem.Contains("Migrat") || mensagem.Contains("Seed"))
            return "Database";
        return "Geral";
    }

    // Busca o arquivo de log correspondente à data, considerando os formatos de nome do Serilog.
    private string? BuscarArquivoLog(DateOnly data)
    {
        if (!Directory.Exists(_logPath))
            return null;

        return Directory.GetFiles(_logPath, "pan-cadastro-*.log")
            .Where(f => f.Contains(data.ToString("yyyyMMdd")))
            .FirstOrDefault();
    }

    // Lista as datas para as quais existem arquivos de log disponíveis, no formato yyyy-MM-dd.
    private string[] ListarDatasDisponiveis()
    {
        if (!Directory.Exists(_logPath))
            return Array.Empty<string>();

        var regex = new Regex(@"pan-cadastro-(\d{8})\.log");

        return Directory.GetFiles(_logPath, "pan-cadastro-*.log")
            .Select(f => regex.Match(Path.GetFileName(f)))
            .Where(m => m.Success)
            .Select(m =>
            {
                var d = m.Groups[1].Value;
                return $"{d[..4]}-{d[4..6]}-{d[6..8]}";
            })
            .OrderByDescending(d => d)
            .ToArray();
    }

    #endregion
}

#region "DTOs"
public record LogEntry(
    string Timestamp,
    string Nivel,
    string Mensagem,
    string Origem,
    string? Detalhes
);

public record LogPageResponse(
    IEnumerable<LogEntry> Registros,
    int Total,
    string Data,
    string[] DatasDisponiveis
);

public record LogResumo(
    int Info,
    int Warning,
    int Error,
    int Fatal,
    string Data
);
#endregion