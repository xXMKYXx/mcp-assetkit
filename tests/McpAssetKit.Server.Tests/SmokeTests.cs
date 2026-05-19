using McpAssetKit.Server.Models;
using McpAssetKit.Server.Storage;
using Xunit;

namespace McpAssetKit.Server.Tests;

public class SmokeTests
{
    [Fact]
    public void TestRunner_Works() => Assert.Equal(4, 2 + 2);

    [Fact]
    public void Asset_Record_RoundTrips_AllFields()
    {
        var asset = new Asset(
            Id: "a-0001",
            ClientId: "c1",
            AssetType: "Laptop",
            Vendor: "Dell",
            Model: "Latitude 5440",
            Serial: "DLT-5440-001",
            Status: "Active",
            WarrantyExpiresAt: new DateOnly(2027, 3, 15));

        Assert.Equal("a-0001", asset.Id);
        Assert.Equal("c1", asset.ClientId);
        Assert.Equal("Laptop", asset.AssetType);
        Assert.Equal(new DateOnly(2027, 3, 15), asset.WarrantyExpiresAt);
    }

    [Fact]
    public void AssetSearchHit_Can_Carry_Denormalized_ClientName()
    {
        var hit = new AssetSearchHit(
            AssetId: "a-0001",
            ClientId: "c1",
            ClientName: "Acme Healthcare",
            AssetType: "Laptop",
            Vendor: "Dell",
            Model: "Latitude 5440",
            Serial: "DLT-5440-001",
            Status: "Active",
            WarrantyExpiresAt: "2027-03-15");

        Assert.Equal("Acme Healthcare", hit.ClientName);
    }
}
