namespace PortalPegawai.Models;

public class Transaksi
{
    public int Id { get; set; }
    public string NomorKendaraan { get; set; } = string.Empty;
    public string Satker { get; set; } = string.Empty;
    public string NomorKupon { get; set; } = string.Empty;
    public string KtpUrl { get; set; } = string.Empty;
}
