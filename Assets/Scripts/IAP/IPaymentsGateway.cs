using System;
using System.Collections.Generic;

public interface IPaymentsGateway
{
    bool IsSupported { get; }

    void Purchase(string productId, Action<bool, Dictionary<string, string>> onComplete);

    void GetCatalog(Action<bool, List<Dictionary<string, string>>> onComplete);

    void GetPurchases(Action<bool, List<Dictionary<string, string>>> onComplete);

    void Consume(string productId, Action<bool, Dictionary<string, string>> onComplete);
}
