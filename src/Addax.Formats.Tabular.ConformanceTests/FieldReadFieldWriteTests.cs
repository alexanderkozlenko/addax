#pragma warning disable IDE1006

using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.ConformanceTests;

[TestClass]
public sealed class FieldReadFieldWriteTests
{
    private static readonly string _assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "Assets");
    private static readonly byte[] _separators = { 0x31, 0x30 };

    [DataTestMethod]
    [DataRow("w3-csvw-20151217/test-utf8.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-20151217/test-utf8-bom.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-20151217/test-utf16.csv", "utf-16", "crlf-2c-22-22")]
    [DataRow("w3-csvw-20151217/test-utf16-bom.csv", "utf-16", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc02/csv_qs601ew2011wardh_151277.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc03/ug_tme_jrc_bg_v1.1.100.txt", "utf-8", "crlf-09-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc03/ug_tmi_jrc_bg_v1.1.100.txt", "utf-8", "crlf-09-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc03/ug_tmx_jrc_bg_v1.1.100.txt", "utf-8", "crlf-09-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc04/hefce_organogram_junior_data_31032011.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc04/hefce_organogram_senior_data_31032011.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc05/pp-monthly-update.100.txt", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc05/pp-monthly-update-new-version.100.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc06/plosone-search-results.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc08/ni.2449-s3.100.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc09/example-15.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc09/example-16.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc10/uganda_final.100.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc11/palo_alto_trees.100.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc15/mth-10-january-2014.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc18/binary_book_patterns_copy2.100.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc21/occurrence.100.txt", "utf-8", "lf-09-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc22/escc-payment-data-q2281011.100.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc23/hxl_3w_samples_draft_multilingual.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc24/2010_occupations.100.csv", "utf-8", "lf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc24/soc_structure_2010.100.csv", "utf-8", "crlf-2c-22-22")]
    [DataRow("w3-csvw-ucr-20160225/uc25/open_data_-_banes_public_toilets_-_15.09.14_-v3-2.csv", "utf-8", "crlf-2c-22-22")]
    public async Task W3(string filePath, string encodingName, string dialectScript)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream1 = File.OpenRead(Path.Combine(_assetsPath, filePath));
        await using var stream2 = new MemoryStream();

        using var hasher1 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        await using var reader1 = new TabularFieldReader(stream1, dialect, new(encoding: encoding));
        await using var writer1 = new TabularFieldWriter(stream2, dialect, new(encoding: encoding));

        while (await reader1.MoveNextRecordAsync(CancellationToken))
        {
            writer1.BeginRecord();

            while (await reader1.ReadFieldAsync(CancellationToken))
            {
                hasher1.AppendData(Encoding.Unicode.GetBytes(reader1.GetString()));
                hasher1.AppendData(_separators[..1]);

                writer1.Write(reader1.Value);
            }

            hasher1.AppendData(_separators[1..]);

            await writer1.FlushAsync(CancellationToken);
        }

        stream2.Seek(0, SeekOrigin.Begin);

        using var hasher2 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        await using var reader2 = new TabularFieldReader(stream2, dialect, new(encoding: encoding));

        while (await reader2.MoveNextRecordAsync(CancellationToken))
        {
            while (await reader2.ReadFieldAsync(CancellationToken))
            {
                hasher2.AppendData(Encoding.Unicode.GetBytes(reader2.GetString()));
                hasher2.AppendData(_separators[..1]);
            }

            hasher2.AppendData(_separators[1..]);
        }

        var hash1 = Convert.ToHexString(hasher1.GetCurrentHash());
        var hash2 = Convert.ToHexString(hasher2.GetCurrentHash());

        Assert.AreEqual(hash1, hash2);
    }

    private CancellationToken CancellationToken
    {
        get
        {
            return TestContext?.CancellationTokenSource?.Token ?? default;
        }
    }

    public TestContext? TestContext
    {
        get;
        set;
    }
}
