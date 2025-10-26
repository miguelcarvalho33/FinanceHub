using System.Text;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Parsers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;
using Xunit;

namespace FinanceHub.Tests.Parsers
{
 public class CgdExtratoGlobalParserTests
 {
 private readonly CgdExtratoGlobalParser _parser = new();

 [Fact]
 public void ParseMetadata_ParsesHeaderFields()
 {
 var sample = @"Cliente184578018\nExtrato n.º004/2025\nEmissão2025-04-04\nPeríodo2025-03-01 a2025-03-31\nIBAN PT50003503010069981253025\nNIB003503010069981253025\nSWIFT CGDIPTPL\nSaldo anterior1.113,55\nSaldo contabilístico997,96";
 var meta = _parser.ParseMetadata(sample);
 Assert.NotNull(meta);
 Assert.Equal("Caixa Geral de Depósitos", meta!.Bank);
 Assert.Equal("184578018", meta.ClientNumber);
 Assert.Equal("004/2025", meta.StatementNumber);
 Assert.Equal("2025-03-01 a2025-03-31", meta.Period);
 Assert.Equal("PT50003503010069981253025", meta.IBAN);
 Assert.Equal("003503010069981253025", meta.NIB);
 Assert.Equal("CGDIPTPL", meta.SWIFT);
 Assert.Equal(1113.55m, meta.PreviousBalance);
 Assert.Equal(997.96m, meta.FinalBalance);
 }

 [Fact]
 public void Parse_Movements_FindsAtLeastOne()
 {
 var sample = @"2025-03-012025-03-01 COMPRA TESTE10,00100,00";
 var txs = _parser.Parse(sample);
 Assert.Single(txs);
 Assert.Equal("Caixa Geral de Depósitos", txs[0].Bank);
 }

 [Fact]
 public void ParseDirectDebits_ParsesRows()
 {
 var sample = @"2024-04-03 MEO, SA PT73ZZZ10124340674604860 Sem Limite";
 var dds = _parser.ParseDirectDebits(sample);
 Assert.Single(dds);
 Assert.Equal("MEO, SA", dds[0].CreditorName);
 Assert.Equal("PT73ZZZ101243", dds[0].EntityNumber);
 Assert.Equal("40674604860", dds[0].AuthorizationNumber);
 Assert.Equal("Sem Limite", dds[0].LimitValueRaw);
 }

 [Fact]
 public void ParseLoansAndPayments_ParsesLoanAndPayments()
 {
 var sample = @"Crédito à habitação\nValor Contratado153.000,00\nData Contratação2022-11-03\nConta Débito0301699812530\nIndexante3,750 % + Spread0,000 % = TANL3,750 %\n2025-03-0528 COBRANCA DE CAPITAL149,78\n2025-03-0528 COBRANCA DE JUROS465,79\nSaldo devedor final148.904,27\nAgenda – Prestação Subsequente\n2025-04-0529 VENCIMENTO CAPITAL150,24\n2025-04-0529 VENCIMENTO JUROS465,33";
 var (loans, pays) = _parser.ParseLoansAndPayments(sample);
 Assert.Single(loans);
 var loan = loans[0];
 Assert.Equal(153000.00m, loan.ContractedValue);
 Assert.Equal(new DateTime(2022,11,3), loan.ContractDate);
 Assert.Equal("0301699812530", loan.AccountNumber);
 Assert.Equal(3.750m, loan.IndexRate);
 Assert.Equal(0.000m, loan.Spread);
 Assert.Equal(3.750m, loan.TANL);
 Assert.Equal(148904.27m, loan.OutstandingBalance);
 Assert.Equal(new DateTime(2025,4,5), loan.NextPaymentDate);
 Assert.Equal(150.24m, loan.NextCapitalPayment);
 Assert.Equal(465.33m, loan.NextInterestPayment);
 Assert.Equal(2, pays.Count);
 Assert.Contains(pays, p => p.CapitalAmount ==149.78m);
 Assert.Contains(pays, p => p.InterestAmount ==465.79m);
 }

 [Fact]
 public void ParseCards_ParsesCard()
 {
 var sample = @"CAIXA CLASSIC4124********5331 MIGUEL CARVALHO725,35 EUR";
 var cards = _parser.ParseCards(sample);
 Assert.Single(cards);
 Assert.Equal("4124********5331", cards[0].MaskedNumber);
 Assert.Equal(725.35m, cards[0].CreditUsed);
 Assert.Equal("EUR", cards[0].Currency);
 }
 }
}
