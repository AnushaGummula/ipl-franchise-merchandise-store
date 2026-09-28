using IPL_Franchises.Domain.Entities;
using IPL_Franchises.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace IPL_Franchises.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        IPLDbContext context)
    {
        /*
         * =========================================
         * 1. ENSURE ALL FRANCHISES EXIST
         * =========================================
         */

        var franchiseSeedData =
            new[]
            {
                new
                {
                    Name = "Chennai Super Kings",
                    Code = "CSK"
                },

                new
                {
                    Name = "Mumbai Indians",
                    Code = "MI"
                },

                new
                {
                    Name = "Royal Challengers Bengaluru",
                    Code = "RCB"
                },

                new
                {
                    Name = "Sunrisers Hyderabad",
                    Code = "SRH"
                },

                new
                {
                    Name = "Kolkata Knight Riders",
                    Code = "KKR"
                },

                new
                {
                    Name = "Rajasthan Royals",
                    Code = "RR"
                },

                new
                {
                    Name = "Delhi Capitals",
                    Code = "DC"
                },

                new
                {
                    Name = "Punjab Kings",
                    Code = "PBKS"
                },

                new
                {
                    Name = "Gujarat Titans",
                    Code = "GT"
                },

                new
                {
                    Name = "Lucknow Super Giants",
                    Code = "LSG"
                }
            };


        var existingFranchises =
            await context.Franchises
                .ToListAsync();


        foreach (
            var seedFranchise
            in franchiseSeedData)
        {
            var existing =
                existingFranchises
                    .FirstOrDefault(
                        franchise =>
                            franchise.Code ==
                            seedFranchise.Code
                    );


            if (existing == null)
            {
                var franchise =
                    new Franchise
                    {
                        Name =
                            seedFranchise.Name,

                        Code =
                            seedFranchise.Code
                    };


                context.Franchises
                    .Add(franchise);


                existingFranchises
                    .Add(franchise);
            }
            else
            {
                /*
                 * Keep franchise metadata
                 * aligned with seed data.
                 */
                existing.Name =
                    seedFranchise.Name;
            }
        }


        await context
            .SaveChangesAsync();


        /*
         * Reload after SaveChanges
         * so newly inserted franchises
         * have their database IDs.
         */

        var franchises =
            await context.Franchises
                .ToDictionaryAsync(
                    franchise =>
                        franchise.Code,
                    franchise =>
                        franchise
                );


        /*
         * =========================================
         * 2. DEFINE FINAL 40 PRODUCT CATALOG
         * =========================================
         */

        var productSeedData =
            new List<SeedProduct>
            {
                /*
                 * =====================
                 * CSK
                 * =====================
                 */

                new(
                    "CSK",
                    "CSK Official Fan Jersey",
                    "Chennai Super Kings fan jersey for match-day and everyday fan wear.",
                    ProductType.Jersey,
                    1499m,
                    28
                ),

                new(
                    "CSK",
                    "CSK Team Cap",
                    "Chennai Super Kings team cap for supporters.",
                    ProductType.Cap,
                    699m,
                    34
                ),

                new(
                    "CSK",
                    "CSK Supporter Flag",
                    "Chennai Super Kings supporter flag for match-day celebrations.",
                    ProductType.Flag,
                    449m,
                    42
                ),

                new(
                    "CSK",
                    "CSK Autographed Player Photo",
                    "Collectible Chennai Super Kings signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1999m,
                    8
                ),


                /*
                 * =====================
                 * MI
                 * =====================
                 */

                new(
                    "MI",
                    "MI Official Fan Jersey",
                    "Mumbai Indians fan jersey for supporters.",
                    ProductType.Jersey,
                    1499m,
                    24
                ),

                new(
                    "MI",
                    "MI Team Cap",
                    "Mumbai Indians team cap for cricket fans.",
                    ProductType.Cap,
                    699m,
                    30
                ),

                new(
                    "MI",
                    "MI Supporter Flag",
                    "Mumbai Indians supporter flag for match day.",
                    ProductType.Flag,
                    499m,
                    38
                ),

                new(
                    "MI",
                    "MI Autographed Player Photo",
                    "Collectible Mumbai Indians signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    2099m,
                    7
                ),


                /*
                 * =====================
                 * RCB
                 * =====================
                 */

                new(
                    "RCB",
                    "RCB Fan Jersey",
                    "Royal Challengers Bengaluru fan jersey.",
                    ProductType.Jersey,
                    1599m,
                    22
                ),

                new(
                    "RCB",
                    "RCB Team Cap",
                    "Royal Challengers Bengaluru supporter cap.",
                    ProductType.Cap,
                    749m,
                    31
                ),

                new(
                    "RCB",
                    "RCB Supporter Flag",
                    "Royal Challengers Bengaluru supporter flag.",
                    ProductType.Flag,
                    449m,
                    40
                ),

                new(
                    "RCB",
                    "RCB Autographed Player Photo",
                    "Collectible Royal Challengers Bengaluru signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    2199m,
                    6
                ),


                /*
                 * =====================
                 * KKR
                 * =====================
                 */

                new(
                    "KKR",
                    "KKR Fan Jersey",
                    "Kolkata Knight Riders fan jersey.",
                    ProductType.Jersey,
                    1599m,
                    19
                ),

                new(
                    "KKR",
                    "KKR Team Cap",
                    "Kolkata Knight Riders supporter cap.",
                    ProductType.Cap,
                    749m,
                    27
                ),

                new(
                    "KKR",
                    "KKR Supporter Flag",
                    "Kolkata Knight Riders supporter flag.",
                    ProductType.Flag,
                    499m,
                    35
                ),

                new(
                    "KKR",
                    "KKR Autographed Player Photo",
                    "Collectible Kolkata Knight Riders signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1999m,
                    9
                ),


                /*
                 * =====================
                 * SRH
                 * =====================
                 */

                new(
                    "SRH",
                    "SRH Fan Jersey",
                    "Sunrisers Hyderabad fan jersey.",
                    ProductType.Jersey,
                    1399m,
                    25
                ),

                new(
                    "SRH",
                    "SRH Team Cap",
                    "Sunrisers Hyderabad team cap.",
                    ProductType.Cap,
                    699m,
                    32
                ),

                new(
                    "SRH",
                    "SRH Team Flag",
                    "Sunrisers Hyderabad supporter flag.",
                    ProductType.Flag,
                    449m,
                    41
                ),

                new(
                    "SRH",
                    "SRH Autographed Player Photo",
                    "Collectible Sunrisers Hyderabad signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1899m,
                    10
                ),


                /*
                 * =====================
                 * RR
                 * =====================
                 */

                new(
                    "RR",
                    "RR Fan Jersey",
                    "Rajasthan Royals fan jersey.",
                    ProductType.Jersey,
                    1499m,
                    20
                ),

                new(
                    "RR",
                    "RR Team Cap",
                    "Rajasthan Royals supporter cap.",
                    ProductType.Cap,
                    699m,
                    24
                ),

                new(
                    "RR",
                    "RR Supporter Flag",
                    "Rajasthan Royals supporter flag.",
                    ProductType.Flag,
                    449m,
                    33
                ),

                new(
                    "RR",
                    "RR Autographed Player Photo",
                    "Collectible Rajasthan Royals signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1999m,
                    7
                ),


                /*
                 * =====================
                 * DC
                 * =====================
                 */

                new(
                    "DC",
                    "DC Match Day Jersey",
                    "Delhi Capitals match-day fan jersey.",
                    ProductType.Jersey,
                    1499m,
                    23
                ),

                new(
                    "DC",
                    "DC Team Cap",
                    "Delhi Capitals supporter cap.",
                    ProductType.Cap,
                    699m,
                    29
                ),

                new(
                    "DC",
                    "DC Supporter Flag",
                    "Delhi Capitals supporter flag.",
                    ProductType.Flag,
                    449m,
                    37
                ),

                new(
                    "DC",
                    "DC Autographed Player Photo",
                    "Collectible Delhi Capitals signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1899m,
                    9
                ),


                /*
                 * =====================
                 * PBKS
                 * =====================
                 */

                new(
                    "PBKS",
                    "PBKS Fan Jersey",
                    "Punjab Kings fan jersey.",
                    ProductType.Jersey,
                    1499m,
                    18
                ),

                new(
                    "PBKS",
                    "PBKS Team Cap",
                    "Punjab Kings supporter cap.",
                    ProductType.Cap,
                    699m,
                    25
                ),

                new(
                    "PBKS",
                    "PBKS Team Flag",
                    "Punjab Kings supporter flag.",
                    ProductType.Flag,
                    449m,
                    31
                ),

                new(
                    "PBKS",
                    "PBKS Autographed Player Photo",
                    "Collectible Punjab Kings signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1799m,
                    8
                ),


                /*
                 * =====================
                 * GT
                 * =====================
                 */

                new(
                    "GT",
                    "GT Fan Jersey",
                    "Gujarat Titans fan jersey.",
                    ProductType.Jersey,
                    1499m,
                    21
                ),

                new(
                    "GT",
                    "GT Team Cap",
                    "Gujarat Titans supporter cap.",
                    ProductType.Cap,
                    749m,
                    28
                ),

                new(
                    "GT",
                    "GT Supporter Flag",
                    "Gujarat Titans supporter flag.",
                    ProductType.Flag,
                    499m,
                    34
                ),

                new(
                    "GT",
                    "GT Autographed Player Photo",
                    "Collectible Gujarat Titans signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1899m,
                    6
                ),


                /*
                 * =====================
                 * LSG
                 * =====================
                 */

                new(
                    "LSG",
                    "LSG Fan Jersey",
                    "Lucknow Super Giants fan jersey.",
                    ProductType.Jersey,
                    1499m,
                    20
                ),

                new(
                    "LSG",
                    "LSG Team Cap",
                    "Lucknow Super Giants supporter cap.",
                    ProductType.Cap,
                    699m,
                    27
                ),

                new(
                    "LSG",
                    "LSG Supporter Flag",
                    "Lucknow Super Giants supporter flag.",
                    ProductType.Flag,
                    449m,
                    36
                ),

                new(
                    "LSG",
                    "LSG Autographed Player Photo",
                    "Collectible Lucknow Super Giants signed-edition player photo for demo merchandise.",
                    ProductType.AutographedPhoto,
                    1899m,
                    5
                )
            };


        /*
         * =========================================
         * 3. INSERT MISSING PRODUCTS
         *
         * Franchise + ProductType uniquely
         * identifies one catalog item in this
         * assessment.
         * =========================================
         */

        var existingProducts =
            await context.Products
                .ToListAsync();


        foreach (
            var seedProduct
            in productSeedData)
        {
            var franchise =
                franchises[
                    seedProduct.FranchiseCode
                ];


            var existing =
                existingProducts
                    .FirstOrDefault(
                        product =>
                            product.FranchiseId ==
                                franchise.Id &&
                            product.ProductType ==
                                seedProduct.ProductType
                    );


            if (existing == null)
            {
                var product =
                    new Product
                    {
                        Name =
                            seedProduct.Name,

                        Description =
                            seedProduct.Description,

                        ProductType =
                            seedProduct.ProductType,

                        Price =
                            seedProduct.Price,

                        StockQuantity =
                            seedProduct.InitialStock,

                        FranchiseId =
                            franchise.Id
                    };


                context.Products
                    .Add(product);


                existingProducts
                    .Add(product);
            }
            else
            {
                /*
                 * Keep catalog metadata current.
                 *
                 * IMPORTANT:
                 * We deliberately do NOT overwrite
                 * StockQuantity here.
                 *
                 * Checkout reduces stock, so
                 * restarting the API must never
                 * reset inventory back to its
                 * original seed value.
                 */

                existing.Name =
                    seedProduct.Name;

                existing.Description =
                    seedProduct.Description;

                existing.ProductType =
                    seedProduct.ProductType;

                existing.Price =
                    seedProduct.Price;
            }
        }


        await context
            .SaveChangesAsync();
    }


    /*
     * Internal seed-data structure.
     */

    private sealed record SeedProduct(
        string FranchiseCode,
        string Name,
        string Description,
        ProductType ProductType,
        decimal Price,
        int InitialStock
    );
}
