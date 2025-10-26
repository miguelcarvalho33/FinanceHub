using System.Globalization;
using FinanceHub.Web.Parsers;
using Xunit;

namespace FinanceHub.Tests.Parsers
{
 public class SantanderParserTests
 {
 private readonly SantanderParser _parser = new();

 [Fact]
 public void ParseMetadata_ParsesHeader()
 {
 var text = @"Banco Santander Totta, S.A.\nEXTRATO N.º081\nPeríodo2025-05-01 a2025-05-30\nIBAN: PT50001800034766233302066\nN.I.B.:001800034766233302066\nBIC/SWIFT: TOTAPTPL\nSaldo Inicial EUR1.234,56\n01-0501-05 AAA10,001.244,56\n31-0531-05 BBB -519,32725,24";
 var bs = _parser.ParseMetadata(text);
 Assert.Equal("Banco Santander Totta", bs.Bank);
 Assert.Equal("081", bs.StatementNumber);
 Assert.Equal("2025-05-01 a2025-05-30", bs.Period);
 Assert.Equal("PT50001800034766233302066", bs.IBAN);
 Assert.Equal("001800034766233302066", bs.NIB);
 Assert.Equal("TOTAPTPL", bs.SWIFT);
 Assert.Equal(1234.56m, bs.PreviousBalance);
 Assert.Equal(725.24m, bs.FinalBalance);
 }

 [Fact]
 public void Parse_ParsesTransactions()
 {
 var text = @"Período2025-05-01 a2025-05-30\n01-0502-05 TRF.IMED. DE MIGUEL170,001.404,56\n02-0502-05 JUROS ULTRAP. CRÉDITO -1,231.403,33";
 var txs = _parser.Parse(text);
 Assert.Equal(2, txs.Count);
 Assert.Equal(new DateTime(2025,5,1), txs[0].MovementDate);
 Assert.Equal(new DateTime(2025,5,2), txs[0].ValueDate);
 Assert.Equal("TRF.IMED. DE MIGUEL", txs[0].OriginalDescription);
 Assert.Equal(170.00m, txs[0].Amount);
 Assert.Equal("Banco Santander Totta", txs[0].Bank);
 }
 }
}
