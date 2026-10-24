using System.IO.Compression;
using System.Text;

namespace CrmMobile.Services;

public static class ArquivoRelatorio
{
    public static byte[] Csv(string[] cab, List<string[]> linhas)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(";", cab.Select(Aspas)));
        foreach (var l in linhas) sb.AppendLine(string.Join(";", l.Select(Aspas)));
        return new UTF8Encoding(true).GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    static string Aspas(string v) => "\"" + v.Replace("\"", "\"\"") + "\"";

    public static byte[] Xlsx(List<string[]> tabela)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            Add(zip, "[Content_Types].xml",
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
            Add(zip, "_rels/.rels",
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
            Add(zip, "xl/workbook.xml",
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                "<sheets><sheet name=\"Relatorio\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
            Add(zip, "xl/_rels/workbook.xml.rels",
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");

            var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");
            for (int i = 0; i < tabela.Count; i++)
            {
                sb.Append($"<row r=\"{i + 1}\">");
                for (int j = 0; j < tabela[i].Length; j++)
                    sb.Append($"<c r=\"{(char)('A' + j)}{i + 1}\" t=\"inlineStr\"><is><t>{Xml(tabela[i][j])}</t></is></c>");
                sb.Append("</row>");
            }
            sb.Append("</sheetData></worksheet>");
            Add(zip, "xl/worksheets/sheet1.xml", sb.ToString());
        }
        return ms.ToArray();
    }

    static void Add(ZipArchive zip, string nome, string conteudo)
    {
        var e = zip.CreateEntry(nome, CompressionLevel.Optimal);
        using var w = new StreamWriter(e.Open(), new UTF8Encoding(false));
        w.Write(conteudo);
    }

    static string Xml(string t) =>
        t.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

    public static byte[] Pdf(List<string> linhas)
    {
        var paginas = linhas.Select(l => l.Length > 90 ? l[..90] : l).Chunk(44).ToList();
        if (paginas.Count == 0) paginas.Add(new[] { "" });
        int n = paginas.Count, total = 3 + 2 * n;

        using var ms = new MemoryStream();
        var offs = new List<int>();
        void W(string s) { var b = Encoding.Latin1.GetBytes(s); ms.Write(b, 0, b.Length); }
        void Obj(string corpo) { offs.Add((int)ms.Position); W($"{offs.Count} 0 obj\n{corpo}\nendobj\n"); }

        W("%PDF-1.4\n");
        Obj("<< /Type /Catalog /Pages 2 0 R >>");
        Obj($"<< /Type /Pages /Kids [{string.Join(" ", Enumerable.Range(0, n).Select(i => $"{4 + 2 * i} 0 R"))}] /Count {n} >>");
        Obj("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");

        for (int i = 0; i < n; i++)
        {
            var sb = new StringBuilder("BT\n/F1 11 Tf\n50 760 Td\n14 TL\n");
            foreach (var l in paginas[i]) sb.Append($"({Esc(l)}) Tj T*\n");
            sb.Append("ET");
            var conteudo = sb.ToString();
            Obj($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R >> >> /Contents {5 + 2 * i} 0 R >>");
            Obj($"<< /Length {Encoding.Latin1.GetByteCount(conteudo)} >>\nstream\n{conteudo}\nendstream");
        }

        var xref = (int)ms.Position;
        W($"xref\n0 {total + 1}\n0000000000 65535 f \n");
        foreach (var o in offs) W($"{o:0000000000} 00000 n \n");
        W($"trailer\n<< /Size {total + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF");
        return ms.ToArray();
    }

    static string Esc(string t) => t.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
}