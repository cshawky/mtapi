// See https://aka.ms/new-console-template for more information
using MtApi5;
using MTApiService;

Console.WriteLine("Connecting to server");
var client = new MtApi5Client();
client.ConnectionStateChanged += Client_ConnectionStateChanged;

void Client_ConnectionStateChanged(object? sender, Mt5ConnectionEventArgs e)
{
    if (e.Status == Mt5ConnectionState.Connected)
    {
        var quotes = client.GetQuotes();
        foreach (var quote in quotes)
        {
            Console.WriteLine($"{quote.Instrument}-{quote.Ask}-{quote.Bid}-{quote.ExpertHandle}");
        }
        Console.WriteLine($"SYMBOL_DESCRIPTION: {client.SymbolInfoString(quotes.First().Instrument, ENUM_SYMBOL_INFO_STRING.SYMBOL_DESCRIPTION)}");
    }
}

client.BeginConnect(8222);
Console.ReadLine();