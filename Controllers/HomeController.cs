using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PortalPegawai.Models;
using PortalPegawai.Data;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PortalPegawai.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public HomeController(AppDbContext context, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;
        _containerName = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONTAINER_NAME") ?? "berkas-pelamar";
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string nomorKendaraan, string satker, string nomorKupon, IFormFile fotoKtp)
    {
        if (fotoKtp == null || fotoKtp.Length == 0)
        {
            ViewBag.Message = "Error: Anda harus mengunggah Foto KTP!";
            return View("Index");
        }

        // 1. Mengunggah File ke Object Storage
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); // Pastikan container publik

        // Buat nama file unik agar tidak bentrok
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fotoKtp.FileName);
        var blobClient = containerClient.GetBlobClient(fileName);

        using (var stream = fotoKtp.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fotoKtp.ContentType });
        }

        // Dapatkan URL Publik
        string ktpUrl = blobClient.Uri.ToString();

        // 2. Menyimpan Teks ke Database
        var transaksiBaru = new Transaksi
        {
            NomorKendaraan = nomorKendaraan,
            Satker = satker,
            NomorKupon = nomorKupon,
            KtpUrl = ktpUrl
        };

        _context.transaksi.Add(transaksiBaru);
        await _context.SaveChangesAsync();

        ViewBag.Message = "Berhasil: Data transaksi berhasil didaftarkan!";
        ViewBag.KtpUrl = ktpUrl;
        return View("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
