using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using CrmMobile.Data;
using CrmMobile.Models;

namespace CrmMobile.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public AuthService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public Funcionario? Usuario { get; private set; }
    public bool Logado => Usuario != null;
    public event Action? Mudou;

    public async Task<bool> ExisteFuncionarioAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Funcionarios.AnyAsync();
    }

    public async Task<string?> CadastrarAsync(string nome, string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return "Preencha todos os campos.";
        if (senha.Length < 4)
            return "A senha precisa ter pelo menos 4 caracteres.";

        email = email.Trim().ToLowerInvariant();
        await using var db = await _factory.CreateDbContextAsync();
        if (await db.Funcionarios.AnyAsync(f => f.Email == email))
            return "Este e-mail já está cadastrado.";

        db.Funcionarios.Add(new Funcionario
        {
            Nome = nome.Trim(),
            Email = email,
            SenhaHash = Hash(senha),
            Ativo = true
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> EntrarAsync(string email, string senha)
    {
        email = (email ?? "").Trim().ToLowerInvariant();
        await using var db = await _factory.CreateDbContextAsync();
        var f = await db.Funcionarios.FirstOrDefaultAsync(x => x.Email == email);
        if (f == null || !f.Ativo || !Verificar(senha ?? "", f.SenhaHash))
            return "E-mail ou senha incorretos.";

        Usuario = f;
        Mudou?.Invoke();
        return null;
    }

    public void Sair()
    {
        Usuario = null;
        Mudou?.Invoke();
    }

    static string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    static bool Verificar(string senha, string armazenado)
    {
        var partes = armazenado.Split('.');
        if (partes.Length != 2) return false;
        var salt = Convert.FromBase64String(partes[0]);
        var esperado = Convert.FromBase64String(partes[1]);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, esperado);
    }
}